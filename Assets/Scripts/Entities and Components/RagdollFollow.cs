using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Entities
{
public class RagdollFollow : EntityObserverBase {
    [SerializeField] private GameObject master;

    [SerializeField] private Transform[] ignoreTransforms = new Transform[0];
    [SerializeField] private int[] stunMask;
    [SerializeField] private int[] deathMask;

    public enum Mask
    {
        None,
        Stun,
        Death
    }
    [SerializeField] private Mask mask;

    [ReadOnly] [SerializeField] private List<Transform> masterTransforms = new List<Transform>();

    private const int RootBoneOffset = 3;

    private int[] followMask = new int[0];
    private List<Transform> ragdollTransforms = new List<Transform>();

    void Awake()
    {
        SetUpTransforms();
        SetRigidbodies();
        HideMaster();

        GetComponentInParent<Entity>().RegisterObserver(this);
    }

    void Update()
    {
        Follow();
    }

    private void Follow()
    {
        for(int i = 0; i < masterTransforms.Count; i++)
        {
            if (!followMask.Contains(i))
            {
                SetPositionAndRotation(i);
            }
        }
    }

    private int[] SetMask()
    {
        switch(mask)
        {
            case Mask.Stun:
                return stunMask;
            case Mask.Death:
                return deathMask;
            default:
                return new int[0];
        }
    }

    private void SetPositionAndRotation(int i)
    {
        ragdollTransforms[i].position = masterTransforms[i].position;
        ragdollTransforms[i].rotation = masterTransforms[i].rotation;
    }

    private void SetRigidbodies()
    {
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
    }

    private void SetUpTransforms()
    {
        Transform[] masterTrans = master.GetComponentsInChildren<Transform>();
        Transform[] ragdollTrans = GetComponentsInChildren<Transform>();

        for (int i = RootBoneOffset; i < masterTrans.Length; i++)
        {
            if(!ignoreTransforms.Contains(masterTrans[i]))
            {
                masterTransforms.Add(masterTrans[i]);
                ragdollTransforms.Add(ragdollTrans[i]);
            }
        }
        ragdollTrans = GetComponentsInChildren<Transform>();
    }

    private void HideMaster()
    {
        master.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.SetActive(false);
    }

    public override void Stun(Vector3 position, int health)
    {
        mask = Mask.Stun;
        followMask = SetMask();
    }

    public override void UnStun()
    {
        mask = Mask.None;
        followMask = SetMask();
    }

    public override void Die(Vector3 position)
    {
        mask = Mask.Death;
        followMask = SetMask();
    }

}
}
