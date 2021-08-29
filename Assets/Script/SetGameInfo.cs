using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGameInfo : MonoBehaviour
{
    // Start is called before the first frame update
    private int userLength = 0;

    void Start()
    {
        userLength = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().userLength;
        GetComponent<Text>().text = "���� �ο�: " + userLength.ToString() + "��";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string message)
    {
        GetComponent<Text>().text = message;
    }
}
