using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class scaletest : MonoBehaviour
{

    public float ScaleSize;
    public int Speed;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(transform.localScale*ScaleSize, Speed).SetLoops(-1).SetEase(Ease.InOutSine);
    }
}
