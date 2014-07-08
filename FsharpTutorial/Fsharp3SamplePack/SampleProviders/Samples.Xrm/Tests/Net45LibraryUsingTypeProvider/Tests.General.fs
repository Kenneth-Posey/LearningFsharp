// Copyright (c) Microsoft Corporation 2005-2013.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose.

module XrmProvider.IntegrationTests.General

open System
open Xunit

// assumes data has already been created from http://msdn.microsoft.com/en-us/library/gg334593.aspx (TODO: create this in the F# code.. )

open Microsoft.Xrm.Sdk.Client
open Microsoft.Xrm.Sdk
open System.ServiceModel.Description
open System.Net
open Samples.XrmProvider

type XRM = XrmDataProvider<OrgService,false,RelationshipNamingType.CrmStylePrefix,CredentialsFile="Credentials.txt">

let (un,pw,dm) = getCredentials "Credentials.txt"
let dc = XRM.GetDataContext(OrgService,un,pw,dm)

type container = { x : string ; y: string }

////////////////////////////////////////////////////////////////////
// single entity sequence tests
////////////////////////////////////////////////////////////////////
[<Fact>] 
let ``Selects single entity sequence`` () = 
    let res = query { for a in dc.accountSet do
                                     
                      select a } |> Seq.toList 
    Assert.NotEmpty res
    Assert.True(res.[0].LogicalName = "account")

[<Fact>] 
let ``Selects single entity sequence with simple attribute projection`` () =     
    let res = query { for a in dc.accountSet do
                        select a.name } |> Seq.toList
    Assert.True(res.[0] <> "")

[<Fact>] 
let ``Selects single entity sequence with tupled attribute projection`` () = 
    let res =  query { for a in dc.accountSet do
                       select (a.name,a.address1_city) } |> Seq.toList
    Assert.True(fst res.[0] <> "" && snd res.[0] <> "")

[<Fact>] 
let ``Selects single entity sequence with new object projection`` () = 
    query { for a in dc.accountSet do
            select {x=a.name; y=a.address1_city} } 
    |> Seq.iter(fun x -> Assert.True(x.x <> "" && x.y <> ""))
    
[<Fact>] 
let ``Selects single entity sequence with tupled new object projection`` () = 
    query { for a in dc.accountSet do
            select (a.name, {x=a.address1_city ; y=a.address1_name}) } 
    |> Seq.iter(fun (x,y) -> Assert.True(x <> "" && y.x <> "" && y.y <> ""))
        
[<Fact>] 
let ``Selects single entity sequence with simple critiera`` () = 
    // note: this is an important test because the expression tree does not contain any form of Select method.
    query { for a in dc.accountSet do
            where (a.name = "Contoso Ltd")
            select a } |> Assert.NotEmpty
   
[<Fact>] 
let ``Selects single entity sequence with simple critiera and attribute projection`` () = 
    query { for a in dc.accountSet do
            where (a.name = "Contoso Ltd")
            select a.name } |> Assert.NotEmpty

[<Fact>] 
let ``Selects single entity sequence with simple OR critiera`` () = 
    query { for a in dc.accountSet do
            where (a.name ="Contoso Ltd" || a.address1_name = "Coho Vineyard & Winery" )
            select a.address1_city } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects single entity sequence with object based criteria`` () = 
    let x = { x = ""; y = "%Contoso%"}
    query { for a in dc.accountSet do
            where (a.name <>% x.y)          
            where (a.address1_city |=| [|"Redmond";"London"|])
            select a.address1_name } |> Assert.NotEmpty 

////////////////////////////////////////////////////////////////////
// single entity with implemented linq execution methods
////////////////////////////////////////////////////////////////////
[<Fact>]
let ``exactlyOne works with single entity sequence`` () = 
    let name = query { for a in dc.accountSet do                      
                       where (a.name = "Contoso Ltd")
                       select a.name
                       exactlyOne } 
    Assert.True((name = "Contoso Ltd"))
    // this next assert will fail becaus there are two accounts
    Assert.Throws<System.InvalidOperationException>( fun () -> query { for a in dc.accountSet do
                                                                       select a.name
                                                                       exactlyOne } |> ignore ) 

