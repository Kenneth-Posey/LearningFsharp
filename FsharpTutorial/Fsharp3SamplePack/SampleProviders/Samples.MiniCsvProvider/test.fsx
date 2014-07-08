#r @".\bin\Debug\Samples.MiniCsvProvider.dll"

let csv = new Samples.FSharp.MiniCsvProvider.MiniCsv<"test.csv">()

let row1 = csv.Data |> Seq.head 

let distance = row1.Distance
let time = row1.Time // try doing "Go To Definition" on Time property
