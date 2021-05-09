using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class PacketHandler
    {
        public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
        {
            PlayerInfoReq p = packet as PlayerInfoReq;

            Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");

            foreach (var skill in p.skills)
            {
                Console.WriteLine($"Skill {skill.id} {skill.level} {skill.duration}");
            }
        }
    }
}
