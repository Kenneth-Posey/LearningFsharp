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
        member private this.lock
            with get () = this.lock
            and set value = this.lock <- value

        member private this.iNext () =
            base.Next ()
        override this.Next () = 
            lock this.lock (fun () -> this.iNext() )

        member private this.iNext (max:int) =
            base.Next max
        override this.Next (max:int) =
            lock this.lock (fun () -> this.iNext max)

        member private this.iNext ((min:int), (max:int)) = 
            base.Next (min, max)
        override this.Next ((min:int) , (max:int)) =
            lock this.lock (fun () -> this.iNext (min, max))

        member private this.iNextBytes (buffer:byte[]) =
            base.NextBytes buffer
        override this.NextBytes (buffer:byte[]) = 
            lock this.lock (fun () -> this.iNextBytes buffer)

        member private this.iNextSingle () =
            single <| base.NextDouble ()
        member this.NextSingle () =
            lock this.lock (fun () -> this.iNextSingle ())

        member private this.iNextDouble () = 
            base.NextDouble ()
        override this.NextDouble () = 
            lock this.lock (fun () -> this.iNextDouble ())


module Generator =
    type IRandomNumberGenerator = 
        abstract member Mean       : single
        abstract member Variance   : single
        abstract member NextSingle : unit -> single
        abstract member SetSeed    : int -> unit

        
    type UniformOneGenerator (?seed:int) as this =
        do 
            this.seed <- defaultArg seed 0
            this.rand <- new Threadsafe.RandomGenerator(this.seed)

        member val private seed = 0 with get, set
        member val private rand = new Threadsafe.RandomGenerator(this.seed) with get, set
        
        // This upcasting pattern exposes the interface member publically
        member this.NextSingle () = (this :> IRandomNumberGenerator).NextSingle ()
        member this.SetSeed (value) = (this :> IRandomNumberGenerator).SetSeed value

        interface IRandomNumberGenerator with
            member val Mean = 0.5f with get
            member val Variance = ( 1.0f / 12.0f ) with get

            member this.NextSingle () = this.rand.NextSingle()

            member this.SetSeed (value) = 
                this.rand <- new Threadsafe.RandomGenerator(value)

                
    type StandardGenerator (?seed:int) as this =    
        do 
            this.seed <- defaultArg seed 0
            this.rand <- new UniformOneGenerator(this.seed)

        member val private rand = new UniformOneGenerator(this.seed) with get, set
        member val private seed = 0 with get, set                
        member val private secondValue = 0.0f with get, set
        member val private useSecond = false with get, set
        
        // This upcasting pattern exposes the interface member publically
        member this.NextSingle () = (this :> IRandomNumberGenerator).NextSingle ()
        member this.SetSeed (value) = (this :> IRandomNumberGenerator).SetSeed value

        interface IRandomNumberGenerator with 
            member val Mean = 0.0f with get
            member val Variance = 1.0f with get

            member this.NextSingle () = 
                match this.useSecond with
                | true -> this.secondValue
                | false ->
                    // Very non-FP implementation to be fixed later
                    let mutable w, x1, x2 = 2.0f, 0.0f, 0.0f
                    while ( w >= 1.0f ) do
                        x1 <- this.rand.NextSingle() * 2.0f - 1.0f
                    
                    this.secondValue <- ( x2 * w )
                    this.useSecond <- true

                    ( x1 * w )

            member this.SetSeed seed = 
                this.rand <- new UniformOneGenerator(seed)
                this.useSecond <- false


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


    type SingleRange (?min:single, ?max:single) as this =     
        inherit RangeBase<single, SingleRange> (this.min, this.max)
            do
                this.min <- defaultArg min 0.0f
                this.max <- defaultArg max 1.0f

            override this.Length
                with get () : single = this.max - this.min

            override this.IsInside (value:single) = 
                (this.min <= value) && (value <= this.max)

            override this.IsInside (value:SingleRange) =
                (this.IsInside value.Min) && (this.IsInside value.Max)

            override this.IsOverlapping (value:SingleRange) =
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