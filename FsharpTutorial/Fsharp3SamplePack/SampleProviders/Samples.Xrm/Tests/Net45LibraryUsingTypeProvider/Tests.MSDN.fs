// Copyright (c) Microsoft Corporation 2005-2013.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose.

module XrmProvider.IntegrationTests.MSDN

open System
open Xunit

// replicates most of the examples found at http://msdn.microsoft.com/en-us/library/gg334593.aspx
// assumes data has already been created as found at the above link (TODO: create this in the F# code.. )

open Microsoft.Xrm.Sdk.Client
open Microsoft.Xrm.Sdk
open System.ServiceModel.Description
open System.Net

type XRM = Samples.XrmProvider.XrmDataProvider<OrgService,false,Samples.XrmProvider.RelationshipNamingType.CrmStylePrefix,CredentialsFile="Credentials.txt">

open Samples.XrmProvider

let (un,pw,dm) = getCredentials "Credentials.txt"
let dc = XRM.GetDataContext(OrgService,un,pw,dm)

// Note: these tests use generated relationship names using the static parameter option CrmStylePrefix which provides the relationships is the format 1:N , N:1, N:N  

[<Fact>]
let ``MSDN List of Contact info using a paging sort 1`` () =
    printfn "List of Contact info using a paging sort 1"
    printfn "======================================="
    let res = query { for c in dc.contactSet do                      
                      where (c.lastname <> "Parker")
                      sortBy c.lastname 
                      thenByDescending c.firstname 
                      select (c.firstname, c.lastname)
                      skip 2
                      take 2 } |> Seq.toList
    
    res |> List.iter(fun a -> printfn "%s %s" (fst a) (snd a))
    Assert.True((res.Length = 1))

[<Fact>]
let ``MSDN Simple where clause 1 using Contains`` () =
    printfn "List of Accounts using one where clause"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      where (a.name.Contains "Contoso")  
                      select a } |> Seq.toList
    
    res |> List.iter(fun a -> printfn "%s %s" a.name a.address1_city)
    Assert.True((res.Length = 1))
    Assert.True(res.[0].name = "Contoso Ltd")
    Assert.True(res.[0].address1_city = "Redmond")

[<Fact>]
let ``MSDN Simple where clause 1`` () =
    printfn "List of Accounts using one where clause and Like operator"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      where (a.name =% "%Contoso%")  
                      select a } |> Seq.toList
    
    res |> List.iter(fun a -> printfn "%s %s" a.name a.address1_city)
    Assert.True((res.Length = 1))
    Assert.True(res.[0].name = "Contoso Ltd")
    Assert.True(res.[0].address1_city = "Redmond")

[<Fact>]
let ``MSDN Simple where clause 2`` () =
    printfn "List of Accounts using two where clauses"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      where (a.name.StartsWith "Contoso")        
                      where (a.address1_city = "Redmond")
                      select a } |> Seq.toList
    
    res |> List.iter(fun a -> printfn "%s %s" a.name a.address1_city)
    Assert.True((res.Length = 1))
    Assert.True(res.[0].name = "Contoso Ltd")
    Assert.True(res.[0].address1_city = "Redmond")

[<Fact>]
let ``MSDN Join and simple where clause query`` () =
    printfn "List of Account and Contact Info using where clause"
    printfn "======================================="
    let res = query { for c in dc.contactSet do
                      for a in c.``1:N account_primary_contact`` do 
                      where (a.name =% "%Contoso%")       
                      where (c.lastname =% "%Smith%")
                      select (a.name, c.lastname) } |> Seq.toList
   
    res |> List.iter(fun (n,ln) -> printfn "%s %s" n ln)
    Assert.True((res.Length = 1))
    Assert.True(fst res.[0] = "Contoso Ltd")
    Assert.True(snd res.[0] = "Smith")

[<Fact>]
let ``MSDN Join and simple where clause query N:1`` () =
    printfn "List of Account and Contact Info using where clause"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      for c in a.``N:1 account_primary_contact`` do
                      where (a.name =% "%Contoso%")
                      where (c.lastname =% "%Smith%")
                      select (a.name, c.lastname) } |> Seq.toList
    
    res |> List.iter(fun (n,ln) -> printfn "%s %s" n ln)
    Assert.True((res.Length = 1))
    Assert.True(fst res.[0] = "Contoso Ltd")
    Assert.True(snd res.[0] = "Smith")

[<Fact>]
let ``MSDN List of Contact Info using Distinct operator`` () =
    printfn "List of Contact Info using Distinct operator"
    printfn "======================================="
    let res = query { for c in dc.contactSet do
                      select c.lastname
                      distinct } |> Seq.toList
   
    res |> List.iter(fun a -> printfn "%s" a)
    Assert.True((res.Length = 3))
    Assert.True(res.[0] = "Parker")
    Assert.True(res.[1] = "Smith")
    Assert.True(res.[2] = "Wilcox")

[<Fact>]
let ``MSDN List of Contact and Account Info Using join 1`` () =
    printfn "List of Contact and Account Info Using join 1"
    printfn "======================================="
    let res = query { for c in dc.contactSet do
                      for a in c.``1:N account_primary_contact`` do
                      select 
                        (c.fullname, 
                         c.address1_city,
                         a.name,
                         a.address1_name) } |> Seq.toList
    
    res |> List.iter(fun (fullName,_,name,_) -> printfn "acct: %s\t\t\tContact:%s" name fullName )
    Assert.True((res.Length = 1))
    let (fullName,_,name,_) = res.[0]
    Assert.True((fullName = "Brian Smith"))
    Assert.True((name = "Contoso Ltd"))

