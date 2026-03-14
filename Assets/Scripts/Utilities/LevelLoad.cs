using UnityEngine;

namespace WarToilet.Utilities
{
    public class LevelLoad : MonoBehaviour {

        [SerializeField] private GameObject levelToLoad;
        [SerializeField] private Vector3 levelPosition;

        public void LoadLevel()
        {
            Instantiate(levelToLoad, transform.position + levelPosition, Quaternion.identity);
            GetComponent<Wall>().canPhaseThroughWall = true;
        }
    }
}
