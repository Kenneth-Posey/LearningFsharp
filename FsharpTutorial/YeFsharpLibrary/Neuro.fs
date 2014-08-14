﻿namespace Neuro

module ActivationFunction =
    open System
    type IActivationFunction = 
        abstract member Function    : double -> double
        abstract member Derivative  : double -> double
        abstract member Derivative2 : double -> double

    type IActivationFunctionWithCloneable =
        inherit IActivationFunction
        inherit ICloneable

    type ThresholdFunction () =
        interface IActivationFunctionWithCloneable with
            member this.Function (x:double) =
                match ( x >= 0.0) with
                | true  -> 1.0
                | false -> 0.0

            // Irrelevant methods for this function
            member this.Derivative  (x:double) = 0.0
            member this.Derivative2 (x:double) = 0.0

            // Clone() requires return type of object
            member this.Clone () = 
                upcast new ThresholdFunction()
        

    type BipolarSigmoidFunction () as this = 
        do 
            this.alpha <- 2.0

        member this.alpha
            with get () : double = this.alpha
            and  set (value:double) = this.alpha <- value

module Neuron = 
    open System
    open ActivationFunction
    open MathAlgorithms.Threadsafe
    open MathAlgorithms.MathStructures

    type ActivationNeuron () as this =
        do 
            this.threshold <- 0.0

        new (alpha:double) = ActivationNeuron (alpha)

        member private this.threshold
            with get () : double = this.threshold
            and  set (value:double) = this.threshold <- value

        member this.Threshold
            with get () = this.threshold
            and  set value = this.threshold <- value

        member private this.activationFunction
            with get () : IActivationFunction = this.activationFunction
            and  set (value:IActivationFunction) = this.activationFunction <- value

        member this.ActivationFunction
            with get () = this.activationFunction
            and  set value = this.activationFunction <- value

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





