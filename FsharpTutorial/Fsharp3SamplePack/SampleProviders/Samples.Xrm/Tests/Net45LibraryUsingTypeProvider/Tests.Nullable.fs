// Copyright (c) Microsoft Corporation 2005-2013.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose.

module XrmProvider.IntegrationTests.Nullable

open System
open Xunit

// tests nullable type operations and the F# nullable operators
// assumes data has already been created from http://msdn.microsoft.com/en-us/library/gg334593.aspx (TODO: create this in the F# code.. )

open Microsoft.FSharp.Linq.NullableOperators
open Microsoft.Xrm.Sdk.Client
open Microsoft.Xrm.Sdk
open System.ServiceModel.Description
open System.Net
open Samples.XrmProvider

type XRM = Samples.XrmProvider.XrmDataProvider<OrgService,true,Samples.XrmProvider.RelationshipNamingType.CrmStylePrefix,CredentialsFile="Credentials.txt">

let (un,pw,dm) = getCredentials "Credentials.txt"
let dc = XRM.GetDataContext(OrgService,un,pw,dm)

type container = { x : Nullable<DateTime> ; y: string }

[<Fact>] 
let ``Selects where nullable optionset is null`` () = 
    let res = query { for a in dc.accountSet do
                      where (a.address1_addresstypecode.HasValue = false)
                      select a } |> Seq.toList 
    Assert.NotEmpty res

[<Fact>] 
let ``Selects where nullable optionset is not null`` () = 
    let res = query { for c in dc.contactSet do
                      where (c.accountrolecode.HasValue)
                      select c } |> Seq.toList 
    Assert.NotEmpty res

[<Fact>] 
let ``Selects where nullable optionset is equal - special case, no nullable operator`` () = 
    let res = query { for c in dc.contactSet do
                      // can't use nullable operators with enums :( this is a workaround that I have allowed.
                      where (c.accountrolecode.Value = XRM.XrmService.contact_accountrolecode.``Decision Maker`` )
                      select c } |> Seq.toList 
    Assert.NotEmpty res
    Assert.True (res.Length = 1)

[<Fact>] 
let ``Selects single entity sequence with simple attribute projection with nullables`` () =     
        query { for c in dc.contactSet do
                select c.anniversary } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects single entity sequence with tupled new object projection with nullables`` () = 
    query { for c in dc.contactSet do
            select (c.firstname, {x=c.anniversary; y=c.lastname}) } |> Assert.NotEmpty

[<Fact>] 
let ``Selects single entity sequence with simple critiera using nullable HasValue`` () = 
    query { for c in dc.contactSet do
            where (c.anniversary.HasValue)
            select c } |> Assert.NotEmpty
            
[<Fact>] 
let ``Selects single entity sequence with simple critiera HasValue = false`` () = 
    query { for c in dc.contactSet do
            where (c.anniversary.HasValue = false)
            select c } |> Assert.Empty

[<Fact>] 
let ``Selects single entity sequence with simple critiera HasValue = false dynamic`` () = 
    query { for c in dc.contactSet do
            where (c.GetAttribute<System.Nullable<DateTime>>("anniversary").HasValue = false)
            select c } |> Assert.Empty

[<Fact>] 
let ``Selects single entity sequence with simple critiera using nullable Value and nullable operator`` () =     
    query { for c in dc.contactSet do
            where (c.anniversary ?> DateTime(2010,2,5))
            select c.anniversary.Value } |> Assert.NotEmpty
