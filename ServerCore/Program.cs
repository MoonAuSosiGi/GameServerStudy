using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                // 받고
                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Client] {recvData}");

                // 전송 
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                clientSocket.Send(sendBuff);

                // 쫒아내기
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Main(string[] args)
        {
            // DNS 사용 (Domain Name System) 
            // 텍스트 주소를 ip 주소로 변환해주는 서버
            string host = Dns.GetHostName(); // 로컬 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            // 최종 주소와 포트번호 
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening ..");

            while (true)
            {
            }
        }
    }
}