////////////////////////////////////////////////////////////////////
// multiple entity sequences
////////////////////////////////////////////////////////////////////
[<Fact>] 
let ``Selects parent entity sequence with single child entity projection`` () =     
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                select c } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent entity sequence with single child entity attribute projection`` () =     
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                select c.firstname } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent entity sequence with child and parent entity projection`` () =     
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent entity sequence with parent entity projection`` () =     
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                select a } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria 1`` () =     
        query { for c in dc.contactSet do
                where (c.lastname = "Smith")
                for a in c.``1:N account_primary_contact`` do                
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria 2`` () =     
        query { for c in dc.contactSet do                
                for a in c.``1:N account_primary_contact`` do
                where (c.lastname = "Smith")
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria 3`` () =     
        query { for c in dc.contactSet do
                where (c.lastname = "Smith")
                for a in c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria 4`` () =     
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria 5`` () =
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                where (c.lastname = "Smith")
                select (a,c) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria  6`` () =
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John"|])
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects parent and child entity sequence with criteria  7`` () =
        query { for c in dc.contactSet do
                where (c.lastname = "Smith")
                for a in c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John'"|])
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple parent entity sequence with criteria 1`` () =
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                for o in c.``N:1 owner_contacts`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John'"|])
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple parent entity sequence with criteria 2`` () =
        query { for c in dc.contactSet do
                for o in c.``N:1 owner_contacts`` do
                for a in c.``1:N account_primary_contact`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John'"|])
                select (a,c) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple parent entity sequence with criteria 3`` () =
        query { for c in dc.contactSet do
                where (c.firstname |=| [|"Brian";"John'"|])
                for o in c.``N:1 owner_contacts`` do
                for a in c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                where (c.lastname = "Smith")
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple parent entity sequence with criteria 4`` () =
        query { for c in dc.contactSet do
                where (c.firstname |=| [|"Brian";"John'"|])
                for a in c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                where (c.lastname = "Smith")
                for o in c.``N:1 owner_contacts`` do
                select (c,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple parent entity sequence with criteria 5`` () =
        query { for c in dc.contactSet do
                where (c.firstname |=| [|"Brian";"John'"|])
                for a in c.``1:N account_primary_contact`` do
                where (c.lastname = "Smith")
                for o in c.``N:1 owner_contacts`` do
                where (a.name = "Contoso Ltd")
                select (c,o,a) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria 1`` () =
        query { for c in dc.contactSet do
                where (c.firstname |=| [|"Brian";"John'"|])
                for a in c.``1:N account_primary_contact`` do
                for ao in a.``N:1 owner_accounts`` do
                where (c.lastname = "Smith")
                for o in c.``N:1 owner_contacts`` do
                where (a.name = "Contoso Ltd")
                select (c,a,ao,o) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria 2`` () =
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                for ao in a.``N:1 owner_accounts`` do
                for o in c.``N:1 owner_contacts`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John'"|])
                select (c,a,ao,o) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria 3`` () =
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                for o in c.``N:1 owner_contacts`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John'"|])
                for ao in a.``N:1 owner_accounts`` do
                select (c,a,ao,o) } |> Assert.NotEmpty

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria 4`` () =
        query { for c in dc.contactSet do
                for a in c.``1:N account_primary_contact`` do
                where (c.lastname = "Smith")
                where (a.name = "Contoso Ltd")
                where (c.firstname |=| [|"Brian";"John'"|])
                for ao in a.``N:1 owner_accounts`` do
                for o in c.``N:1 owner_contacts`` do
                select (c,a,ao,o) } |> Assert.NotEmpty 

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria and complex projection`` () =
    let x = { x = ""; y = "Bri%"}
    query { for c in dc.contactSet do
            where (c.firstname <>% x.y || c.lastname = "Smith")
            where (c.address1_city |=| [|"Bellevue";"London'"|])
            for a in (!!) c.``1:N account_primary_contact`` do
            where (a.name = "Contoso Ltd")
            for cb in c.``N:1 lk_contactbase_createdby`` do
            for o in a.``N:1 owner_accounts`` do
            select({ y = c.lastname.ToString(); x = a.name },a,o,cb ) } |> Assert.NotEmpty 


////////////////////////////////////////////////////////////////////
// Entity re-creation tests
////////////////////////////////////////////////////////////////////
[<Fact>]
let ``Parents and children are correctly re-created from projections`` () =
     query { for c in dc.contactSet do                                                
             for a in c.``1:N account_primary_contact`` do
             for ao in a.``N:1 owner_accounts`` do
             for o in c.``N:1 owner_contacts`` do
             select (c,a,ao,o) } 
    |> Seq.iter(fun (c,a,ao,o) -> 
        Assert.True(c.contactid <> System.Guid.Empty)
        Assert.True(c.LogicalName = "contact")
        Assert.True(c.Attributes |> Seq.forall(fun a -> a.Key.Contains(".") = false))
        Assert.True(a.accountid <> System.Guid.Empty)
        Assert.True(a.LogicalName = "account")
        Assert.True(a.Attributes |> Seq.forall(fun a -> a.Key.Contains(".") = false))
        Assert.True(ao.ownerid <> System.Guid.Empty)
        Assert.True(ao.LogicalName = "owner")
        Assert.True(ao.Attributes |> Seq.forall(fun a -> a.Key.Contains(".") = false))
        Assert.True(o.ownerid <> System.Guid.Empty)
        Assert.True(o.LogicalName = "owner")
        Assert.True(o.Attributes |> Seq.forall(fun a -> a.Key.Contains(".") = false)))

///////////////////////////////////////////////////////////////////
// Option set / enum tests
////////////////////////////////////////////////////////////////////
[<Fact>] 
let ``Select entity sequence with option set criteria`` () = 
    let res = query { for a in dc.contactSet do
                      where (a.accountrolecode = XRM.XrmService.contact_accountrolecode.Employee )
                      select a } |> Seq.toList 
    Assert.NotEmpty res
    Assert.True (res |> List.forall(fun contact -> contact.accountrolecode = XRM.XrmService.contact_accountrolecode.Employee ))


[<Fact>] 
let ``Select entity sequence with option set criteria 2`` () = 
    let res = query { for a in dc.contactSet do
                      where (a.accountrolecode <> XRM.XrmService.contact_accountrolecode.Employee )
                      select a } |> Seq.toList 
    Assert.NotEmpty res
    Assert.True (res |> List.forall(fun contact -> contact.accountrolecode <> XRM.XrmService.contact_accountrolecode.Employee ))

///////////////////////////////////////////////////////////////////
// Individuals tests
////////////////////////////////////////////////////////////////////
[<Fact>] 
let ``Select individual account entity`` () = 
    let coho = dc.accountSet.Individuals.``Coho Winery``    
    Assert.True (coho.Id <> Guid.Empty)
    
///////////////////////////////////////////////////////////////////
// Formatted values tests
////////////////////////////////////////////////////////////////////
[<Fact>] 
let ``Select individual account formatted values`` () = 
    let ben = dc.contactSet.Individuals. ``Ben Smith``
    Assert.True (ben.Formatted.accountrolecode = "Employee")

[<Fact>] 
let ``Select account formatted values LINQ`` () = 
    // note: this query checks that a common problem with the C# provider is avoided.
    // in the c# provider, using the formatted values dictionary in the projection is not enough to tell the 
    // provider that you want to select that attribute -as such you have to both select the attribute in the normal way
    // AND index the formatted collection
    let res = query { for c in dc.contactSet do 
                      select (c.address1_city,c.Formatted.accountrolecode) } |> Seq.toList 
    // here we pick some other attribute (city) to force the provider to not select simply everything,
    // then also select a formatted value to assert this prompts the provider to include accountrolecode in the 
    // attributes to select 
    Assert.NotEmpty res
    Assert.True (res |> List.forall( snd >> (<>) "" ))

[<Fact>] 
let ``Select account formatted values LINQ dynamic`` () = 
    // same as above but in C# fashion with string index to actual collection rather than typed projection    
    let res = query { for c in dc.contactSet do 
                      select (c.address1_city,c.FormattedValues.["accountrolecode"]) } |> Seq.toList 
    Assert.NotEmpty res
    Assert.True (res |> List.forall( snd >> (<>) "" ))    


[<Fact>] 
let ``Select account formatted values LINQ dynamic with contains key check`` () = 
    let res = query { for c in dc.contactSet do 
                      select 
                       (c.address1_city,
                        if c.FormattedValues.ContainsKey "accountrolecode" then c.FormattedValues.["accountrolecode"] else "") } 
                        |> Seq.toList 
    Assert.NotEmpty res
    Assert.True (res |> List.forall( snd >> (<>) "" ))  

///////////////////////////////////////////////////////////////////
// Misc tests
////////////////////////////////////////////////////////////////////

[<Fact>]
let ``Sorting works when using child relationships`` () =
    let res = query { for c in dc.contactSet do 
                      for a in c.``1:N account_primary_contact`` do
                      sortBy a.name
                      select (c.lastname, a.name ) } |> Seq.toList
    // in this test, the account is the ultimate child entity and as such it is that entity
    // which is able to be sorted
    Assert.NotEmpty res


[<Fact>]
let ``Sorting works when using child relationships negative`` () =
    let res = query { for c in dc.contactSet do 
                      for a in c.``1:N account_primary_contact`` do
                      sortBy c.lastname
                      select (c.lastname, a.name ) } 
    // in this test, the account is the ultimate child entity and as such it is that entity
    // which is able to be sorted, and NOT the contact
    Assert.Throws<System.Exception>( fun () -> res |> Seq.toList |> ignore )


[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria and complex projection, left join, option sets, exactly one and formatted values`` () =
    let x = { x = ""; y = "Bri%"}
    let (record,account,accountOwner,contactCreatedBy,contactId,famStatusCode,contactMethod) =
        query { for c in dc.contactSet do
                where (c.firstname <>% x.y || c.lastname = "Smith")
                where (c.address1_city |=| [|"Bellevue";"London'"|])
                where (c.accountrolecode = XRM.XrmService.contact_accountrolecode.Employee)
                for a in (!!) c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                for cb in c.``N:1 lk_contactbase_createdby`` do
                for o in a.``N:1 owner_accounts`` do
                select({ y = c.lastname.ToString(); x = a.name },a,o,cb,c.contactid, c.Formatted.familystatuscode,a.Formatted.preferredcontactmethodcode ) 
                exactlyOne } 
    Assert.True ((famStatusCode = "Divorced"))
    Assert.True ((record.x = "Contoso Ltd" && record.y = "Smith"))
    Assert.True (accountOwner.Id <> Guid.Empty)
    Assert.True (contactCreatedBy.Id <> Guid.Empty)
    Assert.True (contactId <> Guid.Empty)
    Assert.True ((contactMethod = "Any"))

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria and complex projection, left join, option sets, exactly one and formatted values 2`` () =
    let x = { x = ""; y = "Bri%"}
    let (record,account,accountOwner,contactCreatedBy,contactId,famStatusCode,contactMethod) =
        query { for c in dc.contactSet do
                where (c.firstname <>% x.y || c.lastname = "Smith")
                where (c.address1_city |=| [|"Bellevue";"London'"|])                
                for a in (!!) c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")
                for cb in c.``N:1 lk_contactbase_createdby`` do
                for o in a.``N:1 owner_accounts`` do
                where (c.accountrolecode = XRM.XrmService.contact_accountrolecode.Employee)
                select(
                    { y = c.lastname.ToString(); x = a.name },
                    a,o,cb,
                    c.contactid, 
                    c.Formatted.familystatuscode,
                    a.Formatted.preferredcontactmethodcode)
                exactlyOne } 
    Assert.True ((famStatusCode = "Divorced"))
    Assert.True ((record.x = "Contoso Ltd" && record.y = "Smith"))
    Assert.True (accountOwner.Id <> Guid.Empty)
    Assert.True (contactCreatedBy.Id <> Guid.Empty)
    Assert.True (contactId <> Guid.Empty)
    Assert.True ((contactMethod = "Any"))

[<Fact>] 
let ``Selects child and multiple nested parent entity sequence with criteria and complex projection, left join, option sets, exactly one and formatted values 3`` () =
    let x = { x = ""; y = "Bri%"}
    let (record,account,accountOwner,contactCreatedBy,contactId,famStatusCode,contactMethod) =
        query { for c in dc.contactSet do
                where (c.accountrolecode = XRM.XrmService.contact_accountrolecode.Employee)
                where (c.firstname <>% x.y || c.lastname = "Smith")
                where (c.address1_city |=| [|"Bellevue";"London'"|])                
                for a in (!!) c.``1:N account_primary_contact`` do
                where (a.name = "Contoso Ltd")                
                for cb in c.``N:1 lk_contactbase_createdby`` do
                for o in a.``N:1 owner_accounts`` do
                select({ y = c.lastname.ToString(); x = a.name },a,o,cb,c.contactid, c.Formatted.familystatuscode,a.Formatted.preferredcontactmethodcode ) 
                exactlyOne }  
    Assert.True ((famStatusCode = "Divorced"))
    Assert.True ((record.x = "Contoso Ltd" && record.y = "Smith"))
    Assert.True (accountOwner.Id <> Guid.Empty)
    Assert.True (contactCreatedBy.Id <> Guid.Empty)
    Assert.True (contactId <> Guid.Empty)
    Assert.True ((contactMethod = "Any"))

[<Fact>]
let ``Enumerating directly from parent relationship`` () = 
    let lead = dc.accountSet.Individuals.``Contoso Ltd``.``N:1 account_originating_lead`` |> Seq.exactlyOne
    Assert.True(lead.firstname = "Diogo")

[<Fact>]
let ``Enumerating directly from parent relationship does not work if FK attribute was not selected`` () = 
    let acc = query { for acc in dc.accountSet do
                      select (acc.name,acc)} |> Seq.head |> snd
    Assert.Throws<System.Exception>( fun () -> acc.``N:1 account_originating_lead`` |> Seq.exactlyOne |> ignore )

[<Fact>]
let ``Enumerating directly from child relationship`` () = 
    let account = dc.leadSet.Individuals.``Diogo Andrade``.``1:N account_originating_lead`` |> Seq.exactlyOne
    Assert.True(account.name = "Contoso Ltd")

[<Fact>]
let ``Querying directly from relationship`` () =
    let cont = dc.contactSet.Individuals.``Brian Smith``
    let (a,o,l) = query { for acc in cont.``1:N account_primary_contact``do
                          for l in acc.``N:1 account_originating_lead`` do
                          for owner in acc.``N:1 owner_accounts`` do
                          where (l.firstname = "Diogo")
                          select (acc,owner,l) 
                          exactlyOne }   
    Assert.True(l.firstname = "Diogo")

// data binding tests
[<Fact>]
let ``Data binding fires INotifyPropertyChanged when attribute values modified`` () =
    let cont = dc.contactSet.Individuals.``Brian Smith``    
    Assert.PropertyChanged((cont:>System.ComponentModel.INotifyPropertyChanged),"fullname",fun () -> cont.fullname <- "john")


    
