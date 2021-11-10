using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class XRRotator : MonoBehaviour
{
    [Serializable]
    public class RotatorEvent
    {
        public float angleNeeded;
        public UnityEvent angleEvent;
    }

    public Transform linkedDialVisual;
    public Transform rotatorAxisProxy;
    [Space(5)]

    //[Header("Rotation Axis")]
    //[Dropdown("axisValues")]
    //public string axis;
    //private string[] axisValues = new string[] { "X", "Y", "Z" };

    [HorizontalLine]
    [Space(5)]

    [SerializeField] private int snapRotationAmout = 25;
    [SerializeField] private float angleTolerance;

    private float startAngle;
    [Space(10)]
    //public bool clampRotation;
    //public float angleMinMax;

    //private XRBaseInteractor interactor;
    //private float startAngle;
    private bool requiresStartAngle = true;
    private bool shouldGetHandRotation = false;
    //private Rigidbody myRB;

    [HorizontalLine]

    public List<RotatorEvent> RotationEvents = new List<RotatorEvent>();

    [Space(10)]
    public bool debugRotator;


    //MONOBEHAVIOR

    //public void Start()
    //{
    //    rotatorAxisProxy.localPosition = Vector3.zero;
    //    rotatorAxisProxy.localRotation = Quaternion.Euler(0, 0, 0);
    //}

    public void GrabbedBy()
    {
        //interactor = GetComponent<XRGrabInteractable>().selectingInteractor;
        ////interactor.GetComponent<XRDirectInteractor>().hideControllerOnSelect = true;

        //rotatorAxisProxy.SetParent(interactor.transform);
        //rotatorAxisProxy.localPosition = Vector3.zero;

        shouldGetHandRotation = true;
        startAngle = 0f;
    }

    public void GrabEnd()
    {
        shouldGetHandRotation = false;
        requiresStartAngle = true;

        //rotatorAxisProxy.SetParent(transform);
        //rotatorAxisProxy.localPosition = Vector3.zero;
        //rotatorAxisProxy.localRotation = Quaternion.Euler(0, 0, 0);

    }

    void Update()
    {
        if (shouldGetHandRotation)
        {
            var rotationAngle = GetInteractorRotation(); //gets the current controller angle
            GetRotationDistance(rotationAngle);
        }
    }

    public float GetInteractorRotation()
    {
        //var handRotation = interactor.GetComponent<Transform>().eulerAngles;
        //handRotation.
        var handRotation = rotatorAxisProxy.transform.eulerAngles;
        return handRotation.y;
        
    }

    private void GetRotationDistance(float currentAngle)
    {
        if (!requiresStartAngle)
        {
            var angleDifference = Mathf.Abs(startAngle - currentAngle);

            if (angleDifference > angleTolerance)
            {
                if (angleDifference > 270f) //checking to see if the user has gone from 0-360 - a very tiny movement but will trigger the angletolerance
                {
                    float angleCheck;

                    if (startAngle < currentAngle) //going anticlockwise
                    {
                        angleCheck = CheckAngle(currentAngle, startAngle);

                        if (angleCheck < angleTolerance)
                        {
                            return;
                        }
                        else
                        {
                            RotateDialAntiClockwise();
                            startAngle = currentAngle;
                        }
                    }
                    else if (startAngle > currentAngle) //going clockwise;
                    {
                        angleCheck = CheckAngle(currentAngle, startAngle);

                        if (angleCheck < angleTolerance)
                        {
                            return;
                        }
                        else
                        {
                            RotateDialClockwise();
                            startAngle = currentAngle;
                        }
                    }
                }
                else
                {
                    if (startAngle < currentAngle)//clockwise
                    {
                        RotateDialClockwise();
                        startAngle = currentAngle;
                    }
                    else if (startAngle > currentAngle)
                    {
                        RotateDialAntiClockwise();
                        startAngle = currentAngle;
                    }
                }
            }
        }
        else
        {
            requiresStartAngle = false;
            startAngle = currentAngle;
        }
    }

    private void RotateDialAntiClockwise()
    {
        //Debug.Log("rotate counter clockwise");
        linkedDialVisual.localEulerAngles = new Vector3(linkedDialVisual.localEulerAngles.x, linkedDialVisual.localEulerAngles.y - snapRotationAmout, linkedDialVisual.localEulerAngles.z);
        DialChanged(linkedDialVisual.localEulerAngles.y);
    }

    private void RotateDialClockwise()
    {
        //Debug.Log("rotate clockwise");
        linkedDialVisual.localEulerAngles = new Vector3(linkedDialVisual.localEulerAngles.x, linkedDialVisual.localEulerAngles.y + snapRotationAmout, linkedDialVisual.localEulerAngles.z);
        DialChanged(linkedDialVisual.localEulerAngles.y);
    }

    private float CheckAngle(float currentAngle, float startAngle)
    {
        var checkAngleTravelled = (360f - currentAngle) + startAngle;
        return (checkAngleTravelled);
    }

    public void DialChanged(float dialValue)
    {
        dialValue = Mathf.RoundToInt(dialValue);

        if (debugRotator)
        {
            Debug.Log(dialValue);
        }

        foreach (RotatorEvent r in RotationEvents)
        {
            if (r.angleNeeded == dialValue)
            {
                r.angleEvent.Invoke();
                if (debugRotator)
                {
                    Debug.Log(gameObject.name + ": " + r.angleNeeded.ToString() + " event was fired");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.TransformDirection(Vector3.up) * 0.1f));
    }
}
