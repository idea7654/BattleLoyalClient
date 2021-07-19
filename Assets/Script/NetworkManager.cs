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
    object buffer_lock = new object(); //queue�浹 ������ lock
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

                    //���� ó��
                    //string recvString = Encoding.ASCII.GetString(recvBuffer);
                    //recvString = recvString.Replace("\0", string.Empty);

                    //lock (buffer_lock)
                    //{ //ť �浹����
                    //    Buffer.Enqueue(recvString); //ť�� ���� ����
                    //}
                    //System.Array.Clear(recvByte, 0, recvByte.Length); //���۸� ����� �ʱ�ȭ
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
            yield return null; //�ڷ�ƾ���� �ݺ��� �����հ�����
        }
    }

    void BufferSystem()
    {
        while (Buffer.Count != 0)
        { //ť�� ũ�Ⱑ 0�� �ƴϸ� �۵�, ���� while�� ���ϸ� �����Ӹ��� ���۸� ó���ϴµ�
          //���� ��Ŷ�� ó���� �� ó���Ǵ� �纸�� ���̴� ���� ������ �۵��� ����� �̷����������
            string b = null;
            lock (buffer_lock)
            {
                b = Buffer.Dequeue();
            }
            Debug.Log(b); //���۸� ���
        }
    }
}
