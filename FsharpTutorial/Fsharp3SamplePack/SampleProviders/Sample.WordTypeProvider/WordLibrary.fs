module WordLibrary

open System
open System.Linq
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Packaging
open DocumentFormat.OpenXml.Wordprocessing

type WordDoc(fileName, editable) = 
    let pattern = "MERGEFIELD(\s+?)(\S+?)\s"    
    let getFieldName (field:SimpleField) = 
        System.Text.RegularExpressions.Regex.Match(field.Instruction.Value, pattern).Groups.[2].Value
    let doc = WordprocessingDocument.Open((fileName:string), editable)

    new(fileName) = new WordDoc(fileName, false)

    member this.Fields =  
        let simpleFields = 
            doc.MainDocumentPart.Document.Descendants<SimpleField>()
            |> Seq.filter (fun field -> System.Text.RegularExpressions.Regex.IsMatch( field.Instruction.Value, pattern ))
        simpleFields

    member this.FieldNames = 
        this.Fields
        |> Seq.map getFieldName

    member this.GetField fieldName =
        this.Fields
        |> Seq.tryFind (fun field -> fieldName = getFieldName(field))

    member this.GetFieldText (fieldName:string) = 
        fieldName 
        |> this.GetField
        |> function
            | Some(field) -> this.GetFieldText field 
            | None -> ""

    member this.GetFieldText (field:SimpleField) = 
        let strings = 
            field.Descendants<Run>()
            |> Seq.collect (fun run -> run.Descendants<Text>())
            |> Seq.map (fun t -> t.Text)
            |> Seq.toArray
        String.Join(" ", strings)

    member this.Replace (field:SimpleField) (text) = 
        field.FieldLock <- OnOffValue.FromBoolean(true) 
        field.Descendants<Run>()
        |> Seq.collect (fun run -> run.Descendants<Text>())
        |> Seq.iter (fun t -> t.Text <- text)

    member this.Close() = doc.Close()

    interface System.IDisposable with
        member this.Dispose() = this.Close()

