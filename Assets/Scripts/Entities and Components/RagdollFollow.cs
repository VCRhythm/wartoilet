using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class RagdollFollow : MonoBehaviour, IEntityObserver {
    public GameObject master;

    public Transform[] ignoreTransforms = new Transform[0];
    public int[] stunMask;
    public int[] deathMask;

    public enum Mask
    {
        None,
        Stun,
        Death
    }
    public Mask mask;

    [ReadOnly] public List<Transform> masterTransforms = new List<Transform>();

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

        for (int i=3; i<masterTrans.Length; i++)
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

    public void UpdateHealth(int health)
    {
    }

    public void Stun(Vector3 position, int health)
    {
        mask = Mask.Stun;
        followMask = SetMask();
    }

    public void UnStun()
    {
        mask = Mask.None;
        followMask = SetMask();
    }

    public void Die(Vector3 position)
    {
        mask = Mask.Death;
        followMask = SetMask();
    }

    public void Move(Vector3 moveVector)
    {

    }
}