[<Fact>]
let ``MSDN List of Contact, Account, Lead Info using multiple join 4`` () =
    printfn "List of Contact, Account, Lead Info using multiple join 4"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      for c in a.``N:1 account_primary_contact`` do
                      for l in a.``N:1 account_originating_lead`` do
                      select (c.fullname, a.name, l.fullname ) } |> Seq.toList
    
    res |> List.iter(fun (contactName,accountName,leadName) -> printfn "%s  %s  %s" contactName accountName leadName )
    Assert.True((res.Length = 1))
    let(contactName,accountName,leadName) = res.[0]
    Assert.True((contactName = "Brian Smith"))
    Assert.True((accountName = "Contoso Ltd"))
    Assert.True((leadName = "Diogo Andrade"))
    
[<Fact>] 
let  ``MSDN List of Contact, Account, Lead Info using multiple join 4 Explicit`` () =
    let (contactName,accountName,leadName) =
         query{ for a in dc.accountSet do                
                join c in dc.contactSet on (a.primarycontactid.Id = c.contactid)                                     
                join l in dc.leadSet on (a.originatingleadid.Id = l.leadid) 
                select (c.fullname, a.name, l.fullname )
                exactlyOne } 
    Assert.True((contactName = "Brian Smith"))
    Assert.True((accountName = "Contoso Ltd"))
    Assert.True((leadName = "Diogo Andrade"))
   
[<Fact>] 
let  ``MSDN List of Contact, Account, Lead Info using multiple join 4 Explicit 2`` () =
    let (contactName,accountName,leadName) =
         query{ for a in dc.accountSet do                
                join c in dc.contactSet on (a.primarycontactid.Id = c.contactid)                                     
                where (c.fullname = "Brian Smith")
                join l in dc.leadSet on (a.originatingleadid.Id = l.leadid) 
                where (l.fullname = "Diogo Andrade")
                select (c.fullname, a.name, l.fullname )
                exactlyOne } 
    Assert.True((contactName = "Brian Smith"))
    Assert.True((accountName = "Contoso Ltd"))
    Assert.True((leadName = "Diogo Andrade"))
    
[<Fact>]
let ``MSDN List of Account Info using self join 5`` () =
    printfn "List of Account Info using self join 5"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      for a2 in a.``N:1 account_parent_account`` do
                      select (a.name,a2.address1_city) } |> Seq.toList
    
    res |> List.iter(fun (name,city) -> printfn "%s  %s" name city )
    Assert.True((res.Length = 1))
    let(name, city) = res.[0]
    Assert.True((name = "Contoso Ltd"))
    Assert.True((city = "Redmond"))


[<Fact>] 
let ``MSDN List of Account Info using self join 5 Explicit`` () =
    let (name,city) =
         query{ for a in dc.accountSet do                
                join a2 in dc.accountSet on (a.parentaccountid.Id = a2.accountid)                                     
                select(a.name,a2.address1_city) 
                exactlyOne }
    
    Assert.True((name = "Contoso Ltd"))
    Assert.True((city = "Redmond"))

[<Fact>]
let ``MSDN List of Contact Info using double join 6`` () =
    printfn "List of Contact Info using double join 6"
    printfn "======================================="
    let res = query { for c in dc.contactSet do
                      for a in c.``1:N account_primary_contact`` do
                      for a2 in a.``N:1 account_parent_account`` do
                      select (c.fullname, a.name) } |> Seq.toList
    
    res |> List.iter(fun (fullName,name) -> printfn "%s  %s" fullName name )
    Assert.True((res.Length = 1))
    let(fullName, name) = res.[0]
    Assert.True((fullName = "Brian Smith"))
    Assert.True((name = "Contoso Ltd"))    


[<Fact>]
let ``MSDN Entity Fields join 7`` () =
    printfn "Entity Fields join 7"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      for c in a.``N:1 account_primary_contact`` do
                      where (a.name = "Contoso Ltd" && a.address1_name = "Contoso Pharmaceuticals")
                      select a } |> Seq.toList
    
    res |> List.iter(fun a -> printfn "Account %s and it's primary contact %A" a.name a.primarycontactid.Id)
    Assert.True((res.Length = 1))


[<Fact>]
let ``MSDN Entity Fields join 7 Explicit`` () =
    let res = query { for a in dc.accountSet do
                      join c in dc.contactSet on (a.primarycontactid.Id = c.contactid)
                      where (a.name = "Contoso Ltd" && a.address1_name = "Contoso Pharmaceuticals")
                      select a } |> Seq.toList    
    Assert.True((res.Length = 1))


[<Fact>]
let ``MSDN List of Contact Info using left join 8`` () =
    printfn "List of Contact Info using left join 8"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      for c in (!!) a.``N:1 account_primary_contact`` do
                      select (a.name,c.fullname) } |> Seq.toList
    
    res |> List.iter(fun (name,fullName) -> printfn "%s  %s" name fullName)
    Assert.True((res.Length = 2))

[<Fact>]
let ``MSDN Using the greater than operator 1`` () =
    printfn "Using the greater than operator 1"
    printfn "======================================="
    let res = query { for c in dc.contactSet do
                      where (c.anniversary > DateTime(2010,2,5))
                      select (c) } |> Seq.toList
    
    res |> List.iter(fun (name) -> printfn "%O" name )
    Assert.True((res.Length = 4))

[<Fact>]
let ``MSDN List of Account Info using self join 6`` () =
    printfn "List of Account Info using self join 6"
    printfn "======================================="
    let res = query { for a in dc.accountSet do
                      for a2 in a.``N:1 account_parent_account`` do
                      select (a,a2) } |> Seq.toList
    
    Assert.True((res.Length = 1))
    
                                