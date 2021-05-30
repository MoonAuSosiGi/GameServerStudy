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

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
                _sessions.Remove(session);
        }

        public void Broadcast(ClientSession clientSession, string chatPacketChat)
        {
            S_Chat packet = new S_Chat()
            {
                playerId = clientSession.SessionId,
                chat = $"{chatPacketChat} I am {clientSession.SessionId}"
            };
            ArraySegment<byte> segment = packet.Write();
            
            foreach (var session in _sessions)
            {
                session.Send(segment);
            }
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
    }
}
