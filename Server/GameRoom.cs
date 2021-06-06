using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        private List<ClientSession> _sessions = new List<ClientSession>();
        private JobQueue _jobQueue = new JobQueue();
        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }

        public void Flush()
        {
            foreach (var session in _sessions)
            {
                session.Send(_pendingList);
            }

            Console.WriteLine($"Flushed {_pendingList.Count} Items");
            _pendingList.Clear();
        }

        public void Broadcast(ClientSession clientSession, string chatPacketChat)
        {
            S_Chat packet = new S_Chat()
            {
                playerId = clientSession.SessionId,
                chat = $"{chatPacketChat} I am {clientSession.SessionId}"
            };
            ArraySegment<byte> segment = packet.Write();
            
            _pendingList.Add(segment);
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
    }
}
