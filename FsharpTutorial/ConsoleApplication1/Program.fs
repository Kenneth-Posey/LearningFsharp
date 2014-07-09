// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open System.Threading

module TileTypes = 
    type Orientation = 
        | Vertical = 0
        | Horizontal = 1
    
    type Terrain = 
        | Water = 0
        | Rock = 1
        | Land = 2

    type Shipinfo = 
        {
            Length : int;
            Width : int;
            Direction : Orientation
        }

    type Shiptype =
        | Frigate of Info : Shipinfo
        | Destroyer of Info : Shipinfo
        | Cruiser of Info : Shipinfo
        | Battlecruiser of Info : Shipinfo
        | Carrier of Info : Shipinfo

    type gridSquare = 
        | Ship of Kind : Shiptype
        | Terrain of Kind : Terrain 


open TileTypes
[<EntryPoint>]
let main pArgumentValueArray = 

    let grid = [| for x in 1 .. 11 ->
                    gridSquare.Terrain
                    |]
























    0 // return an integer exit code
