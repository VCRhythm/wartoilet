using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RisingWater : MonoBehaviour {

    public float scaleTarget = 2f;
    public float scaleTimeModifier = 1f;

    public float riseTarget = 2.2f;
    public float riseTimeModifier = 1f;

    public float Height { get { return transform.position.y; } }

    void Start()
    {
        Rise();
    }

    public void Rise()
    {
        Sequence scaleAndRise = DOTween.Sequence();
        scaleAndRise.Append(transform.DOScale(scaleTarget, scaleTimeModifier * scaleTarget))
            .Append(transform.DOMoveY(riseTarget, (riseTarget - transform.position.y) * riseTimeModifier));
    }

    public void StopRising()
    {
        transform.DOKill();
    }

}
