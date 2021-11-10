using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class KC46CockpitManager : MonoBehaviour
{
    public float buttonInputDelay;
    public bool canTakeInput;

    [HorizontalLine]

    public List<ButtonBase> buttonMasterList = new List<ButtonBase>();

    // Start is called before the first frame update
    void Start()
    {
        canTakeInput = true;
    }

    //states the input delay feature, called by the button controller class
    public void StartButtonDelayTimer()
    {
        StartCoroutine(DoInputButtonDelay());
    }


    //This coroutine prevents the pilot from pressing to many buttons at once or accidently pressing a button
    IEnumerator DoInputButtonDelay()
    {
        canTakeInput = false;
        yield return new WaitForSeconds(buttonInputDelay);
        canTakeInput = true;
        //yield return null;
    }

    public bool CheckIfValidButton(KC46ButtonController buttonToCheck, ButtonBase correctButton)
    {
        if(buttonToCheck == correctButton)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
        
}
