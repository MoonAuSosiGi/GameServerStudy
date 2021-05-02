using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            ArraySegment<byte>? openSegment = SendBufferHelper.Open(4096);
            int length = 0;
            // 보낸다
            for (int i = 0; i < 5; i++)
            {
                byte[] buffer = Encoding.UTF8.GetBytes($"Hello World! {i} ");
                length += buffer.Length;
                Array.Copy(buffer, 0, openSegment.Value.Array, openSegment.Value.Offset + length, buffer.Length);
            }
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(length);
            Send(sendBuff);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);            
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            // DNS 사용 (Domain Name System) 
            // 텍스트 주소를 ip 주소로 변환해주는 서버
            string host = Dns.GetHostName(); // 로컬 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            // 최종 주소와 포트번호 
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new GameSession(); });

            while (true)
            {                
                try
                {
                
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(100);
            }
        }
    }
}