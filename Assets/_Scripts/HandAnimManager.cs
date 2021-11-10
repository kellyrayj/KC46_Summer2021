using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimManager : MonoBehaviour
{
    [SerializeField] InputActionReference grip;
    [SerializeField] InputActionReference trigger;

    private float flex;
    private float pinch;
    private float point;
    private float smoothPoint;

    private Animator handAnim;
    private float pointRef;

    private void Awake()
    {
        handAnim = GetComponentInChildren<Animator>();
        grip.action.performed += GripPress;
        trigger.action.performed += TriggerPress;
    }

    private void Update()
    {
        
        handAnim.SetFloat("Flex", flex);
        handAnim.SetFloat("Pinch", pinch);
        point = 1 - pinch;
        handAnim.SetLayerWeight(2, point);
    }

    void GripPress(InputAction.CallbackContext obj) => flex = obj.ReadValue<float>();
    void TriggerPress(InputAction.CallbackContext obj) => pinch = obj.ReadValue<float>();



    //void GripPress(InputAction.CallbackContext obj) => handAnim.SetFloat("Flex", obj.ReadValue<float>());
    //void TriggerPress(InputAction.CallbackContext obj) => handAnim.SetFloat("Pinch", obj.ReadValue<float>());
}
