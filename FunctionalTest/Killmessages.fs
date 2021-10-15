module Killmessages

open Microsoft.Extensions.Logging;
open SlipeServer.Server;
open SlipeServer.Server.Services;
open SlipeServer.Server.Elements;
open SlipeServer.Server.Elements.Events;

let getDeathMessage (player: Player) (args: PlayerWastedEventArgs): string =
    if isNull args.Killer then
        $"{player.Name} has died.";        
    elif args.Killer :? Player then 
        $"{player.Name} has been killed by {(args.Killer :?> Player).Name}.";
    elif args.Killer :? Vehicle && isNull (args.Killer :?> Vehicle).Driver then
        $"{player.Name} has been run over.";
    else
        $"{player.Name} has been killed by {((args.Killer :?> Vehicle).Driver :?> Player).Name}.";

let handlePlayerDeath (logger: ILogger) (chatbox: ChatBox) (player: Player) (args: PlayerWastedEventArgs) =
    let message = getDeathMessage player args
    chatbox.Output message
    logger.LogInformation message

let handlePlayerJoin (logger: ILogger) (chatbox: ChatBox) (player: Player) = 
    player.add_Wasted(fun sender args -> handlePlayerDeath logger chatbox sender args)

let public init(server: MtaServer) =
    let chatbox = server.GetRequiredService<ChatBox>()
    let logger = server.GetRequiredService<ILogger>();
    server.add_PlayerJoined(fun player -> handlePlayerJoin logger chatbox player)
    logger.LogInformation "Killmessages initialised"
    server
