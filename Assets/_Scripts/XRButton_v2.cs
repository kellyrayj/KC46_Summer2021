using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using NaughtyAttributes;
using UnityEngine.Events;
using System;

public class XRButton_v2 : MonoBehaviour
{
    public ConfigurableJoint joint;
    public Rigidbody buttonRB;
    public Transform buttonStopper;
    public Transform pressedTargetTransform;

    public bool actuated;
    public bool beingPressed;

    [Space(10)]
    [Header("Events")]

    public UnityEvent onPressed;

    public UnityEvent onActuated;

    public UnityEvent onDeactuated;

    public bool debugButton;
    public LayerMask debuglayer;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        AlignToSurface();
    }

    public void Init()
    {
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = joint.linearLimit.limit * transform.localScale.x;
        joint.linearLimit = limit;
        actuated = false;
        buttonStopper.SetParent(null);
    }

    private void FixedUpdate()
    {
        var dist = Vector3.Distance(buttonRB.position, transform.position);
        //beingPressed = dist > 0.001f;

        if (dist <= 0.0001f && beingPressed)
        {
            beingPressed = false;
            Debug.Log("bring pressed is false");
        }

        HandleEvents(dist);
        //print(dist);

        if(buttonRB.transform.localPosition.y > transform.localPosition.y)
        {
            buttonRB.transform.position = transform.position;
            buttonRB.velocity = Vector3.zero;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    beingPressed = true;
    //    print(collision.collider.name);
    //}
    [Button("AlignToSurface")]
    public void AlignToSurface()
    {
        //RaycastHit hit;
        //// Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, debuglayer))
        //{
        //    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //    //Debug.Log("Did Hit");
        //    transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);

        //}
        //var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        Gizmos.color = new Color(1f, .92f, .016f, 0.5f);
        var rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(pressedTargetTransform.localPosition, new Vector3(.04f , .0005f, .04f));

        //Gizmos.DrawCube()
        //Gizmos.DrawLine(buttonVisual.transform.position, pressedTargetTransform.position);
        //Gizmos.DrawWireSphere(pressedTargetTransform.position, 0.002f);
    }


    public void HandleEvents(float dist)
    {

        if (dist >= joint.linearLimit.limit)
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
                    Debug.Log(gameObject.name + ": Button was deactuaed");
                }
                else
                {
                    actuated = true;
                    onActuated.Invoke();
                    Debug.Log(gameObject.name + ": Button was actuated");
                }

            }
        }
    }
}
