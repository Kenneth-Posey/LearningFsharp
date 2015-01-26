namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.ProductDomain.Types
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Providers
    open EveOnline.MarketDomain.Parser
    open EveOnline.MarketDomain.Market
    open EveOnline.DataDomain.Collections

    open EveOnline.IceDomain.Types
    [<EntryPoint>]
    let main (_) = 
        // let getPrice x = GetPrice x (OrderType.BuyOrder) (Jita)
        let iceProductPrices = GetPrice (RefinedProduct.IceProduct) (OrderType.BuyOrder) (Jita)  
        let mineralPrices = GetPrice (RefinedProduct.Mineral) (OrderType.BuyOrder) (Jita)
        let vol = Volume 1000.0f

//        for ice in IceList do
//            let value = sprintf "%0.2f" (GetVolumePrice vol iceProductPrices IsNotCompressed ice).Value
//            System.Console.WriteLine ( "Ice " + (Name ice).Value
//                + " with value: " + value
//            )
        
        let output (x, y) = System.Console.WriteLine ("Ore: " + x + " with value: " + (sprintf "%0.2f" y))
                
        OreList
        |> List.map (fun ore -> ore, GetVolumePrice vol mineralPrices IsNotCompressed ore)
        |> List.sortWith (fun (_ , val1) (_ , val2) -> 
                match val1.Value <> val2.Value with
                | true when val1.Value > val2.Value -> 1
                | true when val1.Value < val2.Value -> -1
                | _ -> 0 )           
        |> List.map (fun (ore, value) -> ((Name ore).Value, value.Value) |> output )
        |> ignore


        0 