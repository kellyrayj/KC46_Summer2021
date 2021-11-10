using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KC46DebugPlaneSize : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Vector3 orgScale;
    private void Start()
    {
        orgScale = transform.localScale;
    }

    public void PlaneSizer(float scaleFactor)
    {
        transform.localScale = orgScale * scaleFactor;
        text.text = scaleFactor.ToString();
    }
}
