// Learn more about F# at http://fsharp.net

namespace TwitterContracts

open System
open System.ServiceModel
open System.Runtime.Serialization
open System.Collections.Generic

[<DataContract>]
type TwitterStatus =
    { [<DataMember>] mutable Name : string
      [<DataMember>] mutable Location : string
      [<DataMember>] mutable Description : string
      [<DataMember>] mutable LinkifiedText : string
      [<DataMember>] mutable CreatedDate : DateTime
      [<DataMember>] mutable ScreenName : string 
      [<DataMember>] mutable ProfileImageLocation : string }

[<DataContract>]
type TwitterSearchResult = 
           {  [<DataMember>] mutable Text : string
              [<DataMember>] mutable CreatedDate : DateTime
              [<DataMember>] mutable FromUserScreenName : string 
              [<DataMember>] mutable ProfileImageLocation : string }

[<ServiceContract>]
type public ITwitterStatusContracts =
    /// Get the user home time line
    [<OperationContract>]
    abstract GetMyHomeTimeLine : unit -> List<TwitterStatus>
    /// Get the default home time line
    [<OperationContract>]
    abstract GetHomeTimeLine : unit ->  List<TwitterStatus>
    /// Get the special user home time line
    [<OperationContract>]
    abstract GetUserHomeTimeLine : screenname : string ->  List<TwitterStatus>
    /// Add new tweet 
    [<OperationContract>]
    abstract TweetUpdate : text : string -> TwitterStatus
    /// search the twitter key words
    [<OperationContract>]
    abstract TwitterSearch : key :string -> List<TwitterSearchResult>