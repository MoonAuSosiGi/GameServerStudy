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
            // 플레이어 추가하고
            _sessions.Add(session);
            session.Room = this;

            // 신입에게 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach (var s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (session == s),
                    palyerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });   
            }
            session.Send(players.Write());

            // 신입 입장을 모두에게 알림
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.palyerId = session.SessionId;
            enter.posX = 0.0f;
            enter.posY = 0.0f;
            enter.posZ = 0.0f;
            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 제거하고
            _sessions.Remove(session);

            // 모두에게 알림
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;
            // 모두에게 알림
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
            Broadcast(move.Write());
        }

        public void Flush()
        {
            foreach (var session in _sessions)
            {
                session.Send(_pendingList);
            }

            //Console.WriteLine($"Flushed {_pendingList.Count} Items");
            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
    }
}

