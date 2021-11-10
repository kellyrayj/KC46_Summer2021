using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents2D : MonoBehaviour
{
    public string objectTag;

    public UnityEvent onEnter;
    public UnityEvent onStay;
    public UnityEvent onExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(objectTag))
        {
            onEnter.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(objectTag))
        {
            onStay.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(objectTag))
        {
            onExit.Invoke();
        }
    }
}
