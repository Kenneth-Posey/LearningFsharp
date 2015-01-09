namespace EveOnline

module MarketParser =
    open EveOnline
    open EveOnline.Market
    open EveOnline.Market.Types
    open Utility.UtilityFunctions
            
    /// For reading and parsing the text file with typeIDs
    // Example use:
    // let itemArray = MarketParser.LoadTypeIdsFromUrl EveData.TypeIdUrl
    // let tritItems  = itemArray 
    //                  |> FilterToOreOnly 
    //                  |> FilterByName "Tritanium"
