using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using TMPro;

[CreateAssetMenu]
public class ButtonBase : ScriptableObject
{
    public int currentState;
    public int maxStates;
    public float timeOfLastStateChange;

    [HideInInspector] public KC46ButtonController myButtonController;
}
