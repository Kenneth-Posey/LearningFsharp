namespace FSharpMvc.Controllers

open System.Web.Mvc

/// controller to handle actions related to the home page
type HomeController() = 
    inherit Controller()

    /// handles user requests for the home page
    member this.Index() = 
       if this.Request.IsAuthenticated then
           this.RedirectToAction("My", "MovieNight")
       else
           this.RedirectToAction("Login", "Account")