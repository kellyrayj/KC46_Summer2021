using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KC46_FingerController : MonoBehaviour
{
    [SerializeField] InputActionReference grip;
    [SerializeField] InputActionReference trigger;
    //[SerializeField] InputActionReference trigger;
    private Collider myCol;
    //public PlayerInput playerInput;

    bool gripPress;
    bool triggerPress;

    private void Awake()
    {
        myCol = GetComponent<Collider>();
    }

    private void Update()
    {
        if (grip.action.WasPressedThisFrame())
        {
            gripPress = true;
        }

        if (grip.action.WasReleasedThisFrame())
        {
            gripPress = false;
        }

        if (trigger.action.WasPressedThisFrame())
        {
            triggerPress = true;
        }

        if (trigger.action.WasReleasedThisFrame())
        {
            triggerPress = false;
        }

        if(!triggerPress && !gripPress && myCol.enabled == false)
        {
            myCol.enabled = true;
        }

        if (triggerPress && myCol.enabled == true || gripPress && myCol.enabled == true)
        {
            myCol.enabled = false;
        }
    }


}
