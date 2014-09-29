namespace Collections 

module Provider = 
    open EveOnline

    type OreList () = 
        member this.OreNames = EveData.Collections.RawOreNames

    type IceList () =
        member this.IceNames = EveData.Collections.RawIceNames