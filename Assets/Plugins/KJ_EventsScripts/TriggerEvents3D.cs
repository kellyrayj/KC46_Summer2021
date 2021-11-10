using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents3D : MonoBehaviour
{
    public string objectTag;
    [Space(20)]

    public UnityEvent onEnter;
    public UnityEvent onStay;
    public UnityEvent onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(objectTag))
        {
            onEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(objectTag))
        {
            onExit.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(objectTag))
        {
            onStay.Invoke();
        }
    }
}
