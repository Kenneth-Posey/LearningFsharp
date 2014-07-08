module Logging

open System

type Log =
        /// Message
        | Log of string
        /// Warning
        | Warning of string
        /// Application message * error message * stacktrace
        | Error of string * string * string 

let mutable log : Log -> unit = 

    let getColor = function
        | Log _ -> ConsoleColor.Green
        | Warning _ -> ConsoleColor.Yellow
        | Error _ -> ConsoleColor.Red

    let agent = 
        MailboxProcessor.Start(fun (inbox:MailboxProcessor<Log>) ->
            async {
                while true do
                   let! msg = inbox.Receive()
                   let color = getColor msg
                   let text = 
                       match msg with
                       | Log(s) -> "Message: " + s
                       | Warning(s) -> "Warning: " + s
                       | Error(s,e,_) -> "Error: " + s + " " + e 
                   Console.ForegroundColor <- color
                   Console.WriteLine(text)
                   Console.ResetColor()
            } )
    fun msg -> agent.Post(msg)