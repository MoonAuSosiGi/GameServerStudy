using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom
    {
        private List<ClientSession> _sessions = new List<ClientSession>();
        private object _lock = new object();

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }

        public void Broadcast(ClientSession clientSession, string chatPacketChat)
        {
            S_Chat packet = new S_Chat()
            {
                playerId = clientSession.SessionId,
                chat = $"{chatPacketChat} I am {clientSession.SessionId}"
            };
            ArraySegment<byte> segment = packet.Write();

            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    session.Send(segment);
                }
            }
        }
    }
}
