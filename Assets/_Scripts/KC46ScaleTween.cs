using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KC46ScaleTween : MonoBehaviour
{
    public float size;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(transform.localScale * size, time).SetLoops(-1).SetEase(Ease.InOutSine);
    }

}
