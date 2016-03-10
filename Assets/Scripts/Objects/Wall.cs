using UnityEngine;
using System.Linq;

public class Wall : MonoBehaviour, ITriggerObserver {

    [HideInInspector]
    public bool canPhaseThroughWall = false;
    private Collider col;

    void Awake()
    {
        col = GetComponentInChildren<Collider>();
    }

    void Start()
    {
        Trigger[] triggers = GetComponentsInChildren<Trigger>();
        triggers.First(x => x.type == Trigger.Type.Out).Register(this);
    }

    public void EnterInTrigger(Collider other)
    {
    }

    public void EnterOutTrigger(Collider other)
    {
        if (canPhaseThroughWall)
        {
            Physics.IgnoreCollision(other, col, true);
        }
    }

    public void ExitOutTrigger(Collider other)
    {
        if (canPhaseThroughWall)
        {
            Physics.IgnoreCollision(other, col, false);
        }
    }

}
