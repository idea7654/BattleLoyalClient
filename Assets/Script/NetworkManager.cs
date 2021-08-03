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
using UnityEngine.SceneManagement;
[Serializable]
public class NetworkManager : MonoBehaviour
{
    #region SOCKET_SET
    byte[] recvByte = new byte[512];
    Thread ServerCheck_thread;
    Queue<Message> Buffer = new Queue<Message>();
    //Queue<string> Buffer_Connection = new Queue<string>();
    string strIP = "203.250.133.43";
    int PacketNumber = 1;

    //int port = 9999;
    Socket sock;
    IPAddress ip;
    IPEndPoint endPoint;
    EndPoint remoteEP;
    //IPEndPoint serverEP;
    IPEndPoint bindEP;
    IPEndPoint CsServerEP;
    object buffer_lock = new object(); //queue�浹 ������ lock
    public WritePacket WritePacketManager;
    #endregion

    public GameObject MyCharacter;

    void Start()
    {
        serverOn();
        StartCoroutine(buffer_update());
        WritePacketManager = new WritePacket();
    }

    void serverOn()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = IPAddress.Parse(strIP);
        endPoint = new IPEndPoint(IPAddress.Any, 0);
        bindEP = new IPEndPoint(IPAddress.Any, 0);
        CsServerEP = new IPEndPoint(ip, 9999);
        sock.Bind(bindEP);
        remoteEP = (EndPoint)endPoint;
        ServerCheck_thread = new Thread(ServerCheck);
        ServerCheck_thread.Start();

        //SendToReliable();
        //SendSample();
    }

    void ServerCheck()
    {
        sock.BeginReceiveFrom(recvByte, 0, recvByte.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(RecvCallBack), recvByte);

        void RecvCallBack(IAsyncResult result)
        {
            try
            {
                int size = sock.EndReceiveFrom(result, ref remoteEP);
                Debug.Log(size);
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
                    { //ť �浹����
                        Buffer.Enqueue(message); //ť�� ���� ����
                    }
                    System.Array.Clear(recvByte, 0, recvByte.Length); //���۸� ����� �ʱ�ȭ
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
        switch(bufferType)
        {
            case MESSAGE_ID.S2C_MOVE:
                var packet = message.Packet<S2C_MOVE>().Value;
                break;
            //SendToReliable();
            case MESSAGE_ID.S2C_LOGIN_ERROR:
                GameObject.Find("Alert").transform.GetChild(0).gameObject.SetActive(true);
                break;
            case MESSAGE_ID.S2C_COMPLETE_LOGIN:
                {
                    DontDestroyOnLoad(GameObject.Find("NetworkManager").gameObject);
                    var packetValue = message.Packet<S2C_COMPLETE_LOGIN>().Value;
                    SceneManager.LoadScene("LobbyScene");
                    GameObject obj = Instantiate(MyCharacter, new Vector3(-2.7f, 0.2f, -32f), Quaternion.Euler(new Vector3(0, 0, 0)));
                    obj.name = packetValue.Nickname;
                    obj.GetComponent<TPSCharacterController>().enabled = false;
                    DontDestroyOnLoad(obj);
                    //���⼭ ����ȯ + �������� ���� ������(�켱�� �κ񼭹� -> ������ ������ �Ѿ �����̱⿡ �̰� �κ񿡼� ���� ��
                    break;
                }
            case MESSAGE_ID.S2C_COMPLETE_REGISTER:
                //���⼱ �α��� �������� ���ư�����..
                break;
            case MESSAGE_ID.S2C_REGISTER_ERROR:
                GameObject.Find("Alert").transform.GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    void SendToReliable()
    {
        int Ack = 9999;
        byte[] AckByte = BitConverter.GetBytes(Ack);
        sock.SendTo(AckByte, AckByte.Length, SocketFlags.None, CsServerEP);
    }

    public void SendPacket(byte[] SendData)
    {
        int PacketLength = 0;

        PacketLength = SendData.Length + sizeof(int) * 2;
        byte[] PLByte = BitConverter.GetBytes(PacketLength);
        byte[] PNByte = BitConverter.GetBytes(PacketNumber);

        byte[] newArray = new byte[PacketLength];

        Array.Copy(PLByte, 0, newArray, 0, PLByte.Length);
        Array.Copy(PNByte, 0, newArray, PLByte.Length, PNByte.Length);
        Array.Copy(SendData, 0, newArray, PLByte.Length + PNByte.Length, SendData.Length);

        int result = sock.SendTo(newArray, newArray.Length, SocketFlags.None, CsServerEP);
        Debug.Log(result);
    }
}

public class WritePacket
{
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

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }

    public byte[] WRITE_PU_C2S_REQUEST_LOGIN(String email, String password)
    {
        var userEmail = builder.CreateString(email);
        var userPassword = builder.CreateString(password);

        C2S_REQUEST_LOGIN.StartC2S_REQUEST_LOGIN(builder);
        C2S_REQUEST_LOGIN.AddEmail(builder, userEmail);
        C2S_REQUEST_LOGIN.AddPassword(builder, userPassword);
        var packet = C2S_REQUEST_LOGIN.EndC2S_REQUEST_LOGIN(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_REQUEST_LOGIN, packet.Value);
        builder.Finish(message.Value);

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }

    public byte[] WRITE_PU_C2S_REQUEST_REGISTER(String email, String nickname, String password)
    {
        var userEmail = builder.CreateString(email);
        var userNick = builder.CreateString(nickname);
        var userPassword = builder.CreateString(password);

        C2S_REQUEST_REGISTER.StartC2S_REQUEST_REGISTER(builder);
        C2S_REQUEST_REGISTER.AddEmail(builder, userEmail);
        C2S_REQUEST_REGISTER.AddNickname(builder, userNick);
        C2S_REQUEST_REGISTER.AddPassword(builder, userPassword);
        var packet = C2S_REQUEST_REGISTER.EndC2S_REQUEST_REGISTER(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_REQUEST_REGISTER, packet.Value);
        builder.Finish(message.Value);

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }
}


class ReadPacket
{

}
