namespace FSharpMvc.Utils

open System.Linq
open Microsoft.FSharp.Linq.NullableOperators
open Microsoft.FSharp.Data.TypeProviders

// encapsulates access to the Netflix catalog
module Netflix = 
    
    /// access the Netflix catalog via the ODATA type provider
    type Netflix = ODataService<"http://odata.netflix.com/Catalog/">
    let ctx = Netflix.GetDataContext()

    /// Netflix data access layer
    type NetflixAccess() = 

        /// queries Netflix for a batch of old movies, in ascending order by rating
        member this.LoadMoviesForGenre(genre, nToTake, nToSkip) =

            query {
                for g in ctx.Genres do
                where (g.Name = genre)
                for t in g.Titles do
                where (t.ReleaseYear ?<= 1959)
                where (t.ReleaseYear ?>= 1934)
                sortByNullable t.AverageRating
                skip nToSkip
                take nToTake
                select t
            } |> Seq.toArray

        /// queries Netflix for movie autocompletion candidates
        member this.LoadMovieSuggestions(prefix) =
            query {
                for t in ctx.Titles do
                where (t.AverageRating ?> 0.0) // anything rated
                where (t.Name.StartsWith prefix) // that starts with term
                sortByNullable (t.AverageRating) // show worst first
                take 10
            } |> Seq.toArray