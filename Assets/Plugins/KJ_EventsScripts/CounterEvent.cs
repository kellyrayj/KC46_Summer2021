using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CounterEvent : MonoBehaviour
{
    public int currentCount;
    public int neededAmount;

    public UnityEvent onComplete;

    // Start is called before the first frame update
    void Start()
    {
        currentCount = 0;
    }

    public void Add(int amountToAdd)
    {
        currentCount += amountToAdd;

        CheckProgress();
    }

    private void CheckProgress()
    {
        if(currentCount >= neededAmount)
        {
            SendEvent();
        }
    }

    private void SendEvent()
    {
        onComplete.Invoke();
    }
}
