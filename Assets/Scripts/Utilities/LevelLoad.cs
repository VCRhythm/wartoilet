using UnityEngine;

public class LevelLoad : MonoBehaviour {

    public GameObject levelToLoad;
    public Vector3 levelPosition;

    public void LoadLevel()
    {
        Instantiate(levelToLoad, transform.position + levelPosition, Quaternion.identity);
        GetComponent<Wall>().canPhaseThroughWall = true;
    }
}
