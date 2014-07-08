module ScriptUsingTypeProvider

#r @"bin\Debug\Samples.Csv.dll"

let x = new Samples.Csv.CsvFile< @"http://ichart.finance.yahoo.com/table.csv?s=MSFT&a=8&b=1&c=2012&d=8&e=7&f=2012">()
let y = x.Data

type t = Samples.Csv.CsvFile< @"https://explore.data.gov/download/pwaj-zn2n/CSV", InferRows = 2, InferTypes=true, IgnoreErrors=true>

t().Data |> Seq.iter (fun x -> printfn "%A" x)


let bankData = new Samples.Csv.CsvFile< @"https://explore.data.gov/download/pwaj-zn2n/CSV", Schema="string,string,string,int,string,date,date", IgnoreErrors=true>()

bankData.Data |> Seq.iter (fun x -> printfn "%A" x)

