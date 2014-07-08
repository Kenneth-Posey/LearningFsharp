// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("Other samples")>]
module Samples.OfficeInterop

open System
open System.Collections.Generic
open Microsoft.Office.Interop
open Support.Helper

[<Category("Office Interop");
  Title("Excel");
  Description("Reading and writing from and to Excel. Please make sure you have Excel 2010 installed on your computer.")>]
let openExcel() = 

    try
        // Start Excel, Open a exiting file for input and create a new file output.xlsx
        let xlApp = new Excel.ApplicationClass()
        let currentPath = System.IO.Path.GetFullPath(@".\Support.ExcelFile.xlsx");
        let xlWorkBookInput = xlApp.Workbooks.Open(currentPath)
        let xlWorkBookOutput = xlApp.Workbooks.Add()
        xlApp.Visible <- true
        // Open input's 'Sheet1' and create a new worksheet in output.xlsx
        let xlWorkSheetInput = xlWorkBookInput.Worksheets.["Sheet1"] :?> Excel.Worksheet
        let xlWorkSheetOutput = xlWorkBookOutput.Worksheets.[1] :?> Excel.Worksheet
        xlWorkSheetOutput.Name <- "OutputSheet1"
        // Reading\Writing a cell value using cell index
        let value1 = xlWorkSheetInput.Cells.[10,5]
        xlWorkSheetOutput.Cells.[10,5] <- value1 
        // Reading\Writing a cell value using range
        let value2 = xlWorkSheetInput.Cells.Range("E10","E10").Value2
        xlWorkSheetOutput.Cells.Range("E10","E10").Value2 <- value2
        // Reading\Writing a row
        let row = xlWorkSheetInput.Cells.Rows.[1] :?> Excel.Range
        (xlWorkSheetOutput.Cells.Rows.[1] :?> Excel.Range).Value2 <- row.Value2
        // Reading\Writing a column
        let column1 = xlWorkSheetInput.Cells.Range("A:A")
        xlWorkSheetOutput.Cells.Range("A:A").Value2 <- column1.Value2
        // Reading\Writing a Range
        let inputRange = xlWorkSheetInput.Cells.Range("A1","E10")
        for i in 1 .. inputRange.Cells.Rows.Count do
            for j in 1 .. inputRange.Cells.Columns.Count  do
                xlWorkSheetOutput.Cells.[i,j] <- inputRange.[i,j]    
        //write jagged array
        let data =  [|  [|0 .. 1 .. 2|];
                    [|0 .. 1 .. 4|];
                    [|0 .. 1 .. 6|] |] 
        for i in 1 .. data.Length do
            for j in 1 .. data.[i-1].Length do
                xlWorkSheetOutput.Cells.[j, i] <- data.[i-1].[j-1]
    with _ -> printfn "please check Excel installation"

[<Category("Office Interop");
  Title("Excel Chart");
  Description("Creating Excel charts. Please make sure you have Excel 2010 installed on your computer.")>]
let excelChart() = 
    try
        let dataX = [|0.0 .. 0.1 .. 10.|]
        let dataY = [|  [|for f in dataX -> cos f|];
                        [|for f in dataX -> sin f|] |] 
        // Update the excel charting object
        let xlApp = new Excel.ApplicationClass()
        let xlWorkBook = xlApp.Workbooks.Add()
        let xlWorkSheet = xlWorkBook.Worksheets.[1] :?> Excel.Worksheet
        let xlCharts = xlWorkSheet.ChartObjects() :?> Excel.ChartObjects
        let xlChart = xlCharts.Add(1., 1., 460., 380.)
        let myChart = xlChart.Chart 
        // Fill in a excel worksheet with data from dataY[][]
        for i in 1 .. dataY.Length do
            for j in 1 .. dataY.[i-1].Length do
                xlWorkSheet.Cells.[j, i] <- dataY.[i-1].[j-1]
        let xlRange = xlWorkSheet.Cells.CurrentRegion
        myChart.SetSourceData(xlRange) 
        // Set Plot type and show chart
        myChart.ChartType <- Excel.XlChartType.xlXYScatterLines
        xlApp.Visible <- true
    with _ -> printfn "%s" "please check your Excel installation"

[<Category("Office Interop");
  Title("Word");
  Description("This sample create a Word instance, insert text, table, and chart into a word document. Please make sure Word 2010 installed on your computer.")>]
let wordSample() = 
    try
        // open a new Word document
        let word = Word.ApplicationClass()
        word.Visible <- true
        let doc = word.Documents.Add()
        // insert a sentence
        let paragraph = doc.Content.Paragraphs.Add()
        paragraph.Range.Text <- "F# sample"
        paragraph.Range.Font.Bold <- 1
        paragraph.Format.SpaceAfter <- 24.0F   //24 pt sapcing
        paragraph.Range.InsertParagraphAfter();
        //insert a table
        let oEndOfDoc = "\\endofdoc" :> obj
        let wrdRng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range
        let table = doc.Tables.Add(wrdRng, 3, 5)    
        table.Range.ParagraphFormat.SpaceAfter <- 6.F
        for r in [1..3] do
            for c in [1..5] do
                table.Cell(r, c).Range.Text <- sprintf "row %d col %d" r c
        table.Rows.[1].Range.Font.Bold <- 0
        table.Rows.[1].Range.Font.Italic <- 1
        //insert chart
        let classType = "MSGraph.Chart.8" :> obj    
        let wrdRng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
        let shape = wrdRng.InlineShapes.AddOLEObject(ref classType)
        let chart = shape.OLEFormat.Object
        let chartApp = chart.GetType().InvokeMember("Application", Reflection.BindingFlags.GetProperty, null, chart, null)
        let parameters = [|4|] |> Array.map (fun n -> n :> obj)
        chart.GetType().InvokeMember("ChartType", Reflection.BindingFlags.SetProperty, null, chart, parameters) |> ignore
    with _ -> printfn "%s" "please check your Word installation"
