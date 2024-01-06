using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum SPacketId
{
    STATUS=0,
    #region Server
    ENABLE = 1,//no data
    DISABLE=2,
    #endregion
    #region Client
    ENABLED = 100+2,//1 byte yes or no
    DATA = 100 + 3,//1 byte yes or no
    #endregion
}
public class SPacket:IDisposable
{
    public SPacketId id;
    List<byte> data=new List<byte>();
    byte[] bytes;
    int readIndex = 0;// only for incoming packets
    public int size = 0;
    public SPacket(SPacketId id)
    {
        this.id = id;
    }
    public SPacket(byte[] bytes)
    {
        id =(SPacketId) BitConverter.ToUInt32(bytes, 0);
        size = (int)BitConverter.ToUInt32(bytes, 4);
        readIndex = 0;
        this.bytes = new byte[size];
        Array.Copy(bytes, 8, this.bytes, 0, size);
    }
    public static int ToRead(byte[] buff,int index,int count)
    {
        if (count < 8) return 10000;
        int size = (int)BitConverter.ToUInt32(buff, index+4);
        return size+8;
    }
    public byte[] ToBytes()
    {
        byte[] bval = BitConverter.GetBytes((UInt32)size);
        data.InsertRange(0, bval);
        bval = BitConverter.GetBytes((UInt32)id);
        data.InsertRange(0, bval);
        bytes = data.ToArray();
        return bytes;
    }
    public void Write(float val)
    {
        byte[] bval= BitConverter.GetBytes(val);
        data.InsertRange(data.Count, bval);
        size += 4;
    }
    public void Write(int val)
    {
        byte[] bval = BitConverter.GetBytes(val);
        data.InsertRange(data.Count, bval);
        size += 4;
    }
    public float ReadFloat()
    {
        float f = BitConverter.ToSingle(bytes, readIndex);
        readIndex += 4;
        return f;
    }
    public int ReadInt32()
    {
        int i = BitConverter.ToInt32(bytes, readIndex);
        readIndex += 4;
        return i;
    }
    public void Dispose()
    {
        data.Clear();
    }
}
