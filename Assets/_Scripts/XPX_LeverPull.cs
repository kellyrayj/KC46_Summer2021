using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using NaughtyAttributes;
using UnityEngine.Events;
using System;

public class XPX_LeverPull : MonoBehaviour
{
    //[System.Serializable]
    //public class OnPositionValueChanged : UnityEvent<float>
    //{
    //}

    public Rigidbody leverGrabObject;

    [HorizontalLine]

    [Header("Axis Settings")]

    public bool ignoreX;
    public bool ignoreY;
    public bool ignoreZ;

    [HorizontalLine]

    [Header("Lever Settings")]

    [Tooltip("How close do we need to be before max and min events are called. For example, if this value is 0.1f, then the max event will be called when leverPosition becomes 0.9f")]
    public float minMaxReachedThreshold;
    [Range(-0.3f, 0)]
    public float minRange;
    [Range(0, 0.3f)]
    public float maxRange;
    [Range(0, 1)]
    public float leverPosition;

    public float pullSpeed = 1f;

    [Header("Hinge Settings")]

    public Transform hingeTransform;

    private XRBaseInteractor interactor;
    private Vector3 controllerPosLastFrame;
    //float leverPositionLastFrame;

    public bool inverted;

    private Vector3 centerPtx;
    
    private Vector3 centerPty;
    
    private Vector3 centerPtz;

    Vector3 minPosX;
    Vector3 maxPosX;
    private Vector3 minPosZ;
    private Vector3 maxPosZ;
    private Vector3 minPosY;
    private Vector3 maxPosY;

    private float leverXPos;
    private float leverYPos;
    private float leverZPos;

    [HideInInspector] public bool maxXReached;
    [HideInInspector] public bool minXReached;
    [HideInInspector] public bool maxYReached;
    [HideInInspector] public bool minYReached;
    [HideInInspector] public bool maxZReached;
    [HideInInspector] public bool minZReached;

    [Space(10)]

    [Header("Events")]
    //public bool callEventsOnStart;
    //public OnPositionValueChanged onPositionValueChanged;
    public UnityEvent maxPositionXReached;
    public UnityEvent minPositionXReached;
    public UnityEvent maxPositionYReached;
    public UnityEvent minPositionYReached;
    public UnityEvent maxPositionZReached;
    public UnityEvent minPositionZReached;

    [Space(10)]

    public bool sendDebugMessages;

    private void Start()
    {

        minPosX = transform.position + (Vector3.right * minRange);
        maxPosX = transform.position + (Vector3.right * maxRange);

        centerPtx = (minPosX + maxPosX) / 2;

        minPosY = transform.position + (Vector3.up * minRange);
        maxPosY = transform.position + (Vector3.up * maxRange);

        centerPty = (minPosY + maxPosY) / 2;


        minPosZ = transform.position + (Vector3.forward * minRange);
        maxPosZ = transform.position + (Vector3.forward * maxRange);

        centerPtz = (minPosZ + maxPosZ) / 2;

        var startPosX = Vector3.Lerp(minPosX, maxPosX, leverPosition);
        var startPosY = Vector3.Lerp(minPosY, maxPosY, leverPosition);
        var startPosZ = Vector3.Lerp(minPosZ, maxPosZ, leverPosition);

        if (ignoreX)
        {
            startPosX = leverGrabObject.transform.position;
        }

        if (ignoreY)
        {
            startPosY = leverGrabObject.transform.position;
        }

        if (ignoreZ)
        {
            startPosZ = leverGrabObject.transform.position;
        }

        Vector3 startPos = new Vector3(startPosX.x, startPosY.y, startPosZ.z);

        leverGrabObject.transform.position = startPos;

        //leverPositionLastFrame = leverPosition;

        //if (callEventsOnStart)
        //{
        //    if (leverPosition >= maxRange - minMaxReachedThreshold)
        //    {
        //        maxPositionReached.Invoke();
        //    }

        //    if (leverPosition <= minRange + minMaxReachedThreshold)
        //    {
        //        minPositionReached.Invoke();
        //    }
        //}

    }

    

    public void GrabbedBy(SelectEnterEventArgs args)
    {
        interactor = args.interactor;
        controllerPosLastFrame = interactor.transform.position;

        if (sendDebugMessages)
        {
            Debug.Log(gameObject.name + ": Grabbed by + " + args.interactor.name);
        }
    }

    public void LetGoBy(SelectExitEventArgs args)
    {
        interactor = null;

        if (sendDebugMessages)
        {
            Debug.Log(gameObject.name + ": Let go by + " + args.interactor.name);
        }
    }

    void Update()
    {

        if (interactor)
        {
            //move lever grab target to controller move delta
            CalcualteLeverGrabTargetPosition();
            //Update hinge to rotate towards
            CalculateHingeRotation();
            //update lever positions on a 0 to 1 scale
            CalculateLeverPositions();
            //send events based on lever position
            HandleEvents();
            //store controller pos for next frame
            controllerPosLastFrame = interactor.transform.position;
        }
    }

    private void CalculateLeverPositions()
    {
        if (!ignoreX)
        {
            leverXPos = Mathf.InverseLerp(minPosX.x, maxPosX.x, leverGrabObject.transform.position.x);
        }

        if (!ignoreY)
        {
            leverYPos = Mathf.InverseLerp(minPosY.y, maxPosY.y, leverGrabObject.transform.position.y);
        }

        if (!ignoreZ)
        {
            leverZPos = Mathf.InverseLerp(minPosZ.z, maxPosZ.z, leverGrabObject.transform.position.z);
        }
    }

    public void HandleEvents()
    {
        if (interactor.transform.position != controllerPosLastFrame)
        {
            //we've moved and can send events
            if (leverXPos >= (1f - minMaxReachedThreshold) && !ignoreX && !maxXReached)
            {
                maxXReached = true;
                maxPositionXReached.Invoke();
                if (sendDebugMessages)
                    Debug.Log(gameObject.name + ": Max X Reached");
            }

            if(leverXPos < (1f - minMaxReachedThreshold) && maxXReached)
            {
                maxXReached = false;
            }

            if(leverXPos <= (0 + minMaxReachedThreshold) && !ignoreX && !minXReached)
            {
                minXReached = true;
                minPositionXReached.Invoke();
                if (sendDebugMessages)
                    Debug.Log(gameObject.name + ": Min X Reached");
            }

            if (leverXPos > (0 + minMaxReachedThreshold) && minXReached)
            {
                minXReached = false;
            }

            if (leverYPos >= (1f - minMaxReachedThreshold) && !ignoreY && !maxYReached)
            {
                maxYReached = true;
                maxPositionYReached.Invoke();
                if (sendDebugMessages)
                    Debug.Log(gameObject.name + ": Max Y Reached");
            }

            if(leverYPos < (1f - minMaxReachedThreshold) && maxYReached)
            {
                maxYReached = false;
            }

            if (leverYPos <= (0 + minMaxReachedThreshold) && !ignoreY && !minYReached)
            {
                minYReached = true;
                minPositionYReached.Invoke();
                if (sendDebugMessages)
                    Debug.Log(gameObject.name + ": Min Y Reached");
            }

            if( leverYPos > (0 + minMaxReachedThreshold) && minYReached)
            {
                minYReached = false;
            }

            if (leverZPos >= (1f - minMaxReachedThreshold) && !ignoreZ && !maxZReached)
            {
                maxZReached = true;
                maxPositionZReached.Invoke();
                if (sendDebugMessages)
                    Debug.Log(gameObject.name + ": Max Z Reached");
            }

            if (leverZPos < (1f - minMaxReachedThreshold) && maxZReached)
            {
                maxZReached = false;
            }

            if (leverZPos <= (0 + minMaxReachedThreshold) && !ignoreZ && !minZReached)
            {
                minZReached = true;
                minPositionZReached.Invoke();
                if (sendDebugMessages)
                    Debug.Log(gameObject.name + ": Min Z Reached");
            }

            if (leverZPos > (0 + minMaxReachedThreshold) && minZReached)
            {
                minZReached = false;
            }
        }
    }

    public void CalculateHingeRotation()
    {

        if (!inverted)
        {
            var dir = leverGrabObject.transform.position - hingeTransform.position;
            var rot = Quaternion.FromToRotation(Vector3.up, dir);
            hingeTransform.rotation = rot;
            if (sendDebugMessages)
            {
                Debug.Log(gameObject.name + " :roation value is " + rot);
            }
        }
        else
        {
            var dir = leverGrabObject.transform.position - hingeTransform.position;
            var rot = Quaternion.FromToRotation(-Vector3.up, dir);
            hingeTransform.rotation = rot;
            if (sendDebugMessages)
            {
                Debug.Log(gameObject.name + " :roation value is " + rot);
            }
        }
        
    }

    public void CalcualteLeverGrabTargetPosition()
    {
        var moveDelta = interactor.transform.position - controllerPosLastFrame;

        if (ignoreX)
        {
            moveDelta.x = 0;
        }

        if (ignoreY)
        {
            moveDelta.y = 0;
        }

        if (ignoreZ)
        {
            moveDelta.z = 0;
        }

        var newPos = leverGrabObject.transform.position + (moveDelta * pullSpeed);

        if (!ignoreX)
        {
            if (newPos.x > transform.position.x + maxRange)
            {
                newPos.x = transform.position.x + maxRange;
            }

            if (newPos.x < transform.position.x + minRange)
            {
                newPos.x = transform.position.x + minRange;
            }
        }

        if (!ignoreY)
        {
            if (newPos.y > transform.position.y + maxRange)
            {
                newPos.y = transform.position.y + maxRange;
            }

            if (newPos.y < transform.position.y + minRange)
            {
                newPos.y = transform.position.y + minRange;
            }
        }

        if (!ignoreZ)
        {
            if (newPos.z > transform.position.z + maxRange)
            {
                newPos.z = transform.position.z + maxRange;
            }

            if (newPos.z < transform.position.z + minRange)
            {
                newPos.z = transform.position.z + minRange;
            }
        }

        //set lever to new calculated position
        leverGrabObject.transform.position = newPos;
    }

    private void OnDrawGizmos()
    {
        if (!ignoreX)
        {
            Gizmos.color = Color.red;
            var minPos = transform.position + (Vector3.right * minRange);
            var maxPos = transform.position + (Vector3.right * maxRange);
            Gizmos.DrawWireSphere(minPos, 0.005f);
            Gizmos.DrawWireSphere(maxPos, 0.005f);
            Gizmos.DrawLine(minPos, maxPos);
        }
        if (!ignoreY)
        {
            Gizmos.color = Color.green;
            var minPos = transform.position + (Vector3.up * minRange);
            var maxPos = transform.position + (Vector3.up * maxRange);
            Gizmos.DrawWireSphere(minPos, 0.005f);
            Gizmos.DrawWireSphere(maxPos, 0.005f);
            Gizmos.DrawLine(minPos, maxPos);
        }
        if (!ignoreZ)
        {
            Gizmos.color = Color.blue;
            var minPos = transform.position + (Vector3.forward * minRange);
            var maxPos = transform.position + (Vector3.forward * maxRange);
            Gizmos.DrawWireSphere(minPos, 0.005f);
            Gizmos.DrawWireSphere(maxPos, 0.005f);
            Gizmos.DrawLine(minPos, maxPos);
        }

    }
}
