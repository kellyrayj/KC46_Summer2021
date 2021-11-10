using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerVelocity : MonoBehaviour
{
    public Transform trackedFinger;
    private Rigidbody myRB;
    private Vector3 velocity;
    private Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        prevPos = trackedFinger.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = prevPos - trackedFinger.position;
        myRB.MovePosition(trackedFinger.position);
    }
}
