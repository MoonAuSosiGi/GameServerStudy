using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }
        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte>? s = SendBufferHelper.Open(4096);
            ushort count = 0;
            bool success = true;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Value.Array, s.Value.Offset + count, s.Value.Count - count), packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Value.Array, s.Value.Offset + count, s.Value.Count - count), playerId);
            count += 8;

            // size는 맨 마지막에
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Value.Array, s.Value.Offset, s.Value.Count), count);

            if (success == false)
                return null;

            return SendBufferHelper.Close(count);
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            //ushort packetId = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }
    }


    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            switch((PacketID)packetId)
            {
                case PacketID.PlayerInfoReq:
                {
                    PlayerInfoReq p = new PlayerInfoReq();
                    p.Read(buffer);
                    
                    Console.WriteLine($"PlayerInfoReq : {p.playerId}");
                }
                    break;
            }

            Console.WriteLine($"RecvPacketId: {packetId} {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
