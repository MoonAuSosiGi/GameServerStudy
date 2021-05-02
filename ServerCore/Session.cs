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

        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();        
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // 여러 정보를 넘겨줄 수 있다.
            // recvArgs.UserToken = 1;
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
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
        void RegisterSend()
        {
            // BufferList와 사용하면 에러
            //_sendArgs.SetBuffer(buff, 0, buff.Length);

            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                // ArraySegment은 struct라 stack 영역에 할당됨. 따라서 값이 복사가 된다.
                // 이런 형태로 넣어주면 안됨
                //_sendArgs.BufferList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);

            if(pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        Console.WriteLine($"Transferred bytes : {_sendArgs.BytesTransferred}");

                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnRecvCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);

            if(pending == false)
            {
                OnRecvCompleted(null, _recvArgs);
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
                    RegisterRecv();
                }catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
