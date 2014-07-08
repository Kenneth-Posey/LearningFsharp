namespace FSharpMvc.Controllers

open System.Web.Mvc
open System.Web.Security
open FSharpMvc.Models

/// controller to handle actions related to user accounts
type AccountController() = 
    inherit Controller()
    
    member private this.RedirectToHome() : ActionResult = 
        this.RedirectToAction("Index", "Home") :> _
    
    /// handles user visiting the login page  
    member this.Login() = this.View()

    /// handles user logging in with username and password
    [<HttpPost>]
    member this.Login(model : LoginModel) : ActionResult =
        if this.ModelState.IsValid then
            if Membership.ValidateUser(model.UserName, model.Password) then
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe)
                this.RedirectToHome()
            else
                this.ModelState.AddModelError("", "The user name or password provided is incorrect.")
                this.View(model) :> _
        else
            this.View(model) :> _
    
    /// handles user logoff
    member this.Logoff() : ActionResult = 
        FormsAuthentication.SignOut()
        this.RedirectToHome()
    
    /// handles user visiting registration page
    member this.Register() = 
        this.View()

    /// handles new user registration
    [<HttpPost>]
    member this.Register(model : RegisterModel) =
        if this.ModelState.IsValid then
            let mutable status = Unchecked.defaultof<_>
            let user = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, &status)
            if status = MembershipCreateStatus.Success then
                FormsAuthentication.SetAuthCookie(model.UserName, false)
                this.RedirectToHome()
            else
                this.ModelState.AddModelError("", (string status))
                this.View(model) :> _
        else
            this.View(model) :> _