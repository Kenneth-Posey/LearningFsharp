﻿namespace LoanCalculatorContracts
open System
open System.Collections.Generic

open System.ServiceModel
open System.Runtime.Serialization
/// Record for LoanInformation
[<DataContract>]
type LoanInformation =
    { [<DataMember>] mutable Amount : double
 
      [<DataMember>] mutable InterestRateInPercent : double 
      [<DataMember>] mutable TermInMonth : int }

/// Record for PaymentInformation
[<DataContract>]
type PaymentInformation =
    { [<DataMember>] mutable MonthlyPayment : double
 
      [<DataMember>] mutable TotalPayment : double }

//[<DataContract>] --> Never add this attribute on Discriminated Union, it won't work
/// Discriminated Union
type Suit =
    | Heart
    | Diamond
    | Spade
    | Club

/// For F#, all the interface method must be given a parameter name: eg. a:LoanInformation
/// otherwise, you will fail to start the service host
[<ServiceContract>]
type public ILoanCadulator =
    /// Use Record to send and recieve data
    [<OperationContract>]
    abstract Calculate : a:LoanInformation -> PaymentInformation
    /// Use Tuple to send and recieve data
    [<OperationContract>]
    abstract Calculate2 : b:(int * string) -> int * string
    /// Use Discriminated Union(DU) to send and recieve data
    [<OperationContract>]
    abstract Calculate3 : c: Suit -> Suit