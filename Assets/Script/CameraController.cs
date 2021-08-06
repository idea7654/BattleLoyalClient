using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 0.3f;
    private CinemachineComposer composer;
    private CinemachineVirtualCamera vcam;
    private GameObject TargetObj;
    private NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        composer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>();
        vcam = GetComponent<CinemachineVirtualCamera>();
        //------------������---------------
        //vcam.LookAt = GameObject.Find("MyCharacter1").transform;
        //vcam.Follow = GameObject.Find("Nickname").transform;
        //-----------------------------------
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        TargetObj = GameObject.Find(networkManager.MyNick);
        vcam.LookAt = TargetObj.transform;
        vcam.Follow = TargetObj.transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Mouse Y") * sensitivity;
        //composer.m_TrackedObjectOffset.y += vertical;
        composer.m_TrackedObjectOffset.y = Mathf.Clamp(composer.m_TrackedObjectOffset.y, -10, 10);
    }
}
