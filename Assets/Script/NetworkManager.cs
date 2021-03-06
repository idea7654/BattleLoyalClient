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
    object buffer_lock = new object(); //queue충돌 방지용 lock
    public WritePacket WritePacketManager;
    #endregion

    public GameObject MyCharacter;
    public GameObject OtherCharacter;
    public string MyNick;

    public GameObject NormalGun;
    public List<GameObject> GunList = new List<GameObject>();
    public List<GameObject> SessionGuns = new List<GameObject>();

    public int userLength = 0;

    void Awake()
    {
        GunList.Add(NormalGun);
    }

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
        switch(bufferType)
        {
            case MESSAGE_ID.S2C_MOVE:
                {
                    var packet = message.Packet<S2C_MOVE>().Value;
                    var nickname = packet.NickName;
                    var userPos = packet.Pos;
                    float userDir = packet.Dir;
                    int moveDir = packet.Movedir;
                    GameObject player = GameObject.Find(nickname);
                    player.GetComponent<CharacterController>().enabled = false;
                    player.transform.position = new Vector3(userPos.Value.X, userPos.Value.Y, userPos.Value.Z);
                    player.transform.rotation = Quaternion.Euler(new Vector3(0, userDir, 0));
                    player.GetComponent<CharacterController>().enabled = true;
                    if(MyNick != nickname)
                    {
                        player.GetComponent<OtherUserController>().moveDir = moveDir;
                    }
                    //여기 고쳐야됨!!
                }
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
                    GameObject obj = Instantiate(MyCharacter, new Vector3(-2.7f, 0.2f, -32f), Quaternion.Euler(new Vector3(0, 180f, 0)));
                    obj.name = packetValue.Nickname;
                    MyNick = packetValue.Nickname;
                    obj.GetComponent<TPSCharacterController>().enabled = false;
                    DontDestroyOnLoad(obj);
                    StartCoroutine(ResetSessionTime(MyNick));
                    int Ack = 8888;
                    byte[] AckByte = BitConverter.GetBytes(Ack);
                    sock.SendTo(AckByte, AckByte.Length, SocketFlags.None, CsServerEP);
                    //여기서 씬전환 + 유저정보 바탕 프리팹(우선은 로비서버 -> 컨텐츠 서버로 넘어갈 예정이기에 이건 로비에서 사용될 것
                    break;
                }
            case MESSAGE_ID.S2C_COMPLETE_REGISTER:
                //여기선 로그인 페이지로 돌아가도록..
                break;
            case MESSAGE_ID.S2C_REGISTER_ERROR:
                GameObject.Find("Alert").transform.GetChild(0).gameObject.SetActive(true);
                break;
            case MESSAGE_ID.S2C_GAME_START:
                {
                    SceneManager.LoadScene("SampleScene");
                    var packetGS = message.Packet<S2C_GAME_START>().Value;
                    userLength = packetGS.UserdataLength;
                    int gunLength = packetGS.GundataLength;
                    GameObject.Find(MyNick).GetComponent<TPSCharacterController>().enabled = true;
                    
                    for (int i = 0; i < userLength; i++)
                    {
                        if(packetGS.Userdata(i).Value.Nickname == MyNick)
                        {
                            GameObject MyChara = GameObject.Find(MyNick);
                            MyChara.GetComponent<CharacterController>().enabled = false;
                            MyChara.transform.position = new Vector3(packetGS.Userdata(i).Value.Pos.Value.X, packetGS.Userdata(i).Value.Pos.Value.Y, packetGS.Userdata(i).Value.Pos.Value.Z);
                            MyChara.GetComponent<CharacterController>().enabled = true;
                        }
                        else
                        {
                            GameObject otherPlayer = Instantiate(OtherCharacter, new Vector3(packetGS.Userdata(i).Value.Pos.Value.X, packetGS.Userdata(i).Value.Pos.Value.Y, packetGS.Userdata(i).Value.Pos.Value.Z), Quaternion.Euler(new Vector3(0, 0, 0)));
                            otherPlayer.name = packetGS.Userdata(i).Value.Nickname;
                            DontDestroyOnLoad(otherPlayer);
                        }
                    }

                    for(int i = 0; i < gunLength; i++)
                    {
                        switch(packetGS.Gundata(i).Value.Type)
                        {
                            case 0:
                                GameObject gun = Instantiate(GunList[i], new Vector3(packetGS.Gundata(i).Value.Pos.Value.X, packetGS.Gundata(i).Value.Pos.Value.Y, packetGS.Gundata(i).Value.Pos.Value.Z), Quaternion.Euler(new Vector3(0, 0, 0)));
                                gun.transform.GetChild(0).GetComponent<Gun>().gunNum = i;
                                gun.name = i.ToString();
                                DontDestroyOnLoad(gun);
                                SessionGuns.Add(gun);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                }
            case MESSAGE_ID.S2C_PICKUP_GUN:
                {
                    var packetGS = message.Packet<S2C_PICKUP_GUN>().Value;
                    GameObject targetPlayer = GameObject.Find(packetGS.Nickname);
                    targetPlayer.GetComponent<GetGun>().PickGun(packetGS.Gunnum);
                    //gun.GetComponent<LateUpdatedFollow>().TargetToFollow = targetPlayer.transform;
                    //이 다음에 해당 플레이어한테 총 붙이고, 처리..
                    break;
                }
            case MESSAGE_ID.S2C_USER_NOT_FOUND:
                {
                    var packetGS = message.Packet<S2C_USER_NOT_FOUND>().Value;
                    string packetMessage = packetGS.Message;
                    Debug.Log(packetMessage);
                    break;
                }
            case MESSAGE_ID.S2C_USER_DISCONNECT:
                {
                    var packetGS = message.Packet<S2C_USER_DISCONNECT>().Value;
                    //Debug.Log(packetGS.Nickname + " disconnect");
                    GameObject.Find("People").GetComponent<SetGameInfo>().SetText("남은 인원: " + (userLength - 1).ToString() + "명");
                    Destroy(GameObject.Find(packetGS.Nickname));
                    break;
                }
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
    }

    IEnumerator ResetSessionTime(String nickname)
    {
        while(true)
        {
            var makePacket = WritePacketManager.WRITE_PU_C2S_EXTEND_SESSION(nickname);
            SendPacket(makePacket);
            yield return new WaitForSeconds(1.0f);
        }
    }
}

