using UnityEngine;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Entities
{
public class PlayerEntity : Entity {

    private SpriteRenderer targetingCursor;
    private const float targetingHeight = 2f;

    protected override void Awake()
    {
        base.Awake();

        targetingCursor = transform.Find("TargetingCursor").GetComponent<SpriteRenderer>();
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
        if(Target)
        {
            targetingCursor.transform.position = new Vector3(Target.transform.position.x, targetingHeight, Target.transform.position.z);
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
}
