namespace FSharpMvc.Utils

/// assists with recommending a set of movies based on user preferences
type Recommender() =

    /// returns recommended movies for the specified users, based on those users' preferences
    member this.RecommendMovies(userIds) : (string * Netflix.Netflix.ServiceTypes.Title[])[]= 
        let NumberOfTopGenres = 2
        let NumberOfRecommendationsForGenre = 40     
        let TruncatedTotal = 8
        
        use db = new Db.DbAccess()
        let topGenres = db.LoadTopGenres(userIds, NumberOfTopGenres)

        if topGenres.Length = 0 then [||]
        else

        let allMovies = 
            db.LoadMovieIdsForUsers(userIds)
            |> fun s -> System.Collections.Generic.HashSet(s)

        let result = ResizeArray()
        let netflix = new Netflix.NetflixAccess()

        // finds movie recommendations for a particular genres
        //   which are not found in any user's "already seen" list
        let loadRecommendationsForGenre genre = 
            let mutable run = true
            let mutable skip = 0
            let total = ResizeArray()

            // keep querying for more movies in the genre until we collect
            //   NumerOfRecommendationsForGenre new movies which nobody has seen yet
            while run do
                let loaded = 
                    netflix.LoadMoviesForGenre(genre, NumberOfRecommendationsForGenre, skip)

                if loaded.Length = 0 then run <- false
                else

                let moviesThatWereNotSeenBefore = loaded |> Array.filter (fun m -> not (allMovies.Contains m.Id))

                // if this query doesn't meet the quota, save all that we have and iterate
                if moviesThatWereNotSeenBefore.Length + total.Count < NumberOfRecommendationsForGenre then
                    for m in moviesThatWereNotSeenBefore do allMovies.Add(m.Id) |> ignore
                    total.AddRange(moviesThatWereNotSeenBefore)

                    skip <- skip + NumberOfRecommendationsForGenre

                // otherwise, save as much as we need, and exit
                else
                    moviesThatWereNotSeenBefore 
                    |> Seq.take (NumberOfRecommendationsForGenre - total.Count)
                    |> total.AddRange

                    run <- false
            
            if total.Count > 0 then
                result.Add(genre, Utils.randomSubset(total.ToArray(), TruncatedTotal))
            
        for g in topGenres do
            loadRecommendationsForGenre g

        result.ToArray()