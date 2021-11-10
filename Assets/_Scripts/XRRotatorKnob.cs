using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class XRRotatorKnob : MonoBehaviour
{
    [Serializable]
    public class RotatorEvent
    {
        public float angleNeeded;
        public UnityEvent angleEvent;
    }

    public Transform linkedDialVisual;
    [Space(5)]

    [Header("Rotation Axis")]
    [Dropdown("axisValues")]
    public string axis;
    private string[] axisValues = new string[] { "X", "Y", "Z" };

    [HorizontalLine]

    [SerializeField] private int snapRotationAmout = 25;
    [SerializeField] private float angleTolerance;

    public float startAngle;
    [Space(10)]
    //public bool clampRotation;
    //public float angleMinMax;

    private XRBaseInteractor interactor;
    //private float startAngle;
    private bool requiresStartAngle = true;
    private bool shouldGetHandRotation = false;
    private Rigidbody myRB;

    [HorizontalLine]

    public List<RotatorEvent> RotationEvents = new List<RotatorEvent>();

    [Space(10)]
    public bool debugRotator;

    private void Start()
    {
        myRB = GetComponent<Rigidbody>();

        //switch (axis)
        //{
        //    case "X":
        //        myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //        break;
        //    case "Y":
        //        myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //        break;
        //    case "Z":
        //        myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        //        break;
        //    default:
        //        myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //        break;
        //}


    }



    public void GrabbedBy()
    {
        interactor = GetComponent<XRGrabInteractable>().selectingInteractor;
        //interactor.GetComponent<XRDirectInteractor>().hideControllerOnSelect = true;

        shouldGetHandRotation = true;
        startAngle = 0f;
    }

    private void HandModelVisibility(bool visibilityState)
    {
        //if(interactor.gameObject.GetComponent<XRController>().controllerNode == XRNode.RightHand)
        //{
        //    RighthandModel.SetActive(visibilityState);
        //}
        //else
        //{
        //    LefthandModel.SetActive(visibilityState);
        //}
    }

    public void GrabEnd()
    {
        shouldGetHandRotation = false;
        requiresStartAngle = true;
        //HandModelVisibility(false);
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
        var handRotation = interactor.GetComponent<Transform>().eulerAngles;
        return handRotation.z;
        //switch (axis)
        //{
        //    case "X":
        //        return handRotation.x;

        //    case "Y":
        //        return handRotation.y;

        //    case "Z":
        //        return handRotation.z;

        //    default:
        //        return handRotation.z;

        //}
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

    private float CheckAngle(float currentAngle, float startAngle)
    {
        var checkAngleTravelled = (360f - currentAngle) + startAngle;
        return (checkAngleTravelled);
    }

    private void RotateDialClockwise()
    {
        linkedDialVisual.localEulerAngles = new Vector3(linkedDialVisual.localEulerAngles.x, linkedDialVisual.localEulerAngles.y - snapRotationAmout, linkedDialVisual.localEulerAngles.z);
        DialChanged(linkedDialVisual.localEulerAngles.y);
        //var dialValue = linkedDialVisual.localEulerAngles.y;
    }

    private void RotateDialAntiClockwise()
    {
        linkedDialVisual.localEulerAngles = new Vector3(linkedDialVisual.localEulerAngles.x, linkedDialVisual.localEulerAngles.y + snapRotationAmout, linkedDialVisual.localEulerAngles.z);
        DialChanged(linkedDialVisual.localEulerAngles.y);
        //var dialValue = linkedDialVisual.localEulerAngles.y;
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
}
