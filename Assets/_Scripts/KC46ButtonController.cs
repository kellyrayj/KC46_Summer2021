using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

public class KC46ButtonController : MonoBehaviour
{

    public ButtonBase buttonData;

    //private Animator myAnim;
    private KC46CockpitManager cockpitManager;

    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;
    public UnityEvent OnSelectionEnter; //grabbing
    public UnityEvent OnSelectionExit; //letting go
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    public bool hovering, wasHoveringLastFrame;
    

    //we gather our required components
    private void Start()
    {
        //myAnim = GetComponent<Animator>();
        cockpitManager = FindObjectOfType<KC46CockpitManager>();
        buttonData.myButtonController = this;

        //SetButtonToDefault();
    }


    private void Update()
    {
        if (!wasHoveringLastFrame && hovering)
        {
            OnHoverEnter.Invoke();
        }

        if (wasHoveringLastFrame && !hovering)
        {
            OnHoverExit.Invoke();
        }

        wasHoveringLastFrame = hovering;
    }

    //handy function to set the default state to 0
    [Button("ResetToDefaut")]
    public virtual void SetButtonToDefault()
    {
        buttonData.timeOfLastStateChange = Time.time;
        //myAnim.SetInteger("State", 0);
        cockpitManager.StartButtonDelayTimer();
    }

    //handy function to set the default state to any state
    public virtual void SetButtonState(int state)
    {
        //check to see if state is in the range of states we can have

        buttonData.timeOfLastStateChange = Time.time;
        //myAnim.SetInteger("State", state);
        cockpitManager.StartButtonDelayTimer();
    }

    //normal activation function when player touches a button
    [Button("GoToNextState")]
    public virtual void GoToNextState()
    {

        //if we can take input we check the state and add one to it
        if (cockpitManager.canTakeInput)
        {
            buttonData.currentState++;
            //print("do go to next state");

            //if our current state is equal to or somehow above max states, we go back to state 0
            if (buttonData.currentState >= buttonData.maxStates)
            {
                buttonData.currentState = 0;
            }

            buttonData.timeOfLastStateChange = Time.time;
            //myAnim.SetInteger("State", buttonData.currentState);

            //we start a delay input timer so the buttons have time to react and animatte
            cockpitManager.StartButtonDelayTimer();
        }
    }
        
}
