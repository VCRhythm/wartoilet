using UnityEngine;
using WarToilet.Interfaces;
using WarToilet.Items;

namespace WarToilet.Controllers
{
    public class AIController : MonoBehaviour, IController
    {
        public Vector3 GetMoveVector()
        {
            return Vector3.zero;
        }

        public Swing GetSwing()
        {
            return null;
        }

        public float GetWeaponPosition()
        {
            return 0f;
        }

        public bool ChangeTarget()
        {
            return false;
        }
    }
}
