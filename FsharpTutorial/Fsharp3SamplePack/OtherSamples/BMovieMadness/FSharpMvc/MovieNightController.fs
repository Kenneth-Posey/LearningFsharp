namespace FSharpMvc.Controllers

open System.Web.Mvc
open System.Web.Security
open FSharpMvc.Models
open FSharpMvc.Utils
open Microsoft.FSharp.Linq.NullableOperators

/// controller to handle actions related to movies
[<Authorize>]
type MovieNightController() = 
    inherit Controller()

    let makeDbAccess() = new Db.DbAccess()

    /// maps from the provided type to our model type
    let mapMovie (m : Netflix.Netflix.ServiceTypes.Title) = 
        {
            Id = 0
            NetflixId = m.Id
            Title = m.Name
            LargeUrl = m.BoxArt.LargeUrl
            Synopsis = m.Synopsis
            NetflixUrl = m.Url
        }

    /// wraps data access calls with error handling and reporting
    member private this.Do f : ActionResult = 
        let result = 
            if not this.ModelState.IsValid then
                let errors = 
                    [|
                        for ms in this.ModelState.Values do
                        for err in ms.Errors do
                            yield err.ErrorMessage
                    |]
                DataOrError<_>.Errors(errors)
            else
                let userId = this.User.GetProviderUserKeyAsGuid()
                use db = makeDbAccess()
                f(db, userId)
        this.Json( result ) :> _

    /// initializes and loads all users besides the current user
    member this.New() = 
        let userId = this.User.GetProviderUserKeyAsGuid()
        use db = makeDbAccess()

        let users = db.LoadUsers(userId)
        let users = [| for u in users -> { Id = string u.UserId; UserName = u.UserName } |]

        this.View(users)

    /// handles user requesting movie recommendataions for specified users
    [<HttpPost>]
    member this.RecommendMovies(participants : ResizeArray<UserData>) = 
        
        let userIds = 
            [| 
                if participants <> null then
                    for p in participants -> System.Guid.Parse p.Id
                yield this.User.GetProviderUserKeyAsGuid() // include self
            |]

        let recommender = new Recommender()
        let recommendations = recommender.RecommendMovies(userIds)

        // wrap up results in model array
        let movies = 
            [|
                for (g, movies) in recommendations ->
                    let recommendations =
                        [|
                            for m in movies ->
                              {
                                  Movie = mapMovie m
                                  Rating = if m.AverageRating.HasValue then m.AverageRating.Value else 0.0
                              }
                       |]
                   
                    { Genre = g; Recommendations = recommendations }
            |]

        this.Json(movies)

    /// handles user requesting their personal page
    member this.My() =
        let userId = this.User.GetProviderUserKeyAsGuid()
        use db = makeDbAccess()
        let userMovies = db.LoadMovies(userId)
        let userGenres = db.LoadGenres(userId)

        // map from DB types to model types

        let userMovies = 
            userMovies
            |> Array.map (fun m ->
                { 
                    Id = m.MovieId
                    Title = m.Title
                    NetflixId = m.NetflixId
                    LargeUrl = m.LargeUrl
                    Synopsis = m.Synopsis
                    NetflixUrl = m.Url
                } 
            )

        let userGenres = 
            userGenres 
            |> Array.map (fun g ->
                    { 
                        Id = g.UserGenresId
                        Name = g.Genres.Name
                    }  
            )

        this.View({ Movies = userMovies; Genres = userGenres })   

    /// handles user adding new movies to their "already seen" list
    [<HttpPost>]
    member this.AddMovie(model : MovieData) : ActionResult = 
        this.Do <| fun(dal, userId) ->
            match dal.AddMovie(userId, model) with
            | Choice1Of2 newModel -> DataOrError<_>.Data( newModel )
            | Choice2Of2 error -> DataOrError<_>.Error(error)
            
    /// handles user adding new genres to their preferred genre list
    [<HttpPost>]
    member this.AddGenre(model : GenreData) : ActionResult = 
        this.Do <| fun(dal, userId) ->
            match dal.AddGenre(userId, model) with
            | Choice1Of2 newModel -> DataOrError<_>.Data( newModel )
            | Choice2Of2 error -> DataOrError<_>.Error(error)

    /// handles user removing a movie from their "already seen" list
    [<HttpPost>]
    member this.DeleteMovie(movieId : int) = 
        this.Do <| fun(dal, userId) ->
            dal.DeleteMovie(userId, movieId)
            DataOrError<_>.Data(null)

    /// handles user removing a movie from their preferred genre list
    [<HttpPost>]
    member this.DeleteGenre(userGenreId : int) = 
        this.Do <| fun (dal, user) ->
            dal.DeleteGenre(user, userGenreId)
            DataOrError<_>.Data(null)

    /// handles autocompletion on the genre text box
    member this.SuggestGenre(term : string) = 
        use db = makeDbAccess()
        let genres = db.LoadGenreSuggestions(term)
        let genres =
            [|
                for (id, name) in genres ->
                    {
                        label = name
                        value = name
                        data = { Id = id; Name = name }
                    }
            |]
        this.Json(genres, JsonRequestBehavior.AllowGet)

    /// handles autocompletion on the movie text box
    member this.SuggestMovie(term : string) = 
        let ctx = Netflix.NetflixAccess()
        let q = ctx.LoadMovieSuggestions(term)
        let suggestions = 
            [| for o in q -> LabeledValue<_>.New o.Name (mapMovie o)|]
        this.Json(suggestions, JsonRequestBehavior.AllowGet)