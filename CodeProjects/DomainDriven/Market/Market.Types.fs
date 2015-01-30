namespace EveOnline.MarketDomain

module Types = 
    open EveOnline.OreDomain.Records
    open EveOnline.IceDomain.Records
    open EveOnline.ProductDomain.Records


    type RefinePrice = 
    | MineralPrices of MineralPrices
    | IceProductPrices of IceProductPrices

    type RefineYield = 
    | IceYield of IceYield 
    | OreYield of OreYield

    type RefinedProduct = 
    | Mineral
    | IceProduct

