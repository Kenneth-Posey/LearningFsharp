﻿namespace EveOnline.MarketDomain

module Market = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.UnionTypes
    open EveOnline.ProductDomain.Records
    open EveOnline.ProductDomain.Product
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Records
    open EveOnline.OreDomain.Ore
    open EveOnline.IceDomain.Records
    open EveOnline.IceDomain.Ice
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Parser
    open EveOnline.DataDomain.Collections
    open EveOnline.Geography.System

    // functions for finding the typeid of a material type
    let IceTypeId x = IceTypeId (x) (IsNotCompressed)
    let OreTypeId x = (OreData (x) (Common) (IsNotCompressed)).OreId

    let TypeId x = 
        match x with
        | Mineral x     -> MineralTypeid x
        | IceProduct x  -> IceProductTypeid x
        | IceType x     -> IceTypeId x
        | OreType x     -> OreTypeId x

    let StringId x = 
        string (TypeId x).Value


    // functions for finding the name of a material type
    let IceName x = RawIceName (x)
    let OreName x = RawOreName (x)

    let Name x = 
        match x with
        | IceType x -> IceName x
        | OreType x -> OreName x
        | Mineral x    -> MineralName x
        | IceProduct x -> IceProductName x


    // load a material's data
    let loadItem (loc:TradeHub) (item:Material)= 
        StringId item
        |> baseUrl loc
        |> loadUrl
        |> parse 

    // load a list of material's data
    let loadItems (loc:TradeHub) (items:Material list) = 
        items
        |> List.map (fun x -> loadItem loc x)


    // loads ice product prices based on the highest buy offer or lowest sell offer in system
    let loadIceProductPrices (orderType:OrderType) (loc:TradeHub) :IceProductPrices = 
        let loadItem (item:Material) = Price <|
            match orderType with
            | BuyOrder  -> (loadItem loc item).prices.highBuy
            | SellOrder -> (loadItem loc item).prices.lowSell

        {
            HeavyWater          = loadItem <| IceProduct HeavyWater 
            HeliumIsotopes      = loadItem <| IceProduct HeliumIsotopes
            HydrogenIsotopes    = loadItem <| IceProduct HydrogenIsotopes
            LiquidOzone         = loadItem <| IceProduct LiquidOzone
            NitrogenIsotopes    = loadItem <| IceProduct NitrogenIsotopes
            OxygenIsotopes      = loadItem <| IceProduct OxygenIsotopes
            StrontiumClathrates = loadItem <| IceProduct StrontiumClathrates
        }


    // loads mineral prices based on the highest buy offer or lowest sell offer in system
    let loadMineralPrices (orderType:OrderType) (loc:TradeHub) :MineralPrices =
        let loadItem (item:Material) = Price <|
            match orderType with
            | BuyOrder  -> (loadItem loc item).prices.highBuy
            | SellOrder -> (loadItem loc item).prices.lowSell

        {
            Tritanium = loadItem <| Mineral Tritanium
            Pyerite   = loadItem <| Mineral Pyerite
            Mexallon  = loadItem <| Mineral Mexallon
            Isogen    = loadItem <| Mineral Isogen
            Nocxium   = loadItem <| Mineral Nocxium
            Megacyte  = loadItem <| Mineral Megacyte
            Zydrine   = loadItem <| Mineral Zydrine
            Morphite  = loadItem <| Mineral Morphite
        }
        
        
    let accumulator = (fun total (refine, price) -> total + (single refine * price))
    let refineValueProcessor (pairs:(int *single) list) :Price =
        pairs |> List.fold accumulator (0.0f) |> Price

    
    // calculates the maximum market value of the yield of a single ice block
    let refineIceValue (refine:IceYield) (price:RefinePrice) :Price =          
        price 
        |> function
           | IceProductPrices x -> x
           | _ -> BaseIceProductPrices
        |> (fun price -> 
            [
                refine.HeliumIsotopes.Value,    price.HeliumIsotopes.Value
                refine.HydrogenIsotopes.Value,  price.HydrogenIsotopes.Value
                refine.NitrogenIsotopes.Value,  price.NitrogenIsotopes.Value
                refine.OxygenIsotopes.Value,    price.OxygenIsotopes.Value
            
                refine.HeavyWater.Value,        price.HeavyWater.Value
                refine.LiquidOzone.Value,       price.LiquidOzone.Value
                refine.StrontiumClathrates.Value,   price.StrontiumClathrates.Value
            ])
        |> refineValueProcessor

    
    // calculates the maximum market value of the yield of 100 ore units (one refine unit)   
    let refineOreValue (refine:OreYield) (price:RefinePrice) :Price =
        price 
        |> function
           | MineralPrices x -> x
           | _ -> BaseMineralPrices
        |> (fun price -> 
            [
                refine.Isogen.Value,      price.Isogen.Value    * 0.01f
                refine.Megacyte.Value,    price.Megacyte.Value  * 0.01f
                refine.Mexallon.Value,    price.Mexallon.Value  * 0.01f
                refine.Morphite.Value,    price.Morphite.Value  * 0.01f
                refine.Nocxium.Value,     price.Nocxium.Value   * 0.01f
                refine.Pyerite.Value,     price.Pyerite.Value   * 0.01f
                refine.Tritanium.Value,   price.Tritanium.Value * 0.01f
                refine.Zydrine.Value,     price.Zydrine.Value   * 0.01f
            ])
        |> refineValueProcessor
    

    // main volume function
    let GetVolume (com:Compressed) (mat:Material) :Volume = 
        match mat with
        | OreType x -> OreVolume x com
        | IceType _ -> RawIceVolume com
        | _ -> Volume 0.1f // to-do: insert actual refined product volumes
        

    // main yield function
    let GetYield (x:Material) :RefineYield = 
        match x with
        | IceType x -> IceYield <| RawIceYield x
        | OreType x -> OreYield <| RawOreYield x
        | IceProduct _ -> IceYield <| BaseIceYield
        | Mineral _ -> OreYield <| BaseOreYield
    

    // main refined product price function
    let GetPrice material order loc :RefinePrice = 
        match material with
        | RefinedProduct.Mineral     -> MineralPrices    <| loadMineralPrices order loc
        | RefinedProduct.IceProduct  -> IceProductPrices <| loadIceProductPrices order loc


    // main refine function
    let GetRefineValue (refine:RefineYield) (price:RefinePrice) :Price =
        match refine with
        | OreYield x -> refineOreValue x price
        | IceYield x -> refineIceValue x price
        

    // If I have the volume of 100 units and the value of 100 units, I can work out
    // the value per m^3 by dividing the value by the volume
    let GetVolumePrice (vol:Volume) price com mat :Price =         
        GetYield mat
        |> fun _yield -> GetRefineValue _yield price
        |> fun refine -> refine, GetVolume com mat
        |> fun (refine, unitVolume) -> refine.Value / unitVolume.Value * vol.Value
        |> Price
        
