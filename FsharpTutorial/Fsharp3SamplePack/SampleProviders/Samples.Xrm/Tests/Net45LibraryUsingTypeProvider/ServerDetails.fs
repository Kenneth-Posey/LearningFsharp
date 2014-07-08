// Copyright (c) Microsoft Corporation 2005-2013.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose.

[<AutoOpen>]
module ServerDetails

open System
open System.IO  

[<Literal>]
let OrgService = "http://server:port/org/XRMServices/2011/Organization.svc"

let getCredentials credentialsFile =
    match credentialsFile with
    | "" -> failwith "no file location specified"
    | _ -> 
        if not (File.Exists credentialsFile) then failwithf "Could not find credentials file at %s" credentialsFile
        let values = File.ReadAllLines credentialsFile
        match values.Length with
        | 2 -> (values.[0],values.[1],"")
        | 3 -> (values.[0],values.[1],values.[2])
        | _ -> failwith "Credentials file should have between two to three lines, containing the username, password and domain respectively"