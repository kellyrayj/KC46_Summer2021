using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorRigManager : MonoBehaviour
{
    public Camera editorCam;
    //public KC46ButtonController currentSelectedButton;

    private void FixedUpdate()
    {
        //RaycastHit hit;
        //// Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(editorCam.transform.position, editorCam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        //{
        //    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //    //Debug.Log("Did Hit");
        //    if(hit.collider.TryGetComponent(out KC46ButtonController button))
        //    {
        //        currentSelectedButton = button;
        //        currentSelectedButton.hovering = true;
        //    }
        //    else
        //    {
        //        if (currentSelectedButton)
        //        {
        //            currentSelectedButton.hovering = false;
        //            currentSelectedButton = null;
        //        }
                
        //    }
        //}
    }

    private void Update()
    {
        
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (currentSelectedButton)
        //    {
        //        //select enter
        //        currentSelectedButton.GoToNextState();
        //        currentSelectedButton.OnSelectionEnter.Invoke();
        //    }
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    if (currentSelectedButton)
        //    {
        //        //select exit
        //        currentSelectedButton.OnSelectionExit.Invoke();
        //    }
        //}


        //if (Input.GetMouseButtonDown(1))
        //{
        //    if (currentSelectedButton)
        //    {
        //        //select enter
        //        //currentSelectedButton.GoToNextState();
        //        currentSelectedButton.OnActivate.Invoke();
        //    }
        //}

        //if (Input.GetMouseButtonUp(1))
        //{
        //    if (currentSelectedButton)
        //    {
        //        //select exit
        //        currentSelectedButton.OnDeactivate.Invoke();
        //    }
        //}
    }

}