public class WritePacket
{
    FlatBuffers.FlatBufferBuilder builder = new FlatBufferBuilder(1024);

    public byte[] WRITE_PU_C2S_MOVE(String nickname, Vector3 pos, float angleY, int moveDir)
    {
        var playername = builder.CreateString(nickname);
        var playerPos = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);

        C2S_MOVE.StartC2S_MOVE(builder);
        C2S_MOVE.AddNickName(builder, playername);
        C2S_MOVE.AddPos(builder, Vec3.CreateVec3(builder, pos.x, pos.y, pos.z));
        C2S_MOVE.AddDir(builder, angleY);
        C2S_MOVE.AddMovedir(builder, moveDir);
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

    public byte[] WRITE_PU_C2S_START_MATCHING(String nickname)
    {
        var nickName = builder.CreateString(nickname);

        C2S_START_MATCHING.StartC2S_START_MATCHING(builder);
        C2S_START_MATCHING.AddNickname(builder, nickName);
        var packet = C2S_START_MATCHING.EndC2S_START_MATCHING(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_START_MATCHING, packet.Value);
        builder.Finish(message.Value);

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }

    public byte[] WRITE_PU_C2S_CANCEL_MATCHING(String nickname)
    {
        var nickName = builder.CreateString(nickname);

        C2S_CANCEL_MATCHING.StartC2S_CANCEL_MATCHING(builder);
        C2S_CANCEL_MATCHING.AddNickname(builder, nickName);
        var packet = C2S_CANCEL_MATCHING.EndC2S_CANCEL_MATCHING(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_CANCEL_MATCHING, packet.Value);
        builder.Finish(message.Value);

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }

    public byte[] WRITE_PU_C2S_PICKUP_GUN(String nickname, int gunNum)
    {
        var nickName = builder.CreateString(nickname);

        C2S_PICKUP_GUN.StartC2S_PICKUP_GUN(builder);
        C2S_PICKUP_GUN.AddNickname(builder, nickName);
        C2S_PICKUP_GUN.AddGunnum(builder, gunNum);
        var packet = C2S_PICKUP_GUN.EndC2S_PICKUP_GUN(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_PICKUP_GUN, packet.Value);
        builder.Finish(message.Value);

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }

    public byte[] WRITE_PU_C2S_EXTEND_SESSION(String nickname)
    {
        var nickName = builder.CreateString(nickname);

        C2S_EXTEND_SESSION.StartC2S_EXTEND_SESSION(builder);
        C2S_EXTEND_SESSION.AddNickName(builder, nickName);
        var packet = C2S_EXTEND_SESSION.EndC2S_EXTEND_SESSION(builder);
        builder.Finish(packet.Value);

        var message = Message.CreateMessage(builder, MESSAGE_ID.C2S_EXTEND_SESSION, packet.Value);
        builder.Finish(message.Value);

        byte[] returnBuf = builder.SizedByteArray();
        builder.Clear();
        return returnBuf;
    }
}


class ReadPacket
{

}
