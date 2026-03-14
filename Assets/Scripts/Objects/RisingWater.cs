using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace WarToilet.Objects
{
    public class RisingWater : MonoBehaviour {

        [SerializeField] private float scaleTarget = 2f;
        [SerializeField] private float scaleTimeModifier = 1f;

        [SerializeField] private float riseTarget = 2.2f;
        [SerializeField] private float riseTimeModifier = 1f;

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
}
