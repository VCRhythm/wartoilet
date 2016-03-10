using UnityEngine;

public class PlayerController : MonoBehaviour, IController {

    private IInput input;

    void Awake()
    {
        input = (IInput)GetComponent(typeof(IInput));
    }

    public Vector3 GetMoveVector()
    {
        return new Vector3(input.GetAxes(0).x, 0, input.GetAxes(0).y);
    }

    public float GetWeaponPosition()
    {
        return input.GetAxesRaw(1).x;
    }

    public bool ChangeTarget()
    {
        return Input.GetButtonDown("Change Target");
    }

    public Swing GetSwing()
    {
        if (input.HasInputStarted(0))
        {
            return new Swing(Swing.Type.Full, Position.Left);
        }
        else if (input.HasInputStarted(1))
        {
            return new Swing(Swing.Type.Full, Position.Right);
        }
        else if (input.HasInputStarted(2))
        {
            return new Swing(Swing.Type.Slash, Position.Left);
        }
        else if (input.HasInputStarted(3))
        {
            return new Swing(Swing.Type.Slash, Position.Right);
        }

        else return null;
    }

}