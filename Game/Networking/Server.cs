using Lidgren.Network;

namespace Game;

class Server
{
    private NetServer _server;

    private List<string> _playerIDs;

    private Dictionary<string, Vector2> _playerPositions;

    private NetworkLogger _logger;

    public Server(SharedData sharedData, int port, string gameName)
    {
     //   _playerIDs = new();
    //    _playerPositions = new();

        NetPeerConfiguration config = new("game");
        config.MaximumConnections = 8;
        config.Port = port;

        _server = new(config);
        _server.Start();

        _logger = sharedData.gameData.logger;
        _logger.Log("Listening for clients...", LogType.Info, SenderType.Sever);
    }

    public void Update()
    {
        NetIncomingMessage message;

        while ((message = _server.ReadMessage()) != null)
        {
            _logger.Log($"Message recieved", LogType.Info, SenderType.Sever);
            //  _logger.Log($"Message recieved from {NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier)}", LogType.Info, SenderType.Sever);

          //  List<NetConnection> allConnections = _server.Connections;

            switch (message.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    var status = (NetConnectionStatus)message.ReadByte();
                    //string reason = message.ReadString();

                    if (status == NetConnectionStatus.Connected)
                    {
                       
                    }
                    break;
                case NetIncomingMessageType.Data:
                    var type = message.ReadByte();

                    switch (type)
                    {
                        case (byte)PacketType.Position:
                            float x = message.ReadFloat();
                            float y = message.ReadFloat();

                            _logger.Log($"Position revieved: {x} {y}", LogType.Info, SenderType.Sever);

                            break;
                    }
                    //    case (byte)PacketType.PlayerDisconnect:
                    //        packet = new PlayerDissconnectPacket();
                    //        packet.ToNetIncomingMessage(message);
                    //        SendPlayerDisconnectPacket(allConnections, (PlayerDissconnectPacket)packet);
                    //        break;
                    //    default:
                    //        _logger.Log("Unhandled data / Packet type", LogType.Error, SenderType.Sever);
                    //        break;
                    //}
                    break;
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.WarningMessage:
                    string text = message.ReadString();
                    _logger.Log(text, LogType.Debug, SenderType.Sever);
                    break;

                default:
                    _logger.Log($"Unhandled type: {message.MessageType} with {message.LengthBytes} bytes", LogType.Error, SenderType.Sever);
                    break;
            }

            _server.Recycle(message);
        }
    }

   // public void SendPosition()

    //public void SpawnPlayers(List<NetConnection> allConnections, NetConnection local, string playerID)
    //{
    //    //spawn all clients on the local player
    //    allConnections.ForEach(p =>
    //    {
    //        string playerID1 = NetUtility.ToHexString(p.RemoteUniqueIdentifier);

    //        if (playerID != playerID1)
    //            SendSpawnPacketsToLocal(local, playerID1, _playerPositions[playerID1].X, _playerPositions[playerID1].Y);

    //    });

    //    //spawn the local player on all the clients
    //    SendSpawnPacketsToAll(allConnections, playerID, 150, 0);
    //}

    //public void SendLocalPlayerPacket(NetConnection local, string playerID)
    //{
    //    _logger.Log($"Sending player their user ID: {playerID}", LogType.Info, SenderType.Sever);

    //    NetOutgoingMessage message = _server.CreateMessage();
    //    new LocalPlayerPacket() { playerID = playerID }.ToNetOutGoingMessage(message);
    //    _server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
    //}

    //public void SendSpawnPacketsToLocal(NetConnection local, string playerID, float x, float y)
    //{
    //    _logger.Log($"Sending user spawn info for player {playerID}", LogType.Info, SenderType.Sever);

    //    Vector2 pos = new Vector2(x, y);
    //    _playerPositions[playerID] = pos;

    //    NetOutgoingMessage message = _server.CreateMessage();
    //    new PositionPacket() { playerID = playerID, X = pos.X, Y = pos.Y }.ToNetOutGoingMessage(message);
    //    _server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
    //}

    //public void SendSpawnPacketsToAll(List<NetConnection> allConnections, string playerID, float x, float y)
    //{
    //    _logger.Log($"Sending player spawn info for users {playerID}", LogType.Info, SenderType.Sever);

    //    Vector2 pos = new Vector2(x, y);
    //    _playerPositions[playerID] = pos;

    //    NetOutgoingMessage message = _server.CreateMessage();
    //    new PositionPacket() { playerID = playerID, X = pos.X, Y = pos.Y }.ToNetOutGoingMessage(message);
    //    _server.SendMessage(message, allConnections, NetDeliveryMethod.ReliableOrdered, 0);
    //}

    //public void SendPositionPacket(List<NetConnection> allConnections, PositionPacket packet)
    //{
    //    _logger.Log($"Sending position packet for {packet.playerID}", LogType.Info, SenderType.Sever);

    //    _playerPositions[packet.playerID] = new Vector2(packet.X, packet.Y);

    //    NetOutgoingMessage message = _server.CreateMessage();
    //    packet.ToNetOutGoingMessage(message);
    //    _server.SendMessage(message, allConnections, NetDeliveryMethod.ReliableOrdered, 0);
    //}

    //public void SendPlayerDisconnectPacket(List<NetConnection> allConnections, PlayerDissconnectPacket packet)
    //{
    //    _logger.Log($"Player disconnected: {packet.playerID}", LogType.Info, SenderType.Sever);

    //    _playerPositions.Remove(packet.playerID);
    //    _playerIDs.Remove(packet.playerID);

    //    NetOutgoingMessage message = _server.CreateMessage();
    //    packet.ToNetOutGoingMessage(message);
    //    _server.SendMessage(message, allConnections, NetDeliveryMethod.ReliableOrdered, 0);
    //}
}
