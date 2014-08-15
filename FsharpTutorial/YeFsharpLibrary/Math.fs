namespace MathAlgorithms

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
    [<AbstractClass>]
    type RangeBase<'T, 'R> ( min:'T, max:'T ) as this =
        // 'T = type of range values
        // 'R = type of range object
        do
            this.min <- min
            this.max <- max
        
        member internal this.min
            with get () : 'T = this.min
            and  set (value:'T) = this.min <- value
            
        member internal this.max
            with get () : 'T = this.max
            and  set (value:'T) = this.max <- value
            
        member this.Min
            with get () : 'T = this.min

        member this.Max
            with get () : 'T = this.max

        // Must be implemented by inheriting classes
        abstract member Length : 'T
        abstract member IsInside : 'T -> bool
        abstract member IsInside : 'R -> bool
        abstract member IsOverlapping : 'R -> bool

    type Range (?min:single, ?max:single) as this =     
        inherit RangeBase<single, Range> (this.min, this.max)
            do
                this.min <- defaultArg min 0.0f
                this.max <- defaultArg max 1.0f

            override this.Length
                with get () : single = this.max - this.min

            override this.IsInside (value:single) = 
                (this.min <= value) && (value <= this.max)

            override this.IsInside (value:Range) =
                (this.IsInside value.Min) && (this.IsInside value.Max)

            override this.IsOverlapping (value:Range) =
                let internalRangeOverlap = this.IsInside value.Min || this.IsInside value.Max
                let externalRangeOverlap = value.IsInside this.min || value.IsInside this.max

                internalRangeOverlap || externalRangeOverlap

            // Partially implemented, still need IntRange conversion, equality and ToString

    type DoubleRange (?min:double, ?max:double) as this = 
        inherit RangeBase<double, DoubleRange> (this.min, this.max)
            do
                this.min <- defaultArg min 0.0
                this.max <- defaultArg max 1.0

            override this.Length
                with get () : double = this.max - this.min

            override this.IsInside (value:double) = 
                (this.min <= value) && (value <= this.max)

            override this.IsInside (value:DoubleRange) =
                (this.IsInside value.Min) && (this.IsInside value.Max)

            override this.IsOverlapping (value:DoubleRange) =
                let internalRangeOverlap = this.IsInside value.Min || this.IsInside value.Max
                let externalRangeOverlap = value.IsInside this.min || value.IsInside this.max

                internalRangeOverlap || externalRangeOverlap

                
            // Partially implemented, still need IntRange conversion, equality and ToString