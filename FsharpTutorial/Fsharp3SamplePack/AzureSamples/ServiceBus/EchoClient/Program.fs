// Learn more about F# at http://fsharp.net

namespace Microsoft.ServiceBus.Samples

open System
open Microsoft.ServiceBus;
open System.ServiceModel;

[<ServiceContract(Name = "IEchoContract", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")>]
type IEchoContract = interface 
    [<OperationContract>]
    abstract member Echo : msg:string -> string
end

type IEchoChannel = interface
    inherit IEchoContract
    inherit IClientChannel
end

module Program = 

    [<EntryPoint>]
    let Main(args) = 

        ServiceBusEnvironment.SystemConnectivity.Mode <- ConnectivityMode.AutoDetect;
        
        let serviceNamespace = "testServiceBus0"
        let issuerName = "owner"
        let issuerSecret = "<your secret key>";

        let serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "EchoService");
        let sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
        sharedSecretServiceBusCredential.TokenProvider <- TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

        let channelFactory = new ChannelFactory<IEchoChannel>("RelayEndpoint", new EndpointAddress(serviceUri));

        channelFactory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential);

        let channel = channelFactory.CreateChannel();
        channel.Open();

        Console.WriteLine("Enter text to echo (or [Enter] to exit):");
        let mutable input = Console.ReadLine();
        while (input <> String.Empty) do
            try
                Console.WriteLine("Server echoed: {0}", channel.Echo(input));
            with
            | _ as e->
                Console.WriteLine("Error: " + e.Message);
            input <- Console.ReadLine();

        channel.Close();
        channelFactory.Close();

        0