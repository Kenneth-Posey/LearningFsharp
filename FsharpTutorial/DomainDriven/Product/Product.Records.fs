namespace EveOnline.ProductDomain

module Records = 
    open EveOnline.ProductDomain.Types

    type MineralData = {
        TypeId  : TypeId
        Name    : Name
        Mineral : Mineral
    }

    type IceProductData = {
        TypeId     : TypeId
        Name       : Name
        IceProduct : IceProduct
    }



