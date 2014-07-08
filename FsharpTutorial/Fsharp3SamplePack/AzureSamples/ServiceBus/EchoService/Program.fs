// Learn more about F# at http://fsharp.net
namespace Microsoft.ServiceBus.Samples

open System

open Microsoft.ServiceBus
open System.ServiceModel


[<ServiceContract(Name = "IEchoContract", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")>]
type IEchoContract = interface 
    [<OperationContract>]
    abstract member Echo : msg:string -> string
end

type IEchoChannel = interface
    inherit IEchoContract
    inherit IClientChannel
end

[<ServiceBehavior(Name = "EchoService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")>]
type EchoService() = class
    interface IEchoContract with
        member this.Echo(msg) = 
            printfn "%s" msg
            msg
end

module Program = 

    [<EntryPoint>]
    let Main(args) = 
        let serviceNamespace = "testservicebus0"
        let issuerName = "owner"
        let issuerSecret = "<your secret key>";

        let sharedSecretServiceBusCredential = TransportClientEndpointBehavior()
        sharedSecretServiceBusCredential.TokenProvider <- TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

        let address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "EchoService");
        let host = new ServiceHost(typeof<EchoService>, address);
        let serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

        for endpoint in host.Description.Endpoints do
            endpoint.Behaviors.Add(serviceRegistrySettings);
            endpoint.Behaviors.Add(sharedSecretServiceBusCredential)

        host.Open();

        Console.WriteLine("Service address: " + address.ToString());
        Console.WriteLine("Press [Enter] to exit");
        Console.ReadLine() |> ignore

        host.Close()

        0