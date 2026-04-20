using UnityEngine;
using System.Collections.Generic;
using WarToilet.Interfaces;
using WarToilet.Entities;

namespace WarToilet.Utilities
{
    public static class TransformExtensions {
        private static Dictionary<Collider, IWeapon> weaponLookupFromCollider = new Dictionary<Collider, IWeapon>();
        private static Dictionary<Collider, IDangerous> dangerousLookupFromCollider = new Dictionary<Collider, IDangerous>();
        private static Dictionary<GameObject, Entity> entityLookupFromGameObject = new Dictionary<GameObject, Entity>();

        public static void Register(this Transform t)
        {
            Collider collider = t.GetComponentInChildren<Collider>();
            IWeapon weapon = t.GetComponent<IWeapon>();
            IDangerous dangerous = t.GetComponent<IDangerous>();
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
            IDangerous dangerous;
            dangerousLookupFromCollider.TryGetValue(collider, out dangerous);
            return dangerous;
        }

        public static Entity GetEntity(this GameObject go)
        {
            Entity entity;
            entityLookupFromGameObject.TryGetValue(go, out entity);
            return entity;
        }

        public static IWeapon GetWeapon(this Collider collider)
        {
            IWeapon weapon;
            weaponLookupFromCollider.TryGetValue(collider, out weapon);
            return weapon;
        }

    }
}
