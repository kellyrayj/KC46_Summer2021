using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class KC46ButtonController_Guarded : KC46ButtonController
{

    //public ButtonBase buttonData;

    private Animator myAnim2;
    private KC46CockpitManager cockpitManager2;
    private bool Reverse;

    //we gather our required components
    private void Start()
    {
        myAnim2 = GetComponent<Animator>();
        cockpitManager2 = FindObjectOfType<KC46CockpitManager>();
    }

    
    override public void GoToNextState()
    {
        //check if Reverse should be enabled
        if (buttonData.currentState == 0)
        {
            Reverse = false;
        }
        else if (buttonData.currentState == buttonData.maxStates - 1)
        {
            Reverse = true;
        }
        //if we can take input we check the state and add one to it or subtract one from it if !Reverse
        if (cockpitManager2.canTakeInput)
        {
            if (!Reverse)
            {
                buttonData.currentState++;
                print("do go to next state");
            }
            else
            {
                buttonData.currentState--;
                print("do go to previous state");
            }

            buttonData.timeOfLastStateChange = Time.time;
            myAnim2.SetInteger("State", buttonData.currentState);

            //we start a delay input timer so the buttons have time to react and animate
            cockpitManager2.StartButtonDelayTimer();
        }
    }

}