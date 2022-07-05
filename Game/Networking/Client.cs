using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Game;

class Client
{
    private NetClient _client;

    private NetworkLogger _logger;

    public Client(MyGameWindow game, int port, string host, string serverName)
    {
        NetPeerConfiguration congig = new(serverName);
      //  congig.AutoFlushSendQueue = false;

        _client = new NetClient(congig);

        _client.Start();
        _client.Connect(host, port);

        _logger = game.sharedData.gameData.logger;
    }

    public void Update()
    {
        NetIncomingMessage message;

        while ((message = _client.ReadMessage()) != null)
        {
            _logger.Log("Recieved message from server", LogType.Info, SenderType.Client);

            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    byte packetType = message.ReadByte();

                    Packet packet;

                    switch (packetType)
                    {
                        default:
                            _logger.Log("Unhandled packet type", LogType.Error, SenderType.Client);
                            break;
                        case (byte)PacketType.LocalPlayer:
                            break;
                        case (byte)PacketType.PlayerDisconnect:
                            break;
                        case (byte)PacketType.Position:
                            break;
                        case (byte)PacketType.Spawn:
                            break;
                    }

                    break;
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.WarningMessage:
                    string text = message.ReadString();
                    _logger.Log(text, LogType.Debug, SenderType.Client);
                    break;

                default:
                    _logger.Log($"Unhandled type: {message.MessageType} with {message.LengthBytes} bytes", LogType.Error, SenderType.Client);
                    break;
            }

    //        _client.Recycle(message);
        }
    }

    public void SendPosition(Vector2 position)
    {
        NetOutgoingMessage message = _client.CreateMessage();

        message.Write((byte)PacketType.Position);
        message.Write(position.X);
        message.Write(position.Y);

        _client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
    }
}
