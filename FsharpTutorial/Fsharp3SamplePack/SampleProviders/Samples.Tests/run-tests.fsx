#r "Microsoft.Build.Framework"
#r "Microsoft.Build.Engine"
#r "Microsoft.Build"
#r "Microsoft.Build.Utilities.v4.0"

open System
open System.IO
open System.Diagnostics
open Microsoft.Build

let build (projectFile : string) = 
    let project = Evaluation.Project(projectFile)
    let ok = project.Build([|"Rebuild"|], [BuildEngine.ConsoleLogger() :> Framework.ILogger])
    if not ok then failwithf "Failed to build project %s" projectFile

let exec executable args = 
    let stdOut = ResizeArray()
    let stdErr = ResizeArray()
    let exitCode =
        try
            let psi = ProcessStartInfo(executable, args, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true, UseShellExecute = false)
            let p = Process.Start psi
            p.OutputDataReceived.Add(fun args -> if box args.Data <> null then (stdOut.Add args.Data))
            p.ErrorDataReceived.Add(fun args -> if box args.Data <> null then (stdErr.Add args.Data))
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()
            p.WaitForExit()
            p.ExitCode
        with
            | :? FileNotFoundException
            | :? ComponentModel.Win32Exception -> failwithf "%s tool not found" (Path.GetFileName executable)
    stdOut, exitCode

let runPEVerify path =
    let peverifyTool = Utilities.ToolLocationHelper.GetPathToDotNetFrameworkSdkFile("peverify.exe", Utilities.TargetDotNetFrameworkVersion.VersionLatest)
    let stdOut, _ = exec peverifyTool (sprintf "/NOLOGO /VERBOSE \"%s\"" path)
    let errors = stdOut |> Seq.filter (fun line -> line.IndexOf("Error:") <> -1) |> String.concat Environment.NewLine
    if errors.Length <> 0 then failwithf "PEVerify errors: %s" errors


let toAbsolutePath relativePath = Path.Combine(__SOURCE_DIRECTORY__, relativePath)
try
    let pathToClientBinaries = toAbsolutePath @"Clients\bin\Debug"
    if Directory.Exists pathToClientBinaries then
        printfn "Deleting old client binaries"
        Directory.Delete(pathToClientBinaries, true)

    printfn "Building test type providers"    
    build (toAbsolutePath @"TypeProviders\TypeProviders.fsproj")

    printfn "Building consumers of test type providers"
    build (toAbsolutePath @"Clients\Clients.fsproj")

    let clientsBinary = toAbsolutePath @"Clients\bin\Debug\Clients.exe"
    if not (File.Exists clientsBinary) then failwithf "File %s was not found" clientsBinary

    printfn "Running PEVerify"
    runPEVerify clientsBinary

    printfn "Running tests"
    let stdOut ,exitCode = exec clientsBinary ""
    if exitCode <> 0 then 
        let errors = String.concat Environment.NewLine stdOut
        failwith errors
with
    | e -> 
        let oldColor = Console.ForegroundColor
        Console.ForegroundColor <- ConsoleColor.Red
        Console.WriteLine "Test run failed!!!"
        Console.WriteLine e.Message
        Console.ForegroundColor <- oldColor
