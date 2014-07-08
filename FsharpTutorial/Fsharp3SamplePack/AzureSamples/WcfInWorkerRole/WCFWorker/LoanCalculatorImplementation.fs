namespace WCFWorker
open System
open System.Collections.Generic
open System.Linq
open System.Text
open LoanCalculatorContracts
open System.ServiceModel
open System.Runtime.Serialization

[<ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)>]
type public LoanCalculatorImplementation() =

    member this.Calculate(loan : LoanInformation) = (this :> ILoanCadulator).Calculate loan
    member this.Calculate2(a:int,b:string) = (this :> ILoanCadulator).Calculate2(a,b)
    member this.Calculate3(b:Suit) = (this :> ILoanCadulator).Calculate3(b)

    interface ILoanCadulator with
        override this.Calculate(loan : LoanInformation) =
            let monthlyInterest = Math.Pow((1.0 + loan.InterestRateInPercent / 100.0), 1.0 / 12.0) - 1.0
            let num = loan.Amount * monthlyInterest
            let den = 1.0 - (1.0/(Math.Pow(1.0 + monthlyInterest, (double)loan.TermInMonth)))
            let monthlyPayment = num / den

            let totalPayment = monthlyPayment * (double)loan.TermInMonth
            let paymentInformation  = {MonthlyPayment = monthlyPayment;TotalPayment = totalPayment}

            paymentInformation

        override this.Calculate2 (a:int * string) =
            let (ac,bc) = a
            let ax = ac * 5
            let bx = bc + " multiplied by 5"
            (ax,bx)

        override this.Calculate3(a : Suit) = 
            let b = Suit.Diamond
            b