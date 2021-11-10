using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DoUpdateOfButtons : MonoBehaviour
{
    [ContextMenu("Do Something")]
    public void DoUpdate()
    {
        var updates = FindObjectsOfType<ReplacePrefab>();
        
        foreach(ReplacePrefab p in updates)
        {
            p.RunUpdate();
        }
    }
}
