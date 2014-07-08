// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 
namespace Support.Helper

open System
open System.Globalization
open System.IO
open System.Xml
open System.Xml.Linq
open Microsoft.FSharp.Reflection

[<AutoOpen>]
module SampleAttributes = 

    let getResourceString (tag, t) = 
        let resource = System.ComponentModel.ComponentResourceManager(t) 
        let culture = CultureInfo.CurrentCulture 
        try       
            let r = resource.GetString(tag, culture)
            r
        with _ -> tag

    [<System.AttributeUsage(AttributeTargets.All)>]
    type SampleAttribute (data) =
        inherit Attribute()
        member val IsResourceKey = true with get,set
        member this.Data = data
        member this.GetData() = 
            if this.IsResourceKey then
                let r = getResourceString (data, this.GetType())
                if String.IsNullOrEmpty r then data
                else r
            else
                data

    [<System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false)>]
    type TitleAttribute(title:string) = 
        inherit SampleAttribute(title)
        member this.Title = base.GetData()            

    [<System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false)>]
    type CategoryAttribute(category:string) = 
        inherit SampleAttribute(category)
        member this.Category = base.GetData()         

    [<System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false)>]
    type DescriptionAttribute(description:string) = 
        inherit SampleAttribute(description)
        member this.Description = base.GetData()

    [<System.AttributeUsage(AttributeTargets.All, AllowMultiple = true)>]
    type SupportAttribute(v:string) = 
        inherit SampleAttribute(v)
        member this.SampleName = this.Data
    
    [<System.AttributeUsage(AttributeTargets.All, AllowMultiple = true)>]
    type SupportAllAttribute() = 
        inherit SampleAttribute("")
        member this.SampleName = this.Data            

    [<System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false)>]
    type PrefixAttribute(prefix:string) = 
        inherit SampleAttribute(prefix)
        member this.Prefix = this.Data 

