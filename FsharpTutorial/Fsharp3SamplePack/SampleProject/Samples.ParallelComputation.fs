// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("Parallel and Async")>]
module Samples.ParallelComputation

open System
open System.Collections.Generic
open System.Threading
open System.IO
open System.Net
open System.Threading.Tasks
open Support.Helper

[<Category("Parallel");
  Title("Thread");
  Description("This sample spawns threads at background. Please check the thread output at the background Console window.")>]
let ParallelSample1() = 
    // What will execute on each thread
    let threadBody() =
        for i in 1 .. 5 do
            // Wait 1/10 of a second
            Thread.Sleep 100
            printfn "[Thread %d] %d..."
                Thread.CurrentThread.ManagedThreadId
                i

    let spawnThread() =
        let thread = new Thread(threadBody)
        thread.Start()

    printfn "please check the background console window for output"

    // Spawn a couple of threads at once
    spawnThread()
    spawnThread()

[<Category("Parallel");
  Title("Async");
  Description("This sample uses the F# async to download the web pages in a parallel way. The output is three web pages' content.")>]
let ParallelSample2() = 
    let getHtml (url: string) =
        async {
            let req = WebRequest.Create(url)
            let! rsp = req.AsyncGetResponse()

            use stream = rsp.GetResponseStream()
            use reader = new StreamReader(stream)            
            return reader.ReadToEnd()
        }

    let webPages =
        [ "http://www.tryfsharp.org/About.aspx";
          "http://www.tryfsharp.org/Tools.aspx";
          "http://www.tryfsharp.org/FunctionalProg.aspx" ]
        |> List.map getHtml
        |> Async.Parallel
        |> Async.RunSynchronously

    webPages |> Seq.iter (fun n -> printfn "%s" n)
 
[<Category("Parallel");
  Title("TPL");
  Description("Task Parallel Library (TPL) is applied in this sample to manipulate matrix. Code originally from Chris Smith's book")>]   
let ParallelSample3() = 
    // multiply two matrices using the PFX
    let matrixMultiply (a: float[,]) (b: float[,]) =
        let aRow, aCol = Array2D.length1 a,
                         Array2D.length2 a
        let bRow, bCol = Array2D.length1 b,
                         Array2D.length2 b
        if aCol <> bRow then
            failwith "Array dimension mismatch."

        // Allocate space for the resulting matrix, c
        let c = Array2D.create aCol bRow 0.0
        let cRow, cCol = aCol, bRow

        // compute each row of the resulting matrix
        let rowTask rowIdx =
           for colIdx = 0 to cCol - 1 do
               for x = 0 to aRow - 1 do
                   c.[colIdx, rowIdx] <-
                      c.[colIdx, rowIdx] +
                      a.[x, colIdx] * b.[rowIdx, x]
           ()

        let _ = Parallel.For(0, cRow, new Action<int>(rowTask))
        c
    
    let n = 10  // we use n*n matrices
    let unitMatrix = Array2D.create n n 0.0
    for i = 0 to n - 1 do
        unitMatrix.[i,i] <- 1.0
    // display what should be a n by n unit matrix
    let result = matrixMultiply unitMatrix unitMatrix
    printfn "%A" result
