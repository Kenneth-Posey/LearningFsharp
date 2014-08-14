namespace Neuro

module Neuron = 
    open System
    open MathAlgorithms.Threadsafe
    open MathAlgorithms.MathStructures

    [<AbstractClass>]
    type NeuronBase (inputs:int) as this = 
        do
            this.rand        <- new RandomGenerator ()
            this.randomRange <- new Range (0.0, 1.0)
            this.inputsCount <- Math.Max (1 , inputs)
            this.weights     <- Array.zeroCreate<double> this.inputsCount
            this.Randomize ()

        // This is a function that's run only for its side effects 
        abstract member Randomize : unit -> unit
        default this.Randomize () = 
            let length = this.randomRange.Length
            for x in 1 .. this.inputsCount - 1 do
                this.weights.[x] <- (this.rand.NextDouble() * length) + this.randomRange.Min
        
        // Implemented in inherited classes
        abstract member Compute : double[] -> double

        member private this.inputsCount
            with get () : int = this.inputsCount
            and  set (value:int) = this.inputsCount <- value

        member this.InputsCount
            with get () : int = this.inputsCount

        member private this.weights
            with get () : double[] = this.weights
            and  set (value:double[]) = this.weights <- value

        member this.Weights
            with get () : double[] = this.weights

        member private this.output
            with get () : double = this.output
            and  set (value:double) = this.output <- value

        member this.Output
            with get () : double = this.output

        member private this.rand
            with get () : RandomGenerator = this.rand
            and  set (value:RandomGenerator) = this.rand <- value

        member this.RandGenerator
            with get () : RandomGenerator = this.rand

        member private this.randomRange
            with get () : Range = this.randomRange
            and  set (value:Range) = this.randomRange <- value

        member this.RandRange
            with get () : Range = this.randomRange
            and  set (value:Range) = this.randomRange <- value






