using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private ServerSession _session = new ServerSession();
    void Start()
    {
        // DNS 사용 (Domain Name System) 
        // 텍스트 주소를 ip 주소로 변환해주는 서버
        string host = Dns.GetHostName(); // 로컬 컴퓨터의 호스트 이름
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        // 최종 주소와 포트번호 
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; },
            1);

        StartCoroutine("CoSendPacket");
    }

    private void Update()
    {
        IPacket packet = PacketQueue.Instance.Pop();

        if (packet != null)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            C_Chat chatPacket = new C_Chat();
            chatPacket.chat = "Hello Unity";
            ArraySegment<byte> segment = chatPacket.Write();
            
            _session.Send(segment);
        }
    }
}
