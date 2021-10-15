module Vehicles

open Microsoft.Extensions.Logging;
open SlipeServer.Server;
open SlipeServer.Server.Services;
open SlipeServer.Server.Elements;
open SlipeServer.Server.Elements.Events;
open SlipeServer.Packets.Enums;
open System.Drawing
open System.Numerics

let colorVehicle (color: Color) (vehicle: Vehicle) =
    vehicle.Colors.Primary <- color
    vehicle.Colors.Secondary <- color
    vehicle.Colors.Color3 <- color
    vehicle.Colors.Color4 <- color
    vehicle

let setVehicleDriver (driver: Player) (vehicle: Vehicle) =
    vehicle.Driver <- driver

let associateVehicle (server: MtaServer) (vehicle: Vehicle) =
    vehicle.AssociateWith(server)

let handlePlayerCommand (server: MtaServer) (logger: ILogger) (chatbox: ChatBox) (player: Player) (args: PlayerCommandEventArgs) =
    if args.Command = "spawnvehicle" then
        let model = args.Arguments.[0] |> uint16
        let enumModel: VehicleModel = LanguagePrimitives.EnumOfValue (model |> int)

        Vehicle(model, player.Position) |> associateVehicle server |> colorVehicle Color.HotPink |> setVehicleDriver player

        chatbox.OutputTo(player, $"You spawned a {enumModel}", Color.White, false, ChatEchoType.Action)
        logger.LogInformation($"{player.Name} spawned a vehicle ({model})")

let handlePlayerJoin (server: MtaServer) (logger: ILogger) (chatbox: ChatBox) (player: Player) = 
    player.add_CommandEntered(fun player args -> handlePlayerCommand server logger chatbox player args)

let createTestVehicles (server: MtaServer) =
    Vehicle(VehicleModel.Alpha |> uint16, Vector3(0f, 0f, 3f)).AssociateWith(server) |> colorVehicle Color.Purple |> ignore

let public init (server: MtaServer) =
    let chatbox = server.GetRequiredService<ChatBox>()
    let logger = server.GetRequiredService<ILogger>();

    createTestVehicles server

    server.add_PlayerJoined(fun player -> handlePlayerJoin server logger chatbox player)
    logger.LogInformation "Vehicles initialised"
    server
