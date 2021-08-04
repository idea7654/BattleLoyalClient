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
    // Start is called before the first frame update
    void Start()
    {
        composer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>();
        vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.LookAt = GameObject.Find("MyCharacter").transform;
        vcam.Follow = GameObject.Find("Nickname").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Mouse Y") * sensitivity;
        //composer.m_TrackedObjectOffset.y += vertical;
        composer.m_TrackedObjectOffset.y = Mathf.Clamp(composer.m_TrackedObjectOffset.y, -10, 10);
    }
}
