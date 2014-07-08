
namespace TwitterWcfWorkerRole

open Twitterizer
open TwitterContracts
open System.Collections.Generic

type TwitterStatusImplementation() =
    
    let GetStatusList(collection : TwitterStatusCollection) =
        let iEnum = collection.GetEnumerator()
        let twitterlist = new List<TwitterContracts.TwitterStatus>()
        while iEnum.MoveNext() do
            twitterlist.Add <| {Name=iEnum.Current.User.Name
                                Location = iEnum.Current.User.Location
                                Description = iEnum.Current.User.Description
                                LinkifiedText = iEnum.Current.LinkifiedText()
                                CreatedDate = iEnum.Current.CreatedDate
                                ScreenName = iEnum.Current.User.ScreenName
                                ProfileImageLocation = iEnum.Current.User.ProfileImageLocation }
        twitterlist
            
    /// Get the user home time line
    member this.GetMyHomeTimeLine() = (this :> ITwitterStatusContracts).GetMyHomeTimeLine()
    /// Get the default home time line
    member this.GetHomeTimeLine() = (this :> ITwitterStatusContracts).GetHomeTimeLine()
    /// Get the special user home time line
    member this.GetUserHomeTimeLine(screenname : string) = (this :> ITwitterStatusContracts).GetUserHomeTimeLine(screenname)
    /// Add new tweet 
    member this.TweetUpdate(text : string) = (this :> ITwitterStatusContracts).TweetUpdate(text)
    /// search the twitter key words
    member this.TwitterSearch(key:string) = (this :> ITwitterStatusContracts).TwitterSearch(key)

    interface ITwitterStatusContracts with

        /// Get the user home time line
        override this.GetMyHomeTimeLine() =
            let homeTimelineRequest = TwitterTimeline.HomeTimeline(TwitterOAuth.Tokens)
            if homeTimelineRequest.Result = RequestResult.Success then
                GetStatusList homeTimelineRequest.ResponseObject
            else 
                null

        /// Get the default home time line
        override this.GetHomeTimeLine() = 
            let options = new OptionalProperties()
            options.CacheOutput <- true
            options.CacheTimespan <- new System.TimeSpan(0, 1, 0)
            options.UseSSL <- true

            let publicTimelineRequest = TwitterTimeline.PublicTimeline(options)

            if publicTimelineRequest.Result = RequestResult.Success then
                GetStatusList publicTimelineRequest.ResponseObject
            else
                null

        /// Get the special user home time line
        override this.GetUserHomeTimeLine(screenname : string) =
            let usertimelineOption = new UserTimelineOptions()
            usertimelineOption.ScreenName <- screenname
            let userTimelineResponse = TwitterTimeline.UserTimeline(TwitterOAuth.Tokens,usertimelineOption)

            if userTimelineResponse.Result = RequestResult.Success then
                GetStatusList userTimelineResponse.ResponseObject
            else
                null

        /// Add new tweet 
        override this.TweetUpdate(text : string) =
            let tweetUpdateRequest = TwitterStatus.Update(TwitterOAuth.Tokens, text)

            if tweetUpdateRequest.Result = RequestResult.Success then
                let item = tweetUpdateRequest.ResponseObject

                {   Name=item.User.Name
                    Location = item.User.Location
                    Description = item.User.Description
                    LinkifiedText = item.LinkifiedText()
                    CreatedDate = item.CreatedDate
                    ScreenName = item.User.ScreenName
                    ProfileImageLocation = item.User.ProfileImageLocation }

            else
                {Name="";Location="";Description="";LinkifiedText="";CreatedDate=System.DateTime.Now;ScreenName="";ProfileImageLocation=""}

        /// search the twitter key words
        override this.TwitterSearch(key:string) =
            let searchResponse = TwitterSearch.Search(key)
            if  searchResponse.Result = RequestResult.Success then
                let collection = searchResponse.ResponseObject
                let iEnum = collection.GetEnumerator()
                let twitterlist = new List<TwitterContracts.TwitterSearchResult>()
                while iEnum.MoveNext() do
                    twitterlist.Add <| {Text=iEnum.Current.Text
                                        CreatedDate = iEnum.Current.CreatedDate
                                        FromUserScreenName = iEnum.Current.FromUserScreenName
                                        ProfileImageLocation = iEnum.Current.ProfileImageLocation }
                twitterlist
            else
                null
