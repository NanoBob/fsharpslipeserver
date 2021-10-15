module Playerlist

open Microsoft.Extensions.Logging;
open SlipeServer.Server;
open SlipeServer.Server.Services;
open SlipeServer.Server.Elements;
open SlipeServer.Server.Elements.Events;
open SlipeServer.Server.Repositories;
open System.Drawing;

let getPlayerMessage (player: Player) = 
    $"{player.Name} in dimension {player.Dimension}"

let handlePlayerCommand (repository: IElementRepository) (chatbox: ChatBox) (player: Player) (args: PlayerCommandEventArgs) =
    if args.Command = "list" then
        repository.GetByType<Player> ElementType.Player
            |> Seq.map getPlayerMessage
            |> Seq.iter (fun message -> chatbox.OutputTo(player, message, Color.White, true))

let handlePlayerJoin (repository: IElementRepository) (chatbox: ChatBox) (player: Player) = 
    player.add_CommandEntered(fun player args -> handlePlayerCommand repository chatbox player args)

let public init (server: MtaServer) =
    let chatbox = server.GetRequiredService<ChatBox> ()
    let logger = server.GetRequiredService<ILogger> ()
    let repository = server.GetRequiredService<IElementRepository> ()

    server.add_PlayerJoined(fun player -> handlePlayerJoin repository chatbox player)
    logger.LogInformation "Playerlist initialised"
    server
