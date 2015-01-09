﻿namespace ViewModels

open System
open System.Threading
open System.Collections.ObjectModel
open System.Windows
open System.Windows.Input

open FsXaml

open FSharp.ViewModule

[<StructuredFormatDisplay("{X}:{Y}")>]
type Point  = 
    { X: float; Y: float }
    override x.ToString() = sprintf "(%.1f : %.1f)" x.X x.Y
 
type MainViewModel() as me = 
    inherit ViewModelBase()

    let ui = SynchronizationContext.Current

    let trackPositions = me.Factory.Backing(<@ me.TrackPositions @>, true)
    let positions = ObservableCollection<Point>()
    let maxPositions = 20

    let mutable cts = new CancellationTokenSource()
 
    let agent = MailboxProcessor.Start(fun inbox ->
               async {
                    while true do
                        let! pt = inbox.Receive()

                        if me.TrackPositions then
                            // Update our UI
                            do! Async.SwitchToContext ui
                            if positions.Count > maxPositions then positions.RemoveAt 0
                            positions.Add(pt)
               })

    let clear ui = async {
            me.TrackPositions <- false
            while positions.Count > 0 do
                do! Async.Sleep 100
                do! Async.SwitchToContext ui
                positions.RemoveAt(positions.Count - 1)
            me.TrackPositions <- true
        }

    let onCancelClear _ =
        // Force this back on, so we re-track
        me.TrackPositions <- true
        
        // Replace our cancellation token source with a new one so we can re-fire later
        cts.Dispose()
        cts <- new CancellationTokenSource()
        me.Clear.CancellationToken <- cts.Token


    let cancelClear () =
        cts.Cancel()

    let clearCommand = me.Factory.CommandAsync(clear, cts.Token, onCancelClear)
    let cancelClearCommand = me.Factory.CommandSyncChecked(cancelClear, (fun _ -> me.OperationExecuting), [ <@@ me.OperationExecuting @@> ])
    
    member this.MoveAgent = agent
    member this.Positions = positions
    member this.TrackPositions with get() = trackPositions.Value and set(v) = trackPositions.Value <- v
    member this.Clear : IAsyncNotifyCommand = clearCommand
    member this.CancelClear = cancelClearCommand
