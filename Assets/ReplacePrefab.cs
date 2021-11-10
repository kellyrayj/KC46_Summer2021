using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplacePrefab : MonoBehaviour
{
    public GameObject replaceWith;

    public void RunUpdate()
    {
        if (replaceWith)
        {
            //var go = (GameObject)PrefabUtility.InstantiatePrefab(replaceWith);
            //go.transform.parent = transform.parent;

            ////var go = EditorUtility.InstantiatePrefab()
            //go.transform.localPosition = transform.localPosition;
            //go.transform.localRotation = transform.localRotation;

            //DestroyImmediate(gameObject);
        }

       
    }
      
}
