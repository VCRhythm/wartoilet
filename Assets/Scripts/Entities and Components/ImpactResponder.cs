using UnityEngine;
using DG.Tweening;

public class ImpactResponder : MonoBehaviour {

    public Vector3 impactEulerAngles;
    public float impactAngleMinThreshold = 50f;

    private Vector3 originalEulerAngles;

    void Awake()
    {
        originalEulerAngles = transform.eulerAngles;
    }

    public void Impact(Vector3 impact, float effectAngle, float effectSpeed, bool canRecover, float recoveryTime = 0)
    {
        if (impactAngleMinThreshold <= effectAngle)
        {
            transform.DOKill();

            Vector3 impactRotation = new Vector3(impact.z * impactEulerAngles.x, 0, impact.x * impactEulerAngles.z) * effectAngle;

            Sequence impactSequence = DOTween.Sequence();
            impactSequence.Append(transform.DORotate(impactRotation + originalEulerAngles, effectSpeed));

            if (canRecover)
            {
                impactSequence.Append(
                    //DOTween.To(() => transform.localEulerAngles.x, x => transform.localEulerAngles = new Vector3(x, originalEulerAngles.y, originalEulerAngles.z), originalEulerAngles.x, recoveryTime)
                    //DOTween.ToAxis(() => transform.localEulerAngles, x => transform.localEulerAngles = x, originalEulerAngles.x, recoveryTime * 10, AxisConstraint.X)
                    transform.DORotate(originalEulerAngles, recoveryTime)
                    );

            }            
        }
    }
}
