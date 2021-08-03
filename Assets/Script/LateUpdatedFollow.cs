using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdatedFollow : MonoBehaviour
{
    public Transform targetToFollow;

    private void LateUpdate()
    {
        if(targetToFollow)
        {
            transform.position = targetToFollow.position;
            transform.rotation = targetToFollow.rotation;
        }
    }
}
