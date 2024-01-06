using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
public enum ClientStatus
{
    UNKNOWN=1,
    CONNECTED=2,
    DISCONNECTED=3
}
public class SNetClient
{
    public IPEndPoint endPoint;
    byte[] recvbuff = new byte[1024];
    int buffSize=0;
    public event Action<SPacket> OnPacketRecived;
    //ClientStatus status = ClientStatus.UNKNOWN;
    //DateTime lastSeen;
    public SNetClient(IPEndPoint ep)
    {
        this.endPoint = ep;
        //lastSeen = DateTime.Now;
        //lastSeen=lastSeen.AddSeconds(-10);
    }
    public void ReciveBytes(byte[] bytes)
    {
        Array.Copy(bytes, 0, recvbuff, buffSize, bytes.Length);
        buffSize += bytes.Length;
        int toRead = SPacket.ToRead(recvbuff, 0, buffSize);
        Debug.Log(toRead);
        if (toRead<=buffSize)
        {
            SPacket packet = new SPacket(recvbuff);
            Debug.Log(packet.id);
            PacketReceived(packet);
            Array.Copy(recvbuff, toRead, recvbuff, 0, recvbuff.Length-toRead);
            buffSize -= toRead;
        }
    }
    void PacketReceived(SPacket packet)
    {
        //if (packet.id == SPacketId.STATUS)
        //{
        //    ClientStatus s =(ClientStatus) packet.ReadInt32();
        //    status = s;
        //    lastSeen = DateTime.Now;
        //    Debug.Log(s);
        //}
        if (OnPacketRecived != null)
        {
            TaskManager.instance.AddTask(()=>OnPacketRecived(packet));
        }
    }
    //public void CheckStatus()
    //{
    //    if (lastSeen.AddSeconds(5) < DateTime.Now)
    //    {
    //        status = ClientStatus.UNKNOWN;
    //    }
    //}
}
