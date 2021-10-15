module Logger

open System;
open Microsoft.Extensions.Logging;

type LoggerScope() =
    do ()
    interface IDisposable with
        member this.Dispose() =
            ()

type ConsoleLogger() =
    do ()
    interface ILogger with
        member this.BeginScope(state: 'TState): IDisposable = 
            new LoggerScope () :> IDisposable

        member this.IsEnabled(logLevel: LogLevel): bool = 
            true

        member this.Log(logLevel: LogLevel, eventId: EventId, state: 'TState, ``exception``: exn, formatter: System.Func<'TState,exn,string>): unit = 
            let message = formatter.Invoke(state, ``exception``)
            Console.WriteLine message
