using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToLoginPage : MonoBehaviour
{
    private Button button;
    public GameObject LoginPage;
    public GameObject RegisterPage;
    // Start is called before the first frame update
    void Start()
    {
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(GotoLogin);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GotoLogin()
    {
        RegisterPage.SetActive(false);
        LoginPage.SetActive(true);
    }
}
