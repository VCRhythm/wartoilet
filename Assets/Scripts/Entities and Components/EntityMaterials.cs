using UnityEngine;

public class EntityMaterials : MonoBehaviour, IEntityObserver {

    public Material[] shittyMaterials;

    private Material originalMaterial;
    private SkinnedMeshRenderer meshRenderer;
    private const int maxShittinessLevel = 3;

    void Awake()
    {
        GetComponent<Entity>().RegisterObserver(this);

        meshRenderer = transform.FindChild("Ragdoll").GetComponentInChildren<SkinnedMeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    public void Stun(Vector3 position, int health)
    {
        SetShittiness(health);
    }

    public void UpdateHealth(int health)
    {
        SetShittiness(health);
    }

    public void Die(Vector3 position) { }
    public void UnStun() { }
    public void Move(Vector3 moveVector) { }

    private void SetShittiness(int level)
    {
        if (level > maxShittinessLevel)
        {
            meshRenderer.sharedMaterial = originalMaterial;
        }
        else
        {
            meshRenderer.sharedMaterial = shittyMaterials[Mathf.Clamp(maxShittinessLevel - level, 0, 3)];
        }
    }

}
