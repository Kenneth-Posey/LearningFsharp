namespace FSharpMvc.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations

/// model representing login info
[<CLIMutable>]
type LoginModel = 
    {
        [<Required>]
        [<Display(Name = "Username")>]
        UserName : string

        [<Required>]
        Password : string

        [<Display(Name = "Remember me")>]
        RememberMe : bool
    }

/// model repreenting registration info
[<CLIMutable>]
type RegisterModel = 
    {
        [<Required>]
        [<Display(Name = "Username")>]
        UserName : string

        [<Required>]
        Email : string

        [<Required>]
        Password : string

        [<Required>]
        [<Display(Name = "Confirm password")>]
        PasswordConfirmatiom : string
    }

/// movie info
[<CLIMutable>]
type MovieData = 
    {
        [<Required>]
        Id : int

        [<Required>]
        NetflixId : string

        [<Required>]
        Title : string

        [<Required>]
        LargeUrl : string

        [<Required; System.Web.Mvc.AllowHtml>]
        Synopsis : string

        [<Required>]
        NetflixUrl : string
    }

/// recommendation info
type Recommendation = 
    {
        Movie : MovieData
        Rating : float
    }

/// genre recommendation info
type RecommendationsForGenre = 
    {
        Genre : string
        Recommendations : Recommendation[]
    }

/// genre info
[<CLIMutable>]
type GenreData =
    {
        Id : int

        [<Required>]
        Name : string
    }

/// user info
[<CLIMutable>]
type UserData =
    {
        Id : string

        [<Required>]
        UserName : string
    }

/// model representing page views
[<CLIMutable>]
type MyPageViewModel =
    {
        Movies : MovieData[]
        Genres : GenreData[]
    }