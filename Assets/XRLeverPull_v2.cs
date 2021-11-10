using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class XRLeverPull_v2 : MonoBehaviour
{
    public Rigidbody leverRB;
    public ConfigurableJoint joint;

    //public float minReachAngle;
    //public float maxReachAngle;

    public bool InvertAngle;
    public bool useActualAngles;

    public float maxActualAngle;
    public float minActualAngle;

    [Tooltip("Degrees within range of min or max before events are fired")]
    public float minMaxThreshold = 5;

    [Header("Events")]

    public UnityEvent onMaxReached;
    public UnityEvent onMinReached;

    private bool maxEventFired;
    private bool minEventFired;
    public bool debugEvents;
    public bool debugAngle;

    // Start is called before the first frame update
    //void Start()
    //{
    //    print(joint.highAngularXLimit.limit - minMaxThreshold);
    //}

    // Update is called once per frame
    void FixedUpdate()
    {

        //print(leverRB.transform.localEulerAngles.x);
        var angle = leverRB.transform.localEulerAngles.x;
        //angle = Mathf.Abs(WrapAngle(angle));
        angle = WrapAngle(angle);

        if (InvertAngle)
        {
            angle += 180;
        }

        

        if (!useActualAngles)
        {
            if (angle >= (joint.highAngularXLimit.limit - minMaxThreshold) && !maxEventFired)
            {
                maxEventFired = true;
                minEventFired = false;
                onMaxReached.Invoke();
                if (debugEvents)
                {
                    Debug.Log("Max Reached");
                }
            }

            if (angle <= (joint.lowAngularXLimit.limit + minMaxThreshold) && !minEventFired)
            {
                maxEventFired = false;
                minEventFired = true;
                onMinReached.Invoke();
                if (debugEvents)
                {
                    Debug.Log("Min Reached");
                }
            }

            if (debugAngle)
            {
                print(angle);
            }
        }
        else
        {
            angle = Mathf.Round(angle);

            if (angle == maxActualAngle && !maxEventFired)
            {
                maxEventFired = true;
                minEventFired = false;
                onMaxReached.Invoke();
                if (debugEvents)
                {
                    Debug.Log("Max Reached");
                }
            }

            if (angle == minActualAngle && !minEventFired)
            {
                maxEventFired = false;
                minEventFired = true;
                onMinReached.Invoke();
                if (debugEvents)
                {
                    Debug.Log("Min Reached");
                }
            }

            if (debugAngle)
            {
                print(angle);
            }
        }

        








        //if(eventFired && leverRB.transform.localEulerAngles.z >= (-joint.angularZLimit.limit + minMaxThreshold) || leverRB.transform.localEulerAngles.z <= (joint.angularZLimit.limit - minMaxThreshold) && eventFired)
        //{
        //    eventFired = false;
        //    if (debugEvents)
        //    {
        //        Debug.Log("event becomes not fired");
        //    }
        //}
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

}
