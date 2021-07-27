using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestLogin : MonoBehaviour
{
    public Text Email;
    public Text Password;
    private Button Button;
    NetworkManager networkManager;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        Button = transform.GetComponent<Button>();
        Button.onClick.AddListener(getLogin);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void getLogin()
    {
        string email = Email.text;
        string password = Password.text;
        byte[] sendData = networkManager.WritePacketManager.WRITE_PU_C2S_REQUEST_LOGIN(email, password);
        networkManager.SendPacket(sendData);
        //여기서 패킷 만들어서 보내면됨
       // networkManager
    }
}
