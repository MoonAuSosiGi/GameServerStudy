using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            _listenSocket.Bind(endPoint);

            // backlog : 최대 대기수 10으로 지정 
            _listenSocket.Listen(10);

            // non blocking으로 받기 위해 관련 정보 등록
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            // non blocking 처리 시작!
            RegisterAccepet(args);
        }

        void RegisterAccepet(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);

            // 보류 없이 완료
            if(pending == false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 에러없이 처리가 되었다.
            if(args.SocketError == SocketError.Success)
            {
                // @todo 
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            // 다음 Accept을 위해 재호출
            RegisterAccepet(args);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
