using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    private Button button;
    NetworkManager networkManager;

    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(StartMatching);
    }

    private void StartMatching()
    {
        byte[] sendData = networkManager.WritePacketManager.WRITE_PU_C2S_START_MATCHING("Edea");
        networkManager.SendPacket(sendData);
    }
}
