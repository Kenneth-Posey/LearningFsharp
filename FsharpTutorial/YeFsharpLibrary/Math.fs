﻿namespace MathAlgorithms

module Threadsafe = 
    open System
    // Specifies a "lock" to allow multithread access to 
    // a RNG created from the same seed 
    type RandomGenerator = 
        inherit Random         
        new () = { inherit Random(); }
        new (value:int) = { inherit Random(value); }
        
        // Empty object to give threads something to lock onto 
        member internal this.lock
            with get () = this.lock
            and set value = this.lock <- value

        member internal this.iNext () =
            base.Next ()
        override this.Next () = 
            lock this.lock (fun () -> this.iNext() )

        member internal this.iNext (max:int) =
            base.Next max
        override this.Next (max:int) =
            lock this.lock (fun () -> this.iNext max)

        member internal this.iNext ((min:int), (max:int)) = 
            base.Next (min, max)
        override this.Next ((min:int) , (max:int)) =
            lock this.lock (fun () -> this.iNext (min, max))

        member internal this.iNextBytes (buffer:byte[]) =
            base.NextBytes buffer
        override this.NextBytes (buffer:byte[]) = 
            lock this.lock (fun () -> this.iNextBytes buffer)

        member internal this.iNextDouble () = 
            base.NextDouble ()
        override this.NextDouble () = 
            lock this.lock (fun () -> this.iNextDouble ())

module MathStructures = 
    type Range =     
        new () = Range () 
        new (min:single, max:single) = Range (min, max)

        member private this.min
            with get () : single = this.min
            and  set (value:single) = this.min <- value

        member private this.max
            with get () : single = this.max
            and  set (value:single) = this.max <- value

        member this.Min
            with get () : single = this.min

        member this.Max
            with get () : single = this.max

        member this.Length
            with get () : single = this.max - this.min

        member this.IsInside (value:single) = 
            (this.min <= value) && (value <= this.max)

        member this.IsInside (value:Range) =
            (this.IsInside value.Min) && (this.IsInside value.Max)

        member this.IsOverlapping (value:Range) =
            let internalRangeOverlap = this.IsInside value.Min || this.IsInside value.Max
            let externalRangeOverlap = value.IsInside this.min || value.IsInside this.max

            internalRangeOverlap || externalRangeOverlap

        // Partially implemented, still need IntRange conversion, equality and ToString

    type DoubleRange = 
        new () = DoubleRange () 
        new (min:double, max:double) = DoubleRange (min, max)

        member private this.min
            with get () : double = this.min
            and  set (value:double) = this.min <- value

        member private this.max
            with get () : double = this.max
            and  set (value:double) = this.max <- value

        member this.Min
            with get () : double = this.min

        member this.Max
            with get () : double = this.max

        member this.Length
            with get () : double = this.max - this.min

        member this.IsInside (value:double) = 
            (this.min <= value) && (value <= this.max)

        member this.IsInside (value:DoubleRange) =
            (this.IsInside value.Min) && (this.IsInside value.Max)

        member this.IsOverlapping (value:DoubleRange) =
            let internalRangeOverlap = this.IsInside value.Min || this.IsInside value.Max
            let externalRangeOverlap = value.IsInside this.min || value.IsInside this.max

            internalRangeOverlap || externalRangeOverlap