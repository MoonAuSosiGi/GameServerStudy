using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
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

            
            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    // 서버에게 연결 요청
                    socket.Connect(endPoint);
                    Console.WriteLine($"[Connected To {socket.RemoteEndPoint.ToString()}");

                    // 보낸다
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                    int sendBytes = socket.Send(sendBuff);

                    // 받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
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