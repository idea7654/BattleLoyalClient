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
    Queue<string> Buffer = new Queue<string>();
    Queue<string> Buffer_Connection = new Queue<string>();
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
        //serverEP = new IPEndPoint(ip, port);
        //bindEP = new IPEndPoint(IPAddress.Any, 0);
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
                    byte[] recvBuffer = new byte[512];
                    recvBuffer = (byte[])result.AsyncState;
                    int packetLength = BitConverter.ToInt32(recvBuffer, 0);
                    int packetNum = BitConverter.ToInt32(recvBuffer, 4);
                    
                    byte[] flatBuffer = new byte[packetLength - 8];
                    Array.Copy(recvBuffer, 8, flatBuffer, 0, packetLength - 8);
                    //FlatBufferBuilder builder = new FlatBufferBuilder(1);
                    //Message.StartMessage(builder);
                    var newByteBuffer = new ByteBuffer(flatBuffer);
                    
                    Message message = Message.GetRootAsMessage(newByteBuffer);
                    MESSAGE_ID bufferType = message.PacketType;

                    if(bufferType == MESSAGE_ID.S2C_MOVE)
                    {
                        var packet = message.Packet<S2C_MOVE>().Value;
                        Debug.Log(packet.NickName);
                    }

                    //로직 처리
                    //string recvString = Encoding.ASCII.GetString(recvBuffer);
                    //recvString = recvString.Replace("\0", string.Empty);

                    //lock (buffer_lock)
                    //{ //큐 충돌방지
                    //    Buffer.Enqueue(recvString); //큐에 버퍼 저장
                    //}
                    //System.Array.Clear(recvByte, 0, recvByte.Length); //버퍼를 사용후 초기화
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
            string b = null;
            lock (buffer_lock)
            {
                b = Buffer.Dequeue();
            }
            Debug.Log(b); //버퍼를 사용
        }
    }
}
