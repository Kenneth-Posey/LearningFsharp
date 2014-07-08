namespace FSharpMvc.Utils

open System
open System.Configuration
open System.Linq
open FSharpMvc.Models
open Microsoft.FSharp.Linq.NullableOperators
open Microsoft.FSharp.Data.TypeProviders

// encapsulates access to the user data DB
module internal Db = 

    /// access the local SQL DB via the SQL Entity type provider
    type internal T = SqlEntityConnection<ConnectionString = @"Data Source=(LocalDb)\v11.0;Initial Catalog=bmovie-data;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\..\CSharpMvcApp\App_Data\bmovie-data.mdf", ForceUpdate=false>

    /// DB data access layer
    type internal DbAccess() = 

        let db = T.GetDataContext(ConfigurationManager.ConnectionStrings.["DefaultConnection"].ConnectionString)

        /// loads all movies a particular user indicated she has already seen
        member this.LoadMovies(userId) = 
            let q = query {
                for m in db.Users.Single(fun (u : T.ServiceTypes.Users) -> u.UserId = userId).Movies do
                sortBy m.MovieId
                select m
            } 
            Seq.toArray q
        
        /// loads all movies a particular user indicated she prefers
        member this.LoadGenres(userId) = 
            let q = query {
                for g in db.Users.Single(fun (u : T.ServiceTypes.Users) -> u.UserId = userId).UserGenres do
                sortBy g.GenreId
                select g
            }
            Seq.toArray q

        /// gets user info for a particular user
        member this.LoadUser(userId) = 
            db.Users.Single(fun (u : T.ServiceTypes.Users) -> u.UserId = userId)
        
        /// gets all users besides the specified user
        member this.LoadUsers(userId) =
            let users = query {
                for u in db.Users do
                where (u.UserId <> userId)
                select u
                }
            Seq.toArray users

        /// queries the DB for genre autocompletion candidates
        member this.LoadGenreSuggestions(prefix) = 
            let genres = query {
                for g in db.Genres do
                where (g.Name.StartsWith prefix)
                select (g.GenreId, g.Name)
                }
            Seq.toArray genres
        
        /// loads all movies marked as seen by the specified users
        member this.LoadMovieIdsForUsers(userIds : Guid[]) = 
            let q = query {
                for u in db.Users do
                where (userIds.Contains u.UserId)
                for m in u.Movies do
                select m.NetflixId
                }
            Seq.toArray q

        /// queries the DB for genres marked as favorites by the most users
        member this.LoadTopGenres(userIds : Guid[], top) = 
            let genres = 
                db.Users
                    .Where(fun (u : T.ServiceTypes.Users) -> userIds.Contains u.UserId)
                    .SelectMany(fun (u : T.ServiceTypes.Users) -> u.UserGenres :> seq<_>)
                    .GroupBy(fun (g : T.ServiceTypes.UserGenres) -> g.Genres.Name)
                    .OrderByDescending(fun (g  : IGrouping<_, _>) -> g.Count())
                    .Select(fun (g  : IGrouping<_, _>) -> g.Key)
                    .Take(top)
            Seq.toArray genres

        /// marks a movie as one a user has already seen
        member this.AddMovie(userId, movie : MovieData) = 
            let user = this.LoadUser(userId)

            if user.Movies.Any(fun m -> m.NetflixId = movie.NetflixId) then
                Choice2Of2 (sprintf "Movie '%s' was already added" movie.Title)
            else
                let newMovie = T.ServiceTypes.Movies(Title = movie.Title, NetflixId = movie.NetflixId, LargeUrl = movie.LargeUrl, Synopsis = movie.Synopsis, Url = movie.NetflixUrl)
                user.Movies.Add(newMovie)
                let _ = db.DataContext.SaveChanges()
        
                Choice1Of2 { movie with Id = newMovie.MovieId }

        /// marks a genre as a favorite of a user
        member this.AddGenre(userId, genre : GenreData) = 
            let user = this.LoadUser(userId)

            if user.UserGenres.Any(fun m -> m.Genres.Name = genre.Name) then
                Choice2Of2 (sprintf "Genre '%s' was already added" genre.Name)
            else
                let link = new T.ServiceTypes.UserGenres(UserId = user.UserId, GenreId = genre.Id)
                user.UserGenres.Add(link)
                let _ = db.DataContext.SaveChanges()
                Choice1Of2 { genre with Id = genre.Id}

        /// removes a movie from a user's "seen" list
        member this.DeleteMovie(userId, movieId) =
            let user = this.LoadUser(userId)
            let movie = user.Movies.Single(fun m -> m.MovieId = movieId)
            let _ = user.Movies.Remove(movie)
            db.DataContext.DeleteObject(movie)
            db.DataContext.SaveChanges()
            |> ignore

        /// removes a genre from a user's favorite list
        member this.DeleteGenre(userId, userGenreId) = 
            let user = this.LoadUser(userId)
            let link = user.UserGenres.Single(fun g -> g.UserGenresId = userGenreId)
            let _ = user.UserGenres.Remove(link)
            db.DataContext.DeleteObject(link)
            db.DataContext.SaveChanges()
            |> ignore

        interface IDisposable with
            member this.Dispose() = db.DataContext.Dispose()