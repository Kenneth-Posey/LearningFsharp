namespace Neuro

module ActivationFunction =
    open System
    open MathAlgorithms.Generator
    open MathAlgorithms.MathStructures

    type IActivationFunction = 
        abstract member Function    : double -> double
        abstract member DerivativeX : double -> double
        abstract member DerivativeY : double -> double

    type IStochasticFunction = 
        inherit IActivationFunction
        abstract member GenerateX : double -> double
        abstract member GenerateY : double -> double

    type IActivationFunctionWithCloneable =
        inherit IActivationFunction
        inherit ICloneable

    type ThresholdFunction () =
        interface IActivationFunctionWithCloneable with
            member this.Function (x:double) =
                match ( x >= 0.0 ) with
                | true  -> 1.0
                | false -> 0.0

            // Irrelevant methods for this function type
            member this.DerivativeX (x:double) = 0.0
            member this.DerivativeY (y:double) = 0.0

            // Clone() requires return type of object
            member this.Clone () = 
                upcast new ThresholdFunction()
        

    type BipolarSigmoidFunction (?alpha:double) as this = 
        do 
            this.alpha <- defaultArg alpha 2.0        
        let Alpha = this.alpha

        member this.alpha
            with get () : double = this.alpha
            and  set (value:double) = this.alpha <- value

        interface IActivationFunctionWithCloneable with
            member this.Function (x:double) =
                ( 2.0 / ( 1.0 + Math.Exp( Alpha * x * -1.0) ) - 1.0 )
                
            member this.DerivativeX (x:double) = 
                let this = (this :> IActivationFunctionWithCloneable)
                this.DerivativeY ( this.Function x )

            member this.DerivativeY (y:double) = 
                ( Alpha * ( 1.0 - y * y ) / 2.0 )

            // Clone() requires return type of object
            member this.Clone () =
                upcast new BipolarSigmoidFunction( Alpha )

    type SigmoidFunction (?alpha:double) as this = 
        do
            this.alpha <- defaultArg alpha 2.0        
        let Alpha = this.alpha
        
        member this.alpha
            with get () : double = this.alpha
            and  set (value:double) = this.alpha <- value

        interface IActivationFunctionWithCloneable with
            member this.Function (x:double) = 
                ( 1.0 / ( 1.0 + Math.Exp ( -1.0 * Alpha * x ) ) )

            member this.DerivativeX (x:double) = 
                let this = (this :> IActivationFunctionWithCloneable)
                this.DerivativeY ( this.Function x )

            member this.DerivativeY (y:double) = 
                ( Alpha * y * ( 1.0 - y ) )
                
            // Clone() requires return type of object
            member this.Clone () = 
                upcast new SigmoidFunction( Alpha )

    type GaussianFunction (?alpha:double, ?range:DoubleRange) as this = 
        do
            this.alpha <- defaultArg alpha 1.0
            this.range <- defaultArg range (new DoubleRange(-1.0, 1.0))
        let tAlpha = this.alpha
        
        member val private alpha = 1.0 with get, set
        member val Alpha = this.alpha with get

        member val private range = new DoubleRange(-1.0, 1.0) with get, set
        member val Range = this.range with get

        member val Random = new StandardGenerator(Environment.TickCount) with get
        
        // This upcasting pattern exposes the interface member publically
        member this.Function x    = (this :> IStochasticFunction).Function x
        member this.DerivativeX x = (this :> IStochasticFunction).DerivativeX x
        member this.DerivativeY y = (this :> IStochasticFunction).DerivativeY y
        member this.GenerateX x   = (this :> IStochasticFunction).GenerateX x
        member this.GenerateY y   = (this :> IStochasticFunction).GenerateY y

        member private this.chooseY (y:double) = 
            if y > this.range.Max then
                this.range.Max
            else if y < this.range.Min then
                this.range.Min
            else
                y

        interface IStochasticFunction with
            member this.Function x =
                this.chooseY (tAlpha * x)

            member this.DerivativeX x =
                this.DerivativeY (tAlpha * x)

            member this.DerivativeY y =
                if (y <= this.range.Min) || (y >= this.range.Max) then 0.0
                else tAlpha

            member this.GenerateX x =
                let y = tAlpha * x + this.Random.NextDouble()
                this.chooseY y

            member this.GenerateY y =
                let y = y + this.Random.NextDouble()
                this.chooseY y


module Neuron = 
    open System
    open ActivationFunction
    open MathAlgorithms.Threadsafe
    open MathAlgorithms.MathStructures
    
    [<AbstractClass>]
    type NeuronBase (inputs:int) as this = 
        do
            this.inputsCount <- Math.Max (1 , inputs)
            this.Randomize ()

        // This is a function that's run only for its side effects 
        // ie: very much not functional style, will fix later
        abstract member Randomize : unit -> unit
        default this.Randomize () = 
            let length = double this.randomRange.Length
            for x in 1 .. this.inputsCount - 1 do
                this.weights.[x] <- (this.rand.NextDouble() * length) + double this.randomRange.Min
        
        // Implemented in inherited classes
        abstract member Compute : double[] -> double

        member val private inputsCount:int = 0 with get, set
        member val InputsCount = this.inputsCount with get

        member val private weights:double[] = Array.zeroCreate<double> this.inputsCount with get, set
        member val Weights = this.weights with get

        member val private output:double = 0.0 with get, set
        member val Output = this.output with get

        member val private rand:RandomGenerator = new RandomGenerator() with get, set
        member val RandGenerator = this.rand with get

        member val private randomRange:SingleRange = new SingleRange(0.0f, 1.0f) with get,set
        member val RandRange = this.randomRange with get


    type ActivationNeuron ( inputs:int , activation:IActivationFunction ) as this =   
        inherit NeuronBase (inputs) 
            do 
                this.ActivationFunction <- activation
            
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

            override this.Randomize () = 
                base.Randomize ()
                this.Threshold <- this.RandGenerator.NextDouble() * double this.RandRange.Length + double this.RandRange.Min

            override this.Compute (x:double[]) = 
                let mutable sum = 0.0
                for i in 1 .. this.Weights.Length do
                    sum <- sum + (this.Weights.[i] * x.[i])

                this.ActivationFunction.Function ( sum + this.Threshold )

                
        
            





                  
        