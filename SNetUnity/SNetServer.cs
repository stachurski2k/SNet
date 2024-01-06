using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
[System.Serializable]
public struct ClientData
{
    public string ip;
    public int port;
    public string name;
}
[RequireComponent(typeof(TaskManager))]
public class SNetServer : MonoBehaviour
{
    [SerializeField] int myPort=8080;
    [SerializeField] ClientData[] clientDatas;
    Dictionary<IPEndPoint, SNetClient> clients=new Dictionary<IPEndPoint, SNetClient>();
    public Dictionary<string, SNetClient> namedClients = new Dictionary<string, SNetClient>();
    IPEndPoint[] endPoints;
    Thread serverThread,statusThread;
    UdpClient socket;
    bool isWorking = false;
    int counter = 0;
    void Awake()
    {
        endPoints = new IPEndPoint[clientDatas.Length];
        for(int i = 0; i < clientDatas.Length; i++)
        {
            IPEndPoint ep= new IPEndPoint(IPAddress.Parse(clientDatas[i].ip), clientDatas[i].port);
            endPoints[i] = ep;
            clients.Add(ep, new SNetClient(ep));
            namedClients.Add(clientDatas[i].name, clients[ep]);
        }
        serverThread = new Thread(new ThreadStart(ServerRecvThread));
      //  statusThread = new Thread(new ThreadStart(ServerStatusThread));

        isWorking = true;
        socket = new UdpClient(myPort);
        serverThread.Start();
        //statusThread.Start();

    }
    void ServerRecvThread()
    {
        IPEndPoint sender=new IPEndPoint(IPAddress.Any,8080);
        while (isWorking)
        {
            byte[] bytes = socket.Receive(ref sender);
            if (clients.ContainsKey(sender))
            {
                clients[sender].ReciveBytes(bytes);
            }
            Thread.Sleep(2);
        }
    }
    //void ServerStatusThread()
    //{
    //    SPacket packet = new SPacket(SPacketId.STATUS);
    //    packet.Write((int)ClientStatus.CONNECTED);
    //    while (isWorking)
    //    {
    //        foreach(var ep in endPoints)
    //        {
    //            clients[ep].CheckStatus();
    //            SendToClient(ep, packet);
    //        }
    //        Thread.Sleep(200);
    //    }
    //}
    public void SendToClient(IPEndPoint ep,SPacket packet)
    {
        try
        {
            byte[] bytes = packet.ToBytes();
            socket.Send(bytes, bytes.Length, ep);
        }
        catch { }
    }
    public void SendToClient(string client, SPacket packet)
    {
        SendToClient(namedClients[client].endPoint,packet);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SPacket packet = new SPacket(SPacketId.DATA);
            packet.Write((float)counter);
            SendToClient(endPoints[0], packet);
            counter += 1;
        }

    }
}
