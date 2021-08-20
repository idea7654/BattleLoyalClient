using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdatedFollow : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }

    public void SetTarget(Transform paramTrans)
    {
        target = paramTrans;
    }
}
