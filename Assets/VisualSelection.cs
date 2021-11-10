using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSelection : MonoBehaviour
{
    public GameObject[] visuals;

    private void Start()
    {
        TurnOff();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            TurnOn();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            TurnOff();
        }

    }

    public void TurnOff()
    {
        foreach (GameObject v in visuals)
        {
            v.SetActive(false);
        }
    }

    public void TurnOn()
    {
        foreach (GameObject v in visuals)
        {
            v.SetActive(true);
        }
    }

}
