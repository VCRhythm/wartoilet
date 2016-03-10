using UnityEngine;
using DG.Tweening;

public enum Position
{
    Left = -1,
    Right = 1,
    Center = 0
}

public class MeleeWeapon : MonoBehaviour, IWeapon, IDangerous {

    public bool IsDangerous { get { return isDangerous; } }
    public Vector3 ImpactPoint { get { return impactPoint.position; } }

    private Transform impactPoint;
    private bool isDangerous;

    private Vector3 velocity;
    private float smoothTime = 1f;
    private float cooldown;

    private Position position;
    private TrailRenderer trail;
    private AudioSource audioSource;

    void Awake()
    {
        transform.Register();

        audioSource = GetComponent<AudioSource>();
        trail = GetComponentInChildren<TrailRenderer>();
        impactPoint = transform.FindChild("ImpactPoint");
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (CanBeDeflected(otherCollider))
        {
            //-(int)GetPosition(otherCollider.transform.position));
        }
    }

    public void UpdateFromEntity(IController controller, GameObject target)
    {
        if (Time.time > cooldown)
        {
            SetPosition(controller.GetWeaponPosition());
            ExecuteSwing(controller.GetSwing());
        }

        //MoveWeapon(controller.GetWeaponPosition());
    }

    private Position GetPosition(Vector3 position)
    {
        float val = AngleDir(transform.forward, position - transform.position, transform.up);

        if (val > 0) return Position.Right;
        else if (val < 0) return Position.Left;
        else return Position.Center;
    }

    private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else {
            return 0.0f;
        }
    }

    private void MoveWeapon(float x)
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0.4f * (int)position, 0, transform.localPosition.z), ref velocity, smoothTime * Time.deltaTime);
    }

    private void SetPosition(float x)
    {
        if (x > 0)
        {
            position = Position.Right;
        }
        else if (x < 0)
        {
            position = Position.Left;
        }
        else
        {
            position = Position.Center;
        }
    }

    private void ExecuteSwing(Swing swing)
    {
        if (swing == null) return;

        isDangerous = false;

        transform.DOKill();

        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence.Append(transform.DOLocalRotate(Vector3.up * swing.backSwingRotationAngle, swing.speed, RotateMode.Fast).SetEase(Ease.InCubic).OnComplete(() => { isDangerous = true; trail.enabled = true; audioSource.Play(); }))
            .Append(transform.DOLocalRotate(Vector3.up * swing.followThroughRotationAngle, swing.speed / 2, swing.followThroughRotateMode).SetEase(Ease.InCubic).OnComplete(() => { isDangerous = false; trail.enabled = false; }))
            .Append(transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast).SetEase(Ease.OutBack));

        position = Position.Center;
        cooldown = swing.speed * (3/2 + 0.6f) + Time.time;
    }

    private bool CanBeDeflected(Collider otherCollider)
    {
        switch(otherCollider.tag)
        {
            case "Weapon":
                return true;

            case "Enemy":
                if (isDangerous)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