[<AutoOpen>]
module Utils = 
    type sample = 
        { 
          Run: (unit -> unit);
          Category: string;
          Name: string;
          Title: string;
          Description: string;
          File: string;
          Code: string;
          StartIndex: int 
        }

    // A dummy type, used to hook our own assembly.  A common .NET reflection idiom
    type ThisAssem = { dummy: int }

    let getXElementFromSample (data:sample) =     
        let category = XElement (XName.Get("Category"), data.Category)
        let name = XElement (XName.Get("Name"), data.Name)
        let title = XElement (XName.Get("Title"), data.Title)
        let code = XElement (XName.Get("Code"), data.Code)
        let description = XElement (XName.Get("Description"), data.Description)
        let file = XElement (XName.Get("File"), data.Title)
        let sample = XElement(XName.Get("Sample"), [|category; name; title; code; description; file|])
        sample

    let getFile (typ:System.Type, appdir) =         
        let dir = Path.Combine(appdir, @"..\..\") 
        let file = Path.Combine(dir, typ.FullName + ".fs")
        if not (File.Exists(file)) then
            let dir = Path.Combine(appdir, @".\") 
            let file = Path.Combine(dir, typ.FullName + ".fs")
            (dir, file)
        else
            (dir, file)

    let getSampleAttributes() =
        let assem = System.Reflection.Assembly.GetExecutingAssembly()
        assem.GetTypes() |> Seq.filter (fun n -> n.IsSubclassOf( typeof<System.Attribute> ))

    let attributeNames = 
        getSampleAttributes() 
        |> Seq.map (fun n -> [ n.Name; (n.Name.Replace("Attribute", String.Empty)) ] )
        |> Seq.concat
        |> Seq.toArray

    let minCutFromAttribute (allCode:string, blockStart:int) = 
        let names = attributeNames
        try
            attributeNames 
            |> Seq.map (fun n -> allCode.IndexOf((sprintf "[<%s" n), blockStart ))
            |> Seq.filter (fun n -> n>0)
            |> Seq.min
        with 
            | _ -> -1
    
    let getSamples () =
        let assem = typeof<ThisAssem>.Assembly 
        let appdir = AppDomain.CurrentDomain.BaseDirectory 

        // Search the types this program, which are fact F# modules.
        assem.GetTypes()
        |> Array.filter FSharpType.IsModule
        |> Array.map (fun m -> 
            let typ = m
            let dir, file = getFile(typ, appdir)

            // Collect up the samples each F# module...
            let samples = 
                m.GetMembers()
                // We only want the methods with a TitleAttribute, which should always be static
                |> Array.filter (fun m -> (m.GetCustomAttributes(typeof<TitleAttribute>,false)).Length <> 0) 
                // Prepare an entry for each one...
                |> Array.map (fun m -> 
                     let m = (m :?> System.Reflection.MethodInfo) 
                     let name = m.Name
                 
                     // Crack the related attributes...
                     let category = 
                         let arr = (m.GetCustomAttributes(typeof<CategoryAttribute>,false)) 
                         if arr.Length = 0 then "<no category>" else (arr.[0] :?> CategoryAttribute).Category 
                     let title = 
                         let arr = (m.GetCustomAttributes(typeof<TitleAttribute>,false)) 
                         if arr.Length = 0 then "<no title>" else (arr.[0] :?> TitleAttribute).Title 
                     let desc = 
                         let arr = (m.GetCustomAttributes(typeof<DescriptionAttribute>,false)) 
                         if arr.Length = 0 then "<no description>" else (arr.[0] :?> DescriptionAttribute).Description 
                     let code,blockStart = 
                         try 
                             let allCode = using (new StreamReader(file)) (fun sr -> sr.ReadToEnd())
                             let cut x = if x = -1 then allCode.Length else x 
                             let funcStart = allCode.IndexOf("let "+name)
                             let funcStart = min (allCode.LastIndexOf("#",funcStart,30) |> cut) funcStart
                             //printf "name = %s, funcStart = %O, #allCode = %d\n" name funcStart allCode.Length;
                             let codeBlock (blockStart:int) = 
                                 let blockEnd = 
                                     [(cut (minCutFromAttribute(allCode, blockStart ))); 
                                         (cut (allCode.IndexOf("(*",blockStart )))]
                                     |> Seq.min
                                 allCode.Substring(blockStart, blockEnd - blockStart)
                             let supportCode = 
                                 let supportAttribute = allCode.LastIndexOf("Support(\"" + name + "\")" ,funcStart)                              
                                 let supportCode =  if supportAttribute = -1 then "" 
                                                    else codeBlock(allCode.IndexOf("let",supportAttribute))
                                 let supportAllAttribute = allCode.LastIndexOf("SupportAll>]", funcStart)
                                 let supportAllCode = if supportAllAttribute = -1 then ""
                                                      else codeBlock(allCode.IndexOf("let", supportAllAttribute))
                                 supportCode + supportAllCode
                             let supportCode = 
                                let lines = supportCode.Split('\n')
                                if lines.[0].StartsWith("let dumm") then String.Join("\n", lines |> Seq.skip(1))
                                else supportCode
                             let code = codeBlock(funcStart) 
                             supportCode + code,funcStart
                         with e -> e.ToString(),0


                     // Build the sample description.  The code to invoke the sample uses reflection to invoke the
                     // method.
                     { Run = (fun () -> ignore(m.Invoke(null, Array.ofList [  ] )));
                       Category = category;
                       Name=name;
                       Title=title;
                       Description=desc;
                       StartIndex=blockStart;
                       Code= code;
                       File=file }) 

                // Set the samples this module by location the source file
                |> Array.toList
                |> List.sortBy (fun m -> m.StartIndex)
         
            let typName = let arr = typ.GetCustomAttributes(typeof<SampleAttribute>, false)
                          if arr.Length = 0 then "<No Module>" else (arr.[0] :?> SampleAttribute).GetData()
            typName, samples)
        |> Array.filter (fun (_,s) -> not(List.isEmpty s))
        |> Seq.sortBy (fun (nm,s) -> nm)
        |> Seq.toList

    type Assert() = 
        static member AreEqual(a,b) =
            if a=b then printfn "both elements are equal"
            else printfn "failwith not equal";
        static member IsTrue(b) = 
            if b then printfn "true"
            else printfn "failwith false but expected true"
        static member IsFalse(b) = 
            if b then printfn "false"
            else printfn "failwith true but expected false"
        static member IsNull(b) =
            if b=null then printfn "is null"
            else printfn "failwith expect null"
        static member IsNotNull(b) = 
            if b<>null then printfn "is null"
            else printfn "failwith expect null"
        static member IsNaN(d:float) =
            if System.Double.IsNaN(d) then printfn "is Double.NaN"
            else printfn "failwith expect Double.NaN"
        static member IsEmpty (n) = 
            let empty = n |> Seq.isEmpty
            if empty then printfn "is empty"
            else printfn "failwith not empty"
        static member IsNotEmpty (n) = 
            let empty = n |> Seq.isEmpty
            if not empty then printfn "is not empty"
            else printfn "failwith empty"
        static member Greater(a,b) =
            if a>b then printfn "a > b"
            else printfn "failwith a <= b"
        static member Less(a,b) = 
            if a>b then printfn "a < b"
            else printfn "failwith a >= b"
        static member Fail(str) =
            printfn "failwith %s" str

    #if XML_SAMPLE
    let getExecutionResult sampleOption = 
        use stream = new MemoryStream()  
        use writer = new StreamWriter(stream)  
        stream.SetLength(0L);
        match sampleOption with 
          | Some s -> 
            let oldConsoleOut = Console.Out  
            try 
              Console.SetOut(writer);
              try s.Run()
              with e -> 
                Console.WriteLine("Exception raised: {0}",e)
            finally Console.SetOut(oldConsoleOut)
          | None -> ()
        writer.Flush();
        writer.Encoding.GetString(stream.ToArray());

    let getXml (data:(string*sample list) list) = 
        let r = data
                |> Seq.map (fun (_, sampleList) -> 
                            sampleList |> Seq.map (fun sample -> 
                                                        let node = getXElementFromSample sample
                                                        let result = getExecutionResult (Some(sample))
                                                        node.SetElementValue(XName.Get("Result"), result)
                                                        node))
                |> Seq.concat
                |> Seq.toList
        let root = XElement(XName.Get("SampleList"), r)
        root.SetAttributeValue(XName.Get("TotalSample"), r |> Seq.length)
        let doc = XDocument( root )
        doc
    #endif

    //To sign up for a Windows Azure Marketplace account @ https://datamarket.azure.com/account/info
    let ADM_USER_ID = "<your email address>"
    let ADM_ACCOUNT_ID = "<your ADM ID>"

    //To sign up for a Bing service developer account @ https://www.bingmapsportal.com/application/index/1034110
    let BING_APP_ID = "<your bing app id>"
