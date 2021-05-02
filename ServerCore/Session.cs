using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // 여러 정보를 넘겨줄 수 있다.
            // recvArgs.UserToken = 1;

            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        // 한번만 해야한다.
        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected, 1) == 1)
            {
                return;
            }
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신 
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);

            if(pending == false)
            {
                OnRecvCompleted(null, args);
            }

        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 몇 바이트를 받았는지 체크  0 으로 올 수도 있음 (끊기거나 할 떄)
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) 
            {
                // @todo
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterRecv(args);
                }catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                // @todo Disconnect
            }
        }
        #endregion
    }
}
