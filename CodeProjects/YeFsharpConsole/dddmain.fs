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
        // let iceProductPrices = GetPrice (RefinedProduct.IceProduct) (OrderType.BuyOrder) (Jita)  
        let mineralPrices = GetPrice (RefinedProduct.Mineral) (OrderType.BuyOrder) (Jita)
        
        // let vol = Volume 45480.0f // Max prospect with 3 mlus 750 per 60 seconds
        // let vol = Volume 100120.0f // Max retriever with 3 mlus
        let vol = Volume 162663.0f // 29.5 cycles of 1838

//        for ice in IceList do
//            let value = sprintf "%0.2f" (GetVolumePrice vol iceProductPrices IsNotCompressed ice).Value
//            System.Console.WriteLine ( "Ice " + (Name ice).Value
//                + " with value: " + value
//            )

        let print (x:string) = System.Console.WriteLine x        
        let round (x:single) = int (System.Math.Round (float x))

        let output (x, y) = print ("Ore: " + x + " with value: " + (sprintf "%i" (round y)))

        OreList
        |> List.map (fun ore -> ore, GetVolumePrice vol mineralPrices IsNotCompressed ore)
        |> List.sortWith (fun (_ , val1) (_ , val2) -> 
                match val1.Value <> val2.Value with
                | true when val1.Value < val2.Value -> 1
                | true when val1.Value > val2.Value -> -1
                | _ -> 0 )           
        |> List.map (fun (ore, value) -> ore, Price (value.Value * 0.696f))
        |> List.map (fun (ore, value) -> (Name ore).Value, value.Value)
        //|> List.map (fun (ore, value) -> ore)
        |> List.map output
        |> ignore


        0 