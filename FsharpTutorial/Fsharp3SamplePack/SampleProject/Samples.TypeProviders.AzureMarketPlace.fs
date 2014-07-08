// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("TypeProviders.AzureMarketPlace")>]
module Samples.TypeProviders.AzureMarketPlace

open System.Data
open System.Data.Linq
open System.Net
open System.ServiceModel
open Support.Helper

[<Support("LawyersDotCom")>]
let dummy() = ()
type Lawyers = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/LexisNexis/ConsumerLegalArticles/" >

[<Category("TypeProviders.AzureMarketPlace");
  Title("Lawyers.com - Consumer Legal Articles");
  Description("Consume Azure Marketplace data from Lawyers.com Consumer Legal Articles. For more information, please goto https://datamarket.azure.com/dataset/4306e828-ad35-4049-b9a9-4e4824cc37e8")>]
let LawyersDotCom() = 
    let articles = Lawyers.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    articles.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for a in articles.Articles do
        where (a.State = "Washington")
        select a
    }|> Seq.iter(fun i -> printfn "%A" i.Title)

    // sample output
    //    "WA Witnesses at a Small Claims Trial"
    //    "WA Alternatives to Small Claims Court"
    //    "WA After Small Claims Court"
    //    "Estate Planning in Washington State (WA)"
    //    "Washington Expungement and Record Sealing"
    //    "Selling a House in Washington"
    //    "Washington Statutes of Limitations"
    //    "Criminal Process in Washington State (WA)"
    //    ...


[<Support("MarketBank")>]
let dummy1() = ()
type MarketBank = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/RpmConsulting/MarketBank/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("MarketBank - Retail Financial Services Demand & Potential");
  Description("Consume Azure Marketplace data from MarketBank Retail Financial Services Demand & Potential. For more information, please goto https://datamarket.azure.com/dataset/13d66793-c703-4efb-9198-ca95802c0d13")>]
let MarketBank() = 
    let data = MarketBank.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    data.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for b in data.MarketBank2010 do
        where (b.StateAbbreviation = "WA")
        sortBy (b.Id)
        select b
    }|>  Seq.iter(fun i -> printfn "%A - %A" i.Id i.AnnuitiesPenetrationIndex)

    // sample output
    //    "530019501001" - 113
    //    "530019501002" - 116
    //    "530019501003" - 78
    //    "530019501004" - 122
    //    "530019502001" - 132
    //    "530019502002" - 113
    //    "530019502003" - 77
    //    "530019503001" - 124
    //    ...


[<Support("BranchInfo")>]
let dummy2() = ()
type BankInfo = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/RpmConsulting/BranchInfo">

[<Category("TypeProviders.AzureMarketPlace");
  Title("BranchInfo - U.S. Bank Branch Locations, History & Performance Database");
  Description("BranchInfo™ 2011 is a historical database of every bank branch location in the U.S. containing information matched by institution and site, across a five-year timeframe. For more information, please goto https://datamarket.azure.com/dataset/36c385f2-5903-4335-af09-294e3043839f")>]
let BranchInfo() = 
    let bank = BankInfo.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    bank.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for b in bank.BranchInfo2011 do
        where (b.BranchState = "WA")
        where (b.BranchZip = "98052")
        select b
    }|>  Seq.iter(fun i -> printfn "%A - %A" i.BranchName i.BranchDeposits)

    // sample output
    //    "Washington Federal S & L Association    " - 1000788M
    //    "Redmond Branch                          " - 1000788M
    //    "Overlake Safeway Branch                 " - 925095M
    //    "Overlake Park Branch                    " - 883017M
    //    "Redmond Branch                          " - 1000788M
    //    "North Redmond Branch                    " - 965031M
    //    "Bel-Red Branch                          " - 987997M
    //    ...


[<Support("DataGov")>]
let dummy4() = ()
type C = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/data.gov/Crimes">

[<Category("TypeProviders.AzureMarketPlace");
  Title("Data.Gov - 2006-2008 Crime in the United States (Data.gov)");
  Description("Consume Azure Marketplace crime data from Data.gov. For more information, please goto https://datamarket.azure.com/dataset/c663117f-db6d-49e1-bc83-b05390bb3c70")>]
let DataGov() = 
    let crime = new C.ServiceTypes.datagovCrimesContainer()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    crime.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    // compare different property crime rates for cities around microsoft
    query {
        for m in crime.CityCrime do
        where (m.City = "Redmond" || m.City = "Kirkland" || m.City = "Bellevue" || m.City = "Seattle" )
        where (m.State = "Washington")
        where (m.Year = 2008)
        sortBy (m.City)
    } |> Seq.map ( fun c -> (c.City, (float c.Burglary + float c.PropertyCrime) / float c.Population * 100.0)) 
      |> Seq.sortBy(fun d -> snd d )
      |> Seq.toList 
      |> printfn "%A" 

    // sample output
    //    [("Redmond", 3.717874241); ("Bellevue", 4.063400812); ("Kirkland", 4.584304584);
    //     ("Seattle", 6.574905907)]


[<Support("Esri")>]
let dummy5() = ()
type D = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = "https://api.datamarket.azure.com/Esri/KeyUSDemographicsTrial/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("ESRI - 2010 Key US Demographics by ZIP Code, Place and County (Trial)");
  Description("2010 Key US Demographics by ZIP Code, Place, and County Data is a select offering of the demographic data required to understand a market. For more information, please goto https://datamarket.azure.com/dataset/c7154924-7cab-47ac-97fb-7671376ff656")>]
let Esri() = 
    let pl = D.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    pl.Credentials <- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for c in pl.demog1 do
        where (c.StateName = "Washington")
        where (c.CountyName = "King County")
        sortBy c.GeographyId
    } |> Seq.filter (fun i -> match i.GeographyName.Trim().ToLower() with 
                              | "redmond" -> true 
                              | "bellevue" -> true
                              | "kirkland" -> true
                              | _ -> false) 
      |> Seq.iter (fun i -> printfn "%A - %A" i.GeographyId i.PerCapitaIncome2010.Value )

    // sample output
    //    "98004" - 58631
    //    "98005" - 44672
    //    "98006" - 51553
    //    "98007" - 36689
    //    "98008" - 43038
    //    "98033" - 55849
    //    "98034" - 39577
    //    "98052" - 47359
    //    "98053" - 47164


[<Support("LexisNexis")>]
let dummy6() = ()
type L = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/LexisNexis/LegalCommunities/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("LexisNexis - Legal Communities");
  Description("Content from the LexisNexis Legal Communities including expert commentary on current legal issues as well as timely podcasts. For more information, please goto https://datamarket.azure.com/dataset/101eb297-c17f-4f22-a844-16ca2e52252d")>]
let LexisNexis() = 
    let legal = new L.ServiceTypes.LexisNexisLegalCommunitiesContainer()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    legal.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for m in legal.CommunityContent do
        where (m.DatePublished  > System.DateTime(2011, 1, 1))
        select m 
    } |> Seq.iter (fun i -> printfn "%A - %A" i.Author i.Title  )

    // sample output
    //    "Ted Connolly" - "What Do Credit Scores Tell Lenders About Us That We Don’t Know?"
    //    "Patrick McCraney" - "Distressed Investors May Feel a Chill in the Air"
    //    "Arina Shulga" - "Doing Business in Russia"
    //    "George Pressly" - "SEC Whistleblowers can Help Uncover "Soft Dollar" Practices"
    //    "Francis G.X. Pileggi" - "Key Delaware Corporate and Commercial Decisions in 2010 "
    //    "Thomas O. Gorman" - "SEC Enforcement Trends 2011"
    //    ...



[<Support("Chanjet")>]
let dummy7() = ()
type SME = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/Chanjet/ChinaSmallAndMediumEnterprises/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("Chanjet - China Small & Medium Enterprises Management & Operation KPI Data");
  Description("Chanjet provides SME operation and management KPI data based on its nationwide survey conducted among SMEs from multiple industries. For more information, please goto https://datamarket.azure.com/dataset/6bd3a6b3-2fe0-4609-ba11-959190000047")>]
let Chanjet() = 
    let china = SME.ServiceTypes.ChanjetChinaSmallAndMediumEnterprisesContainer()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    china.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for b in china.GeographyInfo do
        where (b.EconomicZoneId.Value = 1)
        select b
    } |> Seq.iter (fun i -> printfn "%A - %A" i.ProvinceNameEnglish i.ProvinceNameChinese)

    // sample output
    //    "Shanghai" - "上海"
    //    "Jiangsu"  - "江苏"
    //    "Zhejiang" - "浙江"


[<Support("MetricMash")>]
let dummy8() = ()
type CPI = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/MetricMash/USConsumerPriceIndex/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("MetricMash - U.S. Consumer Price Index (1913 to Current)");
  Description("The MetricMash U.S. Consumer Price Index (CPI) dataset provides the changes in the prices paid by consumers for over 375 goods and services in major expenditure groups – such as food, housing, apparel, transportation, medical care and education cost. For more information, please goto https://datamarket.azure.com/dataset/26058d69-5cad-4a7c-9a14-a21a0c40de86")>]
let MetricMash() = 
    let us = CPI.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    us.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for b in us.AllUrbanChainedConsumerPriceIndex do
        where (b.DateValue < new System.DateTime(2010, 1, 1) )
        where (b.DateValue > new System.DateTime(2008, 12, 30) )
        select b
    } |> Seq.iter (fun i -> printfn "%A - %A" i.DateValue i.Index)

    // sample output
    //    1/1/2009 12:00:00 AM - 122.09500M
    //    1/1/2009 12:00:00 AM - 149.17700M
    //    1/1/2009 12:00:00 AM - 117.95400M
    //    1/1/2009 12:00:00 AM - 86.86600M
    //    1/1/2009 12:00:00 AM - 107.73100M
    //    1/1/2009 12:00:00 AM - 80.52600M
    //    1/1/2009 12:00:00 AM - 110.22400M
    //    ...


[<Support("EDR")>]
let dummy9() = ()
type EHR = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = @"https://api.datamarket.azure.com/EDR/EnvironmentalHazardRank">

[<Category("TypeProviders.AzureMarketPlace");
  Title("EDR - Environmental Hazard Rank");
  Description("The EDR Environmental Hazard Ranking System depicts the relative environmental health of any U.S. ZIP code based on an advanced analysis of its environmental issues. For more information, please goto https://datamarket.azure.com/dataset/d8161344-f755-4a86-83e6-db9a89f06efd")>]
let EDR() = 
    let e = EHR.ServiceTypes.EDREnvironmentalHazardRankContainer()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    e.Credentials<- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for h in e.EnvironmentalHazardRankingByUSPostalZipCode do
        where (h.State = "WA")
        sortByDescending (h.EnviromentalHazardIndex)
        select h
    } |> Seq.iter (fun i -> printfn "%A - %A" i.ZipCode i.EnviromentalHazardIndex)

    // sample output
    //    "98001" - 55120853.90000M
    //    "98002" - 53938430.83000M
    //    "98003" - 31725567.94000M
    //    "98004" - 51774234.93000M
    //    "98005" - 38690824.03000M
    //    "98006" - 16916724.70000M
    //    "98007" - 16998583.21000M
    //    "98008" - 10245452.63000M
    //    ...



[<Support("MetricMash1")>]
let dummy11() = ()
type Unemployment = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = "https://api.datamarket.azure.com/MetricMash/USUnemploymentData/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("MetricMash - U.S. Unemployment Data");
  Description("The MetricMash U.S. Unemployment dataset provides users the very latest government published metrics going back to 1948. For more information, please goto https://datamarket.azure.com/dataset/662d2b6a-119d-447e-a2b9-84d7a7f83590")>]
let MetricMash1() = 
    let Unemply = Unemployment.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    Unemply.Credentials <- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for c in Unemply.UnemploymentRates do
        where (c.Year = "2009")
        select c
    } |> Seq.iter (fun i -> printfn "%A - %A" (i.MonthMMM + ", " + i.Year) i.Value)

    // sample output
    //    "Jan, 2009" - 8.200M
    //    "Jan, 2009" - 9.000M
    //    "Jan, 2009" - 14.100M
    //    "Jan, 2009" - 7.800M
    //    "Jan, 2009" - 8.500M
    //    "Jan, 2009" - 6.900M
    //    "Jan, 2009" - 7.100M
    //    ...


[<Support("MSU")>]
let dummy12() = ()
type Util = Microsoft.FSharp.Data.TypeProviders.ODataService<ServiceUri = "https://api.datamarket.azure.com/Microsoft/UtilityRateService/">

[<Category("TypeProviders.AzureMarketPlace");
  Title("Microsoft - Utility Rate Service");
  Description("The Microsoft Utility Rate Service is a database of electric utilities and rates indexed by location within the United States, gathered from Partner Utilities and/or from publically available information. For more information, please goto https://datamarket.azure.com/dataset/b848b1ed-3f61-40a9-8e43-597e0d352394")>]
let MSU() = 
    let rate = Util.GetDataContext()
    //To sign up for a Windows Azure Marketplace account, please go to https://datamarket.azure.com/account/info
    rate.Credentials <- NetworkCredential (Utils.ADM_USER_ID, Utils.ADM_ACCOUNT_ID)

    query {
        for c in rate.UtilityByPostalCodes do
        where (c.PostalCode = "98052")
        select c
    } |> Seq.iter (fun i -> printfn "%A - %A" i.Name i.RateSource)

    // sample output
    //    "Average Price by State (Residential)" - "AreaAverages"
    //    "Contoso" - "ThirdParty"



