using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InitializeEvents : MonoBehaviour
{
    public UnityEvent onEnable;
    public UnityEvent onStart;
    public UnityEvent onAwake;

    private void OnEnable()
    {
        onEnable.Invoke();
    }

    private void Awake()
    {
        onAwake.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        onStart.Invoke(); 
    }


}
