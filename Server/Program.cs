using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Knight
    {
        public int hp;
        public int attack;
        public string name = "";
        public List<int> skills = new List<int>();
    }
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Knight knight = new Knight() { hp = 100, attack = 10 };

            ArraySegment<byte>? openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer, 0, openSegment.Value.Array, openSegment.Value.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Value.Array, openSegment.Value.Offset + buffer.Length, buffer2.Length);

            // 실제로 보내야하는 버퍼 
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            // 전송             
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
    internal class Program
    {
        static Listener _listener = new Listener();

        public static void Main(string[] args)
        {
            // DNS 사용 (Domain Name System) 
            // 텍스트 주소를 ip 주소로 변환해주는 서버
            string host = Dns.GetHostName(); // 로컬 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            // 최종 주소와 포트번호 
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening ..");

            while (true)
            {
            }
        }
    }
}