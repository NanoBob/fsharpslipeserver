open System
open System.Threading;
open SlipeServer.Server;
open SlipeServer.Server.ServerOptions;
open Microsoft.Extensions.DependencyInjection;
open Microsoft.Extensions.Logging;

let configureServices (services: ServiceCollection) =
    services.AddSingleton<ILogger, Logger.ConsoleLogger> () |> ignore

let configure (builder: ServerBuilder) = 
    builder.UseConfiguration (Configuration(Port = uint16 30000))
    builder.AddDefaults ()
    configureServices |> builder.ConfigureServices

let start (server: MtaServer) =
    server.Start ()

[<EntryPoint>]
let main args =
    MtaServer configure
        |> Killmessages.init
        |> Spawning.init
        |> Vehicles.init
        |> Playerlist.init
        |> start

    Thread.Sleep -1
    0
