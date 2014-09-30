namespace EveOnlineInterop 

module CollectionsProvider = 
    open EveData

    type OreList () = 
        static member OreNames = 
            List.toSeq <| Collections.RawOreNames

    type IceList () =
        static member IceNames = 
            List.toSeq <| Collections.RawIceNames

    type IceProductList () = 
        static member IceProductNames = 
            List.toSeq <| Collections.IceProductNames

    type MineralList () = 
        static member MineralNames = 
            List.toSeq <| Collections.MineralNames
