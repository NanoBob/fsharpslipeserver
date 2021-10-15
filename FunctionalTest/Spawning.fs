module Spawning

open Microsoft.Extensions.Logging;
open System.Threading.Tasks;
open System.Numerics;
open SlipeServer.Packets.Lua.Camera;
open SlipeServer.Server;
open SlipeServer.Server.Elements;

let spawnPlayer (player: Player) (position: Vector3) (rotation: float32) (model: int) =
    async {
        player.Camera.Fade(CameraFade.Out, 1000f)
        Task.Delay(1000) |> Async.AwaitTask |> ignore
        player.Spawn(position, rotation, (uint16)model, (byte)0, (uint16)0)
        player.Camera.Fade(CameraFade.In, 1000f)
    } |> Async.StartAsTask |> ignore

let handlePlayerDeath (player: Player) =
    spawnPlayer player (new Vector3(0f, 0f, 3f)) 0f 7

let handlePlayerJoin (player: Player) = 
    player.add_Wasted(fun sender args -> handlePlayerDeath sender)
    player.Camera.Target <- (player :> Element)
    spawnPlayer player (new Vector3(0f, 0f, 3f)) 0f 7

let public init(server: MtaServer) =
    let logger = server.GetRequiredService<ILogger>();
    server.add_PlayerJoined(fun player -> handlePlayerJoin player)
    logger.LogInformation "Spawning logic initialised"
    server
