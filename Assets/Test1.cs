using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("NICEEEEEEEEE");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.LogWarning("xdddddddddd - "+other.name);
    }
}
