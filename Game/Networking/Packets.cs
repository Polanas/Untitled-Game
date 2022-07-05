using System;
using Lidgren.Network;

namespace Game;

enum PacketType
{
    LocalPlayer,
    PlayerDisconnect,
    Position,
    Spawn
}

public abstract class Packet
{
    public abstract void ToNetOutGoingMessage(NetOutgoingMessage message);

    public abstract void ToNetIncomingMessage(NetIncomingMessage message);
}

public class LocalPlayerPacket : Packet
{

    public string playerID;

    public override void ToNetIncomingMessage(NetIncomingMessage message)
    {
        message.Write((byte)PacketType.LocalPlayer);
        message.Write(playerID);

    }

    public override void ToNetOutGoingMessage(NetOutgoingMessage message)
    {
        playerID = message.ReadString();
    }
}

public class PlayerDissconnectPacket : Packet
{

    public string playerID;

    public override void ToNetIncomingMessage(NetIncomingMessage message)
    {
        message.Write((byte)PacketType.PlayerDisconnect);
        message.Write(playerID);

    }

    public override void ToNetOutGoingMessage(NetOutgoingMessage message)
    {
        playerID = message.ReadString();
    }
}

public class PositionPacket : Packet
{

    public string playerID;

    public float X;

    public float Y;

    public override void ToNetIncomingMessage(NetIncomingMessage message)
    {
        message.Write((byte)PacketType.Position);
        message.Write(X);
        message.Write(Y);
        message.Write(playerID);

    }

    public override void ToNetOutGoingMessage(NetOutgoingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        playerID = message.ReadString();
    }
}

public class SpawnPacket : Packet
{

    public string playerID;

    public float X;

    public float Y;

    public override void ToNetIncomingMessage(NetIncomingMessage message)
    {
        message.Write((byte)PacketType.Spawn);
        message.Write(X);
        message.Write(Y);
        message.Write(playerID);

    }

    public override void ToNetOutGoingMessage(NetOutgoingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        playerID = message.ReadString();
    }
}