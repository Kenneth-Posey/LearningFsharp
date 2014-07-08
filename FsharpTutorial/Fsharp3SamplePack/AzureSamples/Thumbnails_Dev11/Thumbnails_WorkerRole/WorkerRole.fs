namespace Microsoft.Samples.ServiceHosting.Thumbnails

open System
open System.Collections.Generic
open System.Configuration
open System.Diagnostics
open System.Drawing
open System.IO
open System.Text
open System.Linq
open System.Net
open Microsoft.WindowsAzure
open Microsoft.WindowsAzure.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime
open Microsoft.WindowsAzure.StorageClient

type public WorkerRole() =
    inherit RoleEntryPoint() 
    [<DefaultValue>]
    val mutable width : int

    [<DefaultValue>]
    val mutable height : int

    [<DefaultValue>]
    val mutable configSetter : string * bool -> unit

    member private this.CreateThumbnail( input : Stream ) =                 
        let orig = new Bitmap(input)

        if(orig.Width > orig.Height) then
            this.width <- 128
            this.height <- 128 * orig.Height / orig.Width
        else 
            this.height <- 128
            this.width <- 128 * orig.Width / orig.Height

        let thumb = new Bitmap(this.width,this.height)

        use graphic = Graphics.FromImage(thumb)
        graphic.InterpolationMode <- System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
        graphic.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        graphic.PixelOffsetMode <- System.Drawing.Drawing2D.PixelOffsetMode.HighQuality

        graphic.DrawImage(orig,0,0,this.width,this.height)

        let ms = new MemoryStream()
        thumb.Save(ms,System.Drawing.Imaging.ImageFormat.Jpeg)

        ms.Seek(0L,SeekOrigin.Begin) |> ignore

        ms

    override this.OnStart() = 
        // This code sets up a handler to update CloudStorageAccount instances when their corresponding
        // configuration settings change in the service configuration file.
        CloudStorageAccount.SetConfigurationSettingPublisher(
            new Action<string,Func<string,bool>>(
                fun (configName:string) (configSetter:Func<string,bool>) -> 
                    // Provide the configSetter with the initial value
                    configSetter.Invoke(RoleEnvironment.GetConfigurationSettingValue(configName)) |> ignore
                    RoleEnvironment.Changed.Add(fun (arg : RoleEnvironmentChangedEventArgs) ->
                        if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any(fun change -> change.ConfigurationSettingName = configName)) then
                            if (not (configSetter.Invoke(RoleEnvironment.GetConfigurationSettingValue(configName)))) then
                                // In this case, the change to the storage account credentials in the
                                // service configuration is significant enough that the role needs to be
                                // recycled in order to use the latest settings. (for example, the 
                                // endpoint has changed)
                                RoleEnvironment.RequestRecycle()
                    )      
        ))
        
        base.OnStart()

    override this.Run() = 
        let storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"))
        let blobStorage = storageAccount.CreateCloudBlobClient()
        let container = blobStorage.GetContainerReference("photogallery")

        let queueStorage = storageAccount.CreateCloudQueueClient()
        let queue = queueStorage.GetQueueReference("thumbnailmaker")

        Trace.TraceInformation("Creating container and queue...")

        // If the Start() method throws an exception, the role recycles.
        // If this sample is run locally and the development storage tool has not been started, this 
        // can cause a number of exceptions to be thrown because roles are restarted repeatedly.
        // Lets try to create the queue and the container and check whether the storage services are running
        // at all.
        let mutable containerAndQueueCreated = false

        while(not containerAndQueueCreated) do
            try
                container.CreateIfNotExist() |> ignore
                let mutable permissions = container.GetPermissions()
                permissions.PublicAccess <- BlobContainerPublicAccessType.Container

                container.SetPermissions(permissions)

                permissions <- container.GetPermissions()

                queue.CreateIfNotExist() |> ignore

                containerAndQueueCreated <- true

            with
            | :? StorageClientException as e -> 
                    if (e.ErrorCode = StorageErrorCode.TransportError) then
                        Trace.TraceError(String.Format("Connect failure! The most likely reason is that the local "+
                                                        "Development Storage tool is not running or your storage account configuration is incorrect. "+
                                                        "Message: '{0}'", e.Message))
                        System.Threading.Thread.Sleep(5000);

                    else
                        raise e


        Trace.TraceInformation("Listening for queue messages...")

         // Now that the queue and the container have been created in the above initialization process, get messages
        // from the queue and process them individually.
        while (true) do
            try
                let msg = queue.GetMessage()
                if (box(msg) <> null) then
                    let path = msg.AsString
                    let thumbnailName = System.IO.Path.GetFileNameWithoutExtension(path) + ".jpg"
                    Trace.TraceInformation(String.Format("Dequeued '{0}'", path))
                    let content = container.GetBlockBlobReference(path)
                    let thumbnail = container.GetBlockBlobReference("thumbnails/" + thumbnailName)
                    let image = new MemoryStream()

                    content.DownloadToStream(image)
                    
                    image.Seek(0L, SeekOrigin.Begin) |> ignore
                      
                    thumbnail.Properties.ContentType <- "image/jpeg"
                    thumbnail.UploadFromStream(this.CreateThumbnail(image))

                    Trace.TraceInformation(String.Format("Done with '{0}'", path))

                    queue.DeleteMessage(msg)
                
                else
                    System.Threading.Thread.Sleep(1000)
        
            // Explicitly catch all exceptions of type StorageException here because we should be able to 
            // recover from these exceptions next time the queue message, which caused this exception,
            // becomes visible again.
            with
            | _ as e -> 
                                System.Threading.Thread.Sleep(5000)
                                Trace.TraceError(String.Format("Exception when processing queue item. Message: '{0}'", e.Message))