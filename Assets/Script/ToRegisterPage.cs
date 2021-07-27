using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToRegisterPage : MonoBehaviour
{
    private Button button;
    public GameObject LoginPage;
    public GameObject RegisterPage;
    // Start is called before the first frame update
    void Start()
    {
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(GotoRegister);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GotoRegister()
    {
        LoginPage.SetActive(false);
        RegisterPage.SetActive(true);
    }
}
