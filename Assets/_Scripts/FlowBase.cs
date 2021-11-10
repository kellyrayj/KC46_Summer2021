using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[CreateAssetMenu]
public class FlowBase : ScriptableObject
{
    [Serializable]
    public class FlowButton
    {
        public ButtonBase button;
        public int stateNeeded;
    }

    public string flowName;

    [HorizontalLine]

    public List<FlowButton> flowOrderList = new List<FlowButton>();

    [Button("Run Flow")]
    public void RunFlow()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Needs to be in Play Mode to run a flow");
            return;
        }

        var cockpitManager = FindObjectOfType<KC46CockpitManager>();

        if (cockpitManager)
        {
            Debug.Log("Run " + flowName);
        }
        else
        {
            Debug.LogError("No Cockpit Manager found in scene");
        }
    }
}
