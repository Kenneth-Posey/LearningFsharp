// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("TypeProviders")>]
module Samples.DataAccess.SQL

open System
open System.Collections.Generic
open System.Linq
open System.Data.Linq.SqlClient
open System.Net
open Support.Helper

[<Support("typeProviderODataSample")>]
let dummy9() = ()
type TOData = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/Uk.Gov/TravelAdvisoryService/", LocalSchemaFile="TravelAdvisory.csdl", ForceUpdate=false >

[<Category("DataAccess.ODataService");
  Title("TypeProvider.OData.Sample0");
  Description("TypeProvider.OData.Sample0")>]
let typeProviderODataSample() = 
    let travelInfo = TOData.GetDataContext()
    //To sign up for a Windows Azure Marketplace account @ https://datamarket.azure.com/account/info
    do travelInfo.Credentials <- new NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)  
    
    query {
        for n in travelInfo.BritishEmbassiesAndHighCommissions do 
        select n
        take 2
    }|> Seq.iter (fun n -> printfn "%s\n" n.Address)

    // sample output
    //    1A Brisbane Street
    //    Hobart
    //    Tasmania
    //    7000
    //    ,Hobart
    //
    //    444-446 Pulteney Street
    //    Adelaide
    //    SA 5000
    //    ,Adelaide

[<Support("typeProviderODataSample1")>]
let dummy10() = ()
type Northwind = Microsoft.FSharp.Data.TypeProviders.ODataService<"http://services.odata.org/Northwind/Northwind.svc/", LocalSchemaFile="Northwind.csdl", ForceUpdate=false>

[<Category("DataAccess.ODataService");
  Title("Northwind public OData Service");
  Description("Consume Northwind public OData Service published by http://www.odata.org")>]
let typeProviderODataSample1() = 
    let db = Northwind.GetDataContext()  
    
    query {
        for n in db.Customers do 
        select n
        take 2
    }|> Seq.iter (fun n -> printfn "%s\n" n.ContactName)

    // sample output
    //    Maria Anders
    //    Ana Trujillo


//[<Support("typeResxProviderTest")>]
//let dummy() = ()
//type T = Microsoft.FSharp.Data.TypeProviders.ResxFile< @"Support_Resource.resx" >
//
//[<Category("DataAccess.Resources");
//  Title("TypeProvider.Resources.Sample0");
//  Description("TypeProvider.Resources.Sample0")>]
//let typeResxProviderTest() = 
//    let string1 = T.Support_Resource.String1
//    let string2 = T.Support_Resource.String2
//    printfn "string1 from resource is %s" string1
//    printfn "string2 from resource is %s" string2


[<Support("typeDBMLProvider")>]
[<Support("typeDBMLProvider2")>]
let dummy1() = ()
type T1 = Microsoft.FSharp.Data.TypeProviders.DbmlFile< @".\Support.DataClasses.dbml" >
[<Category("DataAccess.DatabaseDesigner");
  Title("DBML Provider");
  Description("Use DBML provider for FSharpSample database.  You can download the SQL server 2008 Express version for free. Please refer to CreateFSharpSampleDatabase.sql or run Setup project to configure your database.")>]
let typeDBMLProvider() = 
    let db = new T1.DataClasses1DataContext("Data Source=localhost;Initial Catalog=FSharpSample;User ID=sa;Password=FSharpSample1234")    
    let q = query {
        for n in db.Students do
        select n.Name }
    q |> Seq.iter (fun n -> printfn "student name = %s" n)    


[<Category("DataAccess.DatabaseDesigner");
  Title("DBML Provider - Console.Out as output");
  Description("Use Console.Out to log the underlying query.  You can download the SQL server 2008 Express version for free. Please refer to CreateFSharpSampleDatabase.sql or run Setup project to configure your database.")>]
let typeDBMLProvider2() = 
    let db = new T1.DataClasses1DataContext("Data Source=localhost;Initial Catalog=FSharpSample;User ID=sa;Password=FSharpSample1234")    
    db.Log <- Console.Out
    let q = query {
        for n in db.Students do
        select n.Name }
    q |> Seq.iter (fun n -> printfn "student name = %s" n)


[<Support("EDMX Provider")>]
let dummy2() = ()
type internal T2 = Microsoft.FSharp.Data.TypeProviders.EdmxFile< @".\Support.Model.edmx" >

[<Category("DataAccess.EntityDesigner");
  Title("EDMX Provider");
  Description("define a EDMX type provider on the local SQL server database. You can download the SQL server 2008 Express version for free. Please refer to CreateFSharpSampleDatabase.sql or run Setup project to configure your database.")>]
let typeEdmxProvider() = 
    let db = new T2.FSharpSampleModel.Entities("metadata=res://*/Support.Model.csdl|res://*/Support.Model.ssdl|res://*/Support.Model.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=localhost;Initial Catalog=FSharpSample;User ID=sa;Password=FSharpSample1234\"")
    let q = query { 
            for n in db.Students do
            select n.Name }
    q |> Seq.iter (fun n -> printfn "%A" n)


[<Support("WsdlSample1")>]
let dummy3() = () 
type T3 = Microsoft.FSharp.Data.TypeProviders.WsdlService< "http://www.w3schools.com/webservices/tempconvert.asmx?WSDL" >

[<Category("DataAccess.WSDL");
  Title("WsdlService - w3cschools temperature conversion");
  Description("Use the WsdlService type provider to access a WSDL service to do temperature conversion from w3cschools web site.")>]
let WsdlSample1() = 
    let db = T3.GetTempConvertSoap()
    let a = db.CelsiusToFahrenheit("0")
    printfn "%s" a


[<Support("WsdlBingMap")>]
let dummy4() = () 
type GeocodeService = Microsoft.FSharp.Data.TypeProviders.WsdlService<ServiceUri = "http://dev.virtualearth.net/webservices/v1/geocodeservice/geocodeservice.svc?wsdl">
type RouteService = Microsoft.FSharp.Data.TypeProviders.WsdlService<ServiceUri = "http://dev.virtualearth.net/webservices/v1/routeservice/routeservice.svc?wsdl">

type GeoCommon = GeocodeService.ServiceTypes.dev.virtualearth.net.webservices.v1.common
type Geocode = GeocodeService.ServiceTypes.dev.virtualearth.net.webservices.v1.geocode
type RouteCommon = RouteService.ServiceTypes.dev.virtualearth.net.webservices.v1.common
type Route = RouteService.ServiceTypes.dev.virtualearth.net.webservices.v1.route

[<Category("DataAccess.WSDL");
  Title("WsdlService - Bing Map rounting service");
  Description("Using Bing Map API routing service to calculate the driving distance between two addresses. For more information, please goto http://www.microsoft.com/maps/developers/mapapps.aspx. Please make sure you setup the correct credential.")>]
let WsdlBingMap() = 
    let geoClient = GeocodeService.GetBasicHttpBinding_IGeocodeService()
    let geoReq = new Geocode.GeocodeRequest()
    //To sign up for a Bing service developer account @ https://www.bingmapsportal.com/application/index/1034110
    geoReq.Credentials <- GeoCommon.Credentials(ApplicationId = Utils.BING_APP_ID)

    geoReq.Query <- "One Microsoft Way, Redmond, WA 98052"
    let response = geoClient.Geocode(geoReq)
    let work = response.Results.[0]

    geoReq.Query <- "400 Broad St Seattle, WA 98109"
    let response2 = geoClient.Geocode(geoReq)
    let home = response2.Results.[0]
    
    let startPoint = Route.Waypoint()
    startPoint.Location <- RouteCommon.Location(Latitude = work.Locations.[0].Latitude,
                                                Longitude = work.Locations.[0].Longitude)
    let endPoint = Route.Waypoint()
    endPoint.Location <- RouteCommon.Location(Latitude = home.Locations.[0].Latitude,
                                              Longitude = home.Locations.[0].Longitude)

    let routReq1 = Route.RouteRequest(Waypoints = [|startPoint; endPoint|])
    routReq1.Credentials <- RouteCommon.Credentials(ApplicationId = Utils.BING_APP_ID)
    
    printfn "Driving Distance = %A (miles)" (RouteService.GetBasicHttpBinding_IRouteService().CalculateRoute(routReq1).Result.Summary.Distance)

    // sample output
    //    Driving Distance = 22.998 (miles)
