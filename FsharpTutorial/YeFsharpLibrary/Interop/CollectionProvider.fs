namespace EveOnlineInterop 

module CollectionsProvider = 
    open EveOnline

    type OreList () = 
        static member OreNames = 
            List.toSeq <| EveData.Collections.RawOreNames

    type IceList () =
        static member IceNames = 
            List.toSeq <| EveData.Collections.RawIceNames

    type IceProductList () = 
        static member IceProductNames = 
            List.toSeq <| EveData.Collections.IceProductNames

    type MineralList () = 
        static member MineralNames = 
            List.toSeq <| EveData.Collections.MineralNames
