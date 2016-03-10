using UnityEngine;
using System.Collections.Generic;

public static class TransformExtensions {
    private static Dictionary<Collider, IWeapon> weaponLookupFromCollider = new Dictionary<Collider, IWeapon>();
    private static Dictionary<Collider, IDangerous> dangerousLookupFromCollider = new Dictionary<Collider, IDangerous>();
    private static Dictionary<GameObject, Entity> entityLookupFromGameObject = new Dictionary<GameObject, Entity>();

    public static void Register(this Transform t)
    {
        Collider collider = t.GetComponentInChildren<Collider>();
        IWeapon weapon = (IWeapon)t.GetComponent(typeof(IWeapon));
        IDangerous dangerous = (IDangerous)t.GetComponent(typeof(IDangerous));
        Entity entity = t.GetComponent<Entity>();

        if(entity != null)
        {
            entityLookupFromGameObject.Add(t.gameObject, entity);
        }

        if (weapon != null)
        {
            weaponLookupFromCollider.Add(collider, weapon);
        }

        if(dangerous != null)
        {
            dangerousLookupFromCollider.Add(collider, dangerous);
        }
    }

    public static IDangerous GetDangerous(this Collider collider)
    {
        return dangerousLookupFromCollider[collider];
    }

    public static Entity GetEntity(this GameObject go)
    {
        return entityLookupFromGameObject[go];
    }

    public static IWeapon GetWeapon(this Collider collider)
    {
        return weaponLookupFromCollider[collider];
    }

}
