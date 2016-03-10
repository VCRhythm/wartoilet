using UnityEngine;
using System;

public class AIController : MonoBehaviour, IController
{
    public Vector3 GetMoveVector()
    {
        return Vector3.zero;
    }

    public Swing GetSwing()
    {
        throw new NotImplementedException();
    }

    public float GetWeaponPosition()
    {
        throw new NotImplementedException();
    }

    public bool ChangeTarget()
    {
        return false;
    }

    public Transform GetTarget()
    {
        throw new NotImplementedException();
    }
}
