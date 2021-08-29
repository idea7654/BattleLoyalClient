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
        GetComponent<Text>().text = "남은 인원: " + userLength.ToString() + "명";
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
