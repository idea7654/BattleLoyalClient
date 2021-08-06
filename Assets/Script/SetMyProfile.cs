using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMyProfile : MonoBehaviour
{
    NetworkManager networkManager;

    void Start()
    {
        Text text = GetComponent<Text>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        text.text = networkManager.MyNick;
    }
}
