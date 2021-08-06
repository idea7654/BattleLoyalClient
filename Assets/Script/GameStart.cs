using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    private Button button;
    NetworkManager networkManager;
    private Text text;
    private bool isMatching = false;
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        text = transform.GetChild(0).transform.GetComponent<Text>();
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(StartMatching);
    }

    private void StartMatching()
    {
        isMatching = !isMatching;
        if (isMatching)
        {
            Debug.Log("¿€µø");
            text.text = "Matching...";
            byte[] sendData = networkManager.WritePacketManager.WRITE_PU_C2S_START_MATCHING(networkManager.MyNick);
            networkManager.SendPacket(sendData);
        }
        else //Matching cancel
        {
            text.text = "Game Start!";
            byte[] sendData = networkManager.WritePacketManager.WRITE_PU_C2S_CANCEL_MATCHING(networkManager.MyNick);
            networkManager.SendPacket(sendData);
        }
    }
}
