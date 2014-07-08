namespace TwitterWcfWorkerRole
open Twitterizer

type TwitterOAuth() =
    static let mutable tokens = new OAuthTokens()

    static member InitializeTokens() =
        // Add your own twitter application key & secret key
        // please go to https://dev.twitter.com/apps 
        if not tokens.HasBothTokens then
            tokens.set_ConsumerKey("REMOVED")
            tokens.set_ConsumerSecret("REMOVED")
            tokens.set_AccessToken("REMOVED")
            tokens.set_AccessTokenSecret("REMOVED")

    static member Tokens with get() = 
                            if not tokens.HasBothTokens then
                                TwitterOAuth.InitializeTokens()
                            tokens