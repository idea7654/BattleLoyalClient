using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitAlert : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Target;
    private Button Button;
    void Start()
    {
        Button = transform.GetComponent<Button>();
        Button.onClick.AddListener(AlertExit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AlertExit()
    {
        Target.SetActive(false);
    }
}
