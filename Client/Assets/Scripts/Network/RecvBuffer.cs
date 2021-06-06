using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {   
        // [rw][][][][][][][][][]
        // [r][ ][ ][ ][ ][w][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][r][ ][ ][ ][ ]
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        /// <summary>
        /// 유효 범위 -> 얼마나 데이터가 쌓여있는지
        /// writePos - ReadPos
        /// </summary>
        public int DataSize { get { return _writePos - _readPos; } }
        
        /// <summary>
        /// 버퍼에 남은 공간
        /// </summary>
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clear()
        {
            int dataSize = DataSize;

            // 클라에서 보낸 데이터를 모두 처리한 상태
            if (dataSize == 0)
            {
                // 남은 데이터가 없으므로 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 데이터가 있다면 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
            {
                return false;
            }
            _readPos += numOfBytes;
            return true;
        }

        // 클라에서 데이터를 쏴서 Recv를 할 때
        public bool OnWrite(int numOfBytes)
        {
            if(numOfBytes > FreeSize)
            {
                return false;
            }
            _writePos += numOfBytes;
            return true;
        }
    }
}
