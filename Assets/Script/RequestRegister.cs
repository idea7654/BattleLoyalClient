using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestRegister : MonoBehaviour
{
    public Text Email;
    public Text Nickname;
    public Text Password;
    private Button Button;
    NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        Button = transform.GetComponent<Button>();
        Button.onClick.AddListener(getRegister);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void getRegister()
    {
        string email = Email.text;
        string nickname = Nickname.text;
        string password = Password.text;
        //
    }
}
