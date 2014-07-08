// Copyright (c) Microsoft Corporation 2005-2008.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


open System
open System.Collections.Generic
open System.Windows.Forms
open System.IO
open Support.Helper
open System.Globalization

/// <summary>
/// The main entry point for the application.
/// </summary>
[<STAThread>]
[<EntryPoint>]
let main(args) = 
    let harnesses = getSamples() 
    
    #if XML_SAMPLE
    let xml = getXml harnesses
    xml.Save("sampleAndResult.xml");
    #endif

    let count = harnesses |> Seq.sumBy (fun (s,l) -> l.Length)
    printfn "sample count = %d" count
    printfn "this window will show result from non-UI thread and/or some debug information"

    match args with 
    | [| _; "/runall" |] -> 
        harnesses 
        |> List.iter (fun (_,samples) -> samples |> List.iter (fun s -> if s.Name <> "ExceptionSample1" then s.Run()))
    | _ -> 
        Application.EnableVisualStyles();        
        let r = getResourceString ("Title", typeof<Display.SampleForm>)
        let form = new Display.SampleForm(r, harnesses) 
        ignore(form.ShowDialog())
    0
