using UnityEngine;

public class PlayerEntity : Entity {

    private SpriteRenderer targetingCursor;
    private const float targetingHeight = 2f;

    protected override void Awake()
    {
        base.Awake();

        targetingCursor = transform.FindChild("TargetingCursor").GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        if (!isStunned)
        {
            Move();
        }

        base.Update();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void LateUpdate()
    {
        if(target)
        {
            targetingCursor.transform.position = new Vector3(target.transform.position.x, targetingHeight, target.transform.position.z);
            targetingCursor.enabled = true;
        }
        else
        {
            targetingCursor.enabled = false;
        }
    }

    private void Move()
    {
        TellObservers(x => x.Move(controller.GetMoveVector()));
    }

}
