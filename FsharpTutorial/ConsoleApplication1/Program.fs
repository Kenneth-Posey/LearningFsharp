
namespace Battleshits

    module TileTypes = 
        type Orientation = 
            | Vertical      = 0
            | Horizontal    = 1
    
        type Terrain = 
            | Water = 0
            | Rock  = 1
            | Land  = 2

        type Shipinfo = 
            {
                Length      : int
                Width       : int
                Direction   : Orientation
            }

        type Shiptype =
            | Frigate       of Info : Shipinfo
            | Destroyer     of Info : Shipinfo
            | Cruiser       of Info : Shipinfo
            | Battlecruiser of Info : Shipinfo
            | Battleship    of Info : Shipinfo
            | Carrier       of Info : Shipinfo

        type GridSquare = 
            | ShipTile      of Shiptype : Shiptype
            | TerrainTile   of Terrain  : Terrain 

        
        open System
        open System.Threading
        //
        //[<EntryPoint>]
        //let main pArgumentValueArray = 
        let learningfunction arguments =
            let defaultship     = { Length = 1; Width = 1; Direction = Orientation.Vertical }
            let frigate         = { defaultship with Length = 2 }
            let destroyer       = { defaultship with Length = 3 }
            let cruiser         = { defaultship with Length = 4 } 
            let battlecruiser   = { defaultship with Length = 3; Width = 2 }

            printfn "%A" frigate
            0 // return an integer exit code

























