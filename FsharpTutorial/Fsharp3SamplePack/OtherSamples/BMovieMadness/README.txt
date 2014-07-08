Relnotes for B-Movie Madness

- Building this code from inside of CodePlex source control is not recommended, as many files will be locked as read-only.
  It is recommended that you download the F# Sample Pack code from CodePlex as a ZIP, instead.  There is a download link under the "Source Code" tab on the F# 3.0 Sample Pack CodePlex page.
  
- Before your first build, you will need to download some required NuGet packages.

  1. Open the NuGet Package Manager console (Tools -> Library Package Manager -> Package Manager Console)
  2. Set the "Package source" to "NuGet official package source"
  3. Set the "Default project" to "CSharpMvcApp"
  4. Paste the following into the console to download the packages:

        Install-Package -Id "EntityFramework" -Version 5.0.0
        Install-Package -Id "jQuery" -Version 1.8.2
        Install-Package -Id "jQuery.UI.Combined" -Version 1.9.0
        Install-Package -Id "jQuery.Validation" -Version 1.10.0
        Install-Package -Id "knockoutjs" -Version 2.1.0
        Install-Package -Id "Microsoft.AspNet.Mvc" -Version 4.0.20710.0
        Install-Package -Id "Microsoft.AspNet.Providers.Core" -Version 1.2
        Install-Package -Id "Microsoft.AspNet.Providers.LocalDB" -Version 1.1
        Install-Package -Id "Microsoft.AspNet.Razor" -Version 2.0.20710.0
        Install-Package -Id "Microsoft.AspNet.Web.Optimization" -Version 1.0.0
        Install-Package -Id "Microsoft.AspNet.WebApi" -Version 4.0.20710.0
        Install-Package -Id "Microsoft.AspNet.WebApi.Client" -Version 4.0.20710.0
        Install-Package -Id "Microsoft.AspNet.WebApi.Core" -Version 4.0.20710.0
        Install-Package -Id "Microsoft.AspNet.WebApi.WebHost" -Version 4.0.20710.0
        Install-Package -Id "Microsoft.AspNet.WebPages" -Version 2.0.20710.0
        Install-Package -Id "Microsoft.jQuery.Unobtrusive.Ajax" -Version 2.0.20710.0
        Install-Package -Id "Microsoft.jQuery.Unobtrusive.Validation" -Version 2.0.20710.0
        Install-Package -Id "Microsoft.Net.Http" -Version 2.0.20710.0
        Install-Package -Id "Microsoft.Web.Infrastructure" -Version 1.0.0.0
        Install-Package -Id "Modernizr" -Version 2.6.2
        Install-Package -Id "Newtonsoft.Json" -Version 4.5.10
        Install-Package -Id "WebGrease" -Version 1.1.0
    
    The basic functionality of the site has been tested using these versions of the various web packages.  Earlier or later versions of these frameworks might cause the site to work improperly.
    
- If you have never built F# code referencing the assembly FSharp.Data.TypeProviders.dll, the first build will trigger the type provider security dialog to open, which casues the build to fail.  After dismissing the dialog, subsequent builds will succeed.

- The site loads and operates properly in IE 9+ (Compatibility View off) and in the latest releases of Chrome and Firefox.

- The site is simple example of an ASP.NET MVC application.  For more information about creating MVC apps on ASP.NET, check out the content at http://www.asp.net/mvc

- You can create new users or modify the preferences of the F# team users which have already been created.  The login information for the existing users is below.

    User       Password
    ----       --------
    admin      111111  (To view the final movie suggestions page, you must be logged in as admin.  A "Get a movie recommendation" link will appear at the top of the page.)
    Don        111111
    Jack       111111
    Brian      111111
    Wonseok    111111
    Tao        111111
    Vlad       111111
    Lincoln    1111111
    Donna      111111