using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using FlatBuffers;

[Serializable]
public class NetworkManager : MonoBehaviour
{
    #region SOCKET_SET
    byte[] recvByte = new byte[512];
    Thread ServerCheck_thread;
    Queue<Message> Buffer = new Queue<Message>();
    //Queue<string> Buffer_Connection = new Queue<string>();
    string strIP = "127.0.0.1";

    //int port = 9999;
    Socket sock;
    IPAddress ip;
    IPEndPoint endPoint;
    EndPoint remoteEP;
    //IPEndPoint serverEP;
    IPEndPoint bindEP;
    IPEndPoint CsServerEP;
    object buffer_lock = new object(); //queue충돌 방지용 lock
    #endregion

    void Start()
    {
        serverOn();
        StartCoroutine(buffer_update());
    }

    void serverOn()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = IPAddress.Parse(strIP);
        endPoint = new IPEndPoint(IPAddress.Any, 0);
        bindEP = new IPEndPoint(ip, 7777);
        CsServerEP = new IPEndPoint(ip, 9999);
        sock.Bind(bindEP);
        remoteEP = (EndPoint)endPoint;
        ServerCheck_thread = new Thread(ServerCheck);
        ServerCheck_thread.Start();
    }

    void ServerCheck()
    {
        sock.BeginReceiveFrom(recvByte, 0, recvByte.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(RecvCallBack), recvByte);

        void RecvCallBack(IAsyncResult result)
        {
            try
            {
                int size = sock.EndReceiveFrom(result, ref remoteEP);
                if (size > 0)
                {
                    recvByte = (byte[])result.AsyncState;

                    int packetLength = BitConverter.ToInt32(recvByte, 0);
                    int packetNum = BitConverter.ToInt32(recvByte, 4);
                    
                    byte[] flatBuffer = new byte[packetLength - 8];
                    Array.Copy(recvByte, 8, flatBuffer, 0, packetLength - 8);
                    var newByteBuffer = new ByteBuffer(flatBuffer);
                    
                    Message message = Message.GetRootAsMessage(newByteBuffer);

                    lock (buffer_lock)
                    { //큐 충돌방지
                        Buffer.Enqueue(message); //큐에 버퍼 저장
                    }
                    System.Array.Clear(recvByte, 0, recvByte.Length); //버퍼를 사용후 초기화
                }
                sock.BeginReceiveFrom(recvByte, 0, recvByte.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(RecvCallBack), recvByte);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    IEnumerator buffer_update()
    {
        while (true)
        {
            BufferSystem();
            yield return null; //코루틴에서 반복문 쓸수잇게해줌
        }
    }

    void BufferSystem()
    {
        while (Buffer.Count != 0)
        { //큐의 크기가 0이 아니면 작동, 만약 while을 안하면 프레임마다 버퍼를 처리하는데
          //많은 패킷을 처리할 땐 처리되는 양보다 쌓이는 양이 많아져 작동이 제대로 이루어지지않음
            Message message;
            lock (buffer_lock)
            {
                message = Buffer.Dequeue();
            }

            MESSAGE_ID bufferType = message.PacketType;

            AnalyzePacket(bufferType, ref message);
        }
    }

    void AnalyzePacket(MESSAGE_ID bufferType, ref Message message)
    {
        if (bufferType == MESSAGE_ID.S2C_MOVE)
        {
            var packet = message.Packet<S2C_MOVE>().Value;
            //SendToReliable();
            while(true)
            {
                SendSample();
            }
        }
    }

    void SendToReliable()
    {
        int Ack = 9999;
        byte[] AckByte = BitConverter.GetBytes(Ack);
        sock.SendTo(AckByte, AckByte.Length, SocketFlags.None, CsServerEP);
    }

    FlatBuffers.FlatBufferBuilder builder = new FlatBufferBuilder(1024);

    public byte[] WRITE_PU_C2S_MOVE(String nickname, Vector3 pos, Vector3 dir)
    {
        var playername = builder.CreateString(nickname);
        var playerPos = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
        var playerDir = Vec3.CreateVec3(builder, dir.x, dir.y, dir.z);

        C2S_MOVE.StartC2S_MOVE(builder);
        C2S_MOVE.AddNickName(builder, playername);
        C2S_MOVE.AddPos(builder, Vec3.CreateVec3(builder, pos.x, pos.y, pos.z));
        C2S_MOVE.AddDir(builder, Vec3.CreateVec3(builder, dir.x, dir.y, dir.z));
        var packet = C2S_MOVE.EndC2S_MOVE(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_MOVE, packet.Value);
        builder.Finish(message.Value);

        Message messageA = Message.GetRootAsMessage(builder.DataBuffer);
        MESSAGE_ID bufferType = messageA.PacketType;

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }

    void SendSample()
    {
        int PacketLength = 0;
        int PacketNumber = 1;

        byte[] data = WRITE_PU_C2S_MOVE("Edea", new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f));
        PacketLength = data.Length + sizeof(int) * 2;
        byte[] PLByte = BitConverter.GetBytes(PacketLength);
        byte[] PNByte = BitConverter.GetBytes(PacketNumber);

        byte[] newArray = new byte[PacketLength];

        Array.Copy(PLByte, 0, newArray, 0, PLByte.Length);
        Array.Copy(PNByte, 0, newArray, PLByte.Length, PNByte.Length);
        Array.Copy(data, 0, newArray, PLByte.Length + PNByte.Length, data.Length);

        int a = sock.SendTo(newArray, newArray.Length, SocketFlags.None, CsServerEP);

        Debug.Log(a);
    }
}

class WritePacket
{
    
}

class ReadPacket
{

}