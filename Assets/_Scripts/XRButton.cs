using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using NaughtyAttributes;
using UnityEngine.Events;
using System;

public class XRButton : MonoBehaviour
{
    public Transform buttonVisual;
    public string fingerTag = "Finger";

    //[Header("Button Parameters")]

    //[Dropdown("axisValues")]
    //public string axis;

    //private string[] axisValues = new string[] { "X", "Y", "Z"};

    //public bool inverted;
    [Space(10)]

    public Transform pressedTargetTransform;
    public Transform clampMaxPosTransform;
    public float pressedThreshold;

    public bool actuated;

    public bool invertedButton;
    //public float maxThreshold;

    [HideInInspector]public SphereCollider playerFinger;

    [Space(10)]
    [Header("Events")]
    
    public UnityEvent onPressed;

    public UnityEvent onActuated;

    public UnityEvent onDeactuated;
        
    //public UnityEvent onRelease;

    [HideInInspector] public bool beingPressed;

    [Space(10)]
    public bool debugButton;

    private Vector3 prevFingerPosition;
    private float distanceFromPressedTarget;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        actuated = false;
    }

    private void Update()
    {
        if (playerFinger)
        {
            var deltaFingerMove = prevFingerPosition - playerFinger.transform.TransformPoint(playerFinger.center);
            
            var dir = buttonVisual.transform.position - pressedTargetTransform.position;

            //if (invertedButton)
            //{
            //    dir = pressedTargetTransform.position - buttonVisual.transform.position;
            //}
            //else
            //{

            //    

            //}


            var dot = Vector3.Dot(dir.normalized, deltaFingerMove.normalized);
            if (!invertedButton)
            {
                //translate button visual based on dot product
                if (dot < 0)
                {
                    //buttonVisual.transform.Translate(-dir.normalized * deltaFingerMove.magnitude);
                    buttonVisual.transform.Translate(transform.InverseTransformDirection(-transform.up).normalized * deltaFingerMove.magnitude);
                }

                if (dot > 0)
                {
                    //buttonVisual.transform.Translate(dir.normalized * deltaFingerMove.magnitude);
                    buttonVisual.transform.Translate(transform.InverseTransformDirection(transform.up).normalized * deltaFingerMove.magnitude);
                }
            }
            else
            {
                //translate button visual based on dot product
                if (dot < 0)
                {
                    //buttonVisual.transform.Translate(dir.normalized * deltaFingerMove.magnitude);
                    buttonVisual.transform.Translate(transform.InverseTransformDirection(transform.up).normalized * deltaFingerMove.magnitude);
                }

                if (dot > 0)
                {
                    //buttonVisual.transform.Translate(-dir.normalized * deltaFingerMove.magnitude);
                    buttonVisual.transform.Translate(transform.InverseTransformDirection(-transform.up).normalized * deltaFingerMove.magnitude);
                }
            }
            //if (invertedButton)
            //{
            //    buttonVisual.transform.Translate(transform.InverseTransformDirection(-transform.up).normalized * deltaFingerMove.magnitude);


            //}
            //else
            //{
            //    buttonVisual.transform.Translate(transform.InverseTransformDirection(transform.up).normalized * deltaFingerMove.magnitude);

            //    //clamp button
            //}

            ////clamp button
            //if (Vector3.Distance(buttonVisual.transform.position, clampMaxPosTransform.position) <= pressedThreshold)
            //{
            //    buttonVisual.transform.position = clampMaxPosTransform.position;
            //}


            distanceFromPressedTarget = Vector3.Distance(pressedTargetTransform.position, buttonVisual.transform.position);

            HandleEvents();

            prevFingerPosition = playerFinger.transform.TransformPoint(playerFinger.center);
        }
    }

    public void HandleEvents()
    {

        if(distanceFromPressedTarget <= pressedThreshold)
        {
            //on pressed
            if (!beingPressed)
            {
                beingPressed = true;
                onPressed.Invoke();
                if (debugButton)
                {
                    Debug.Log(gameObject.name + ": Button was pressed");
                }


                if (actuated)
                {
                    actuated = false;
                    onDeactuated.Invoke();
                }
                else
                {
                    actuated = true;
                    onActuated.Invoke();
                }

            }
        }
    }

    public void ResetButton()
    {
        buttonVisual.localPosition = Vector3.zero;
        beingPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerFinger)
        {
            if (other.CompareTag(fingerTag))
            {
                playerFinger = other.GetComponent<SphereCollider>();
                prevFingerPosition = playerFinger.transform.TransformPoint(playerFinger.center);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerFinger)
        {
            if(other.GetComponent<SphereCollider>() == playerFinger)
            {
                playerFinger = null;
                ResetButton();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(buttonVisual.transform.position, pressedTargetTransform.position);
        Gizmos.DrawWireSphere(pressedTargetTransform.position, 0.002f);
    }

}
