using UnityEngine;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Entities
{
public class EntityMaterials : EntityObserverBase {

    [SerializeField] private Material[] shittyMaterials;

    private Material originalMaterial;
    private SkinnedMeshRenderer meshRenderer;
    private const int maxShittinessLevel = 3;

    void Awake()
    {
        GetComponent<Entity>().RegisterObserver(this);

        meshRenderer = transform.Find("Ragdoll").GetComponentInChildren<SkinnedMeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    public override void Stun(Vector3 position, int health)
    {
        SetShittiness(health);
    }

    public override void UpdateHealth(int health)
    {
        SetShittiness(health);
    }

    private void SetShittiness(int level)
    {
        if (level > maxShittinessLevel)
        {
            meshRenderer.sharedMaterial = originalMaterial;
        }
        else
        {
            meshRenderer.sharedMaterial = shittyMaterials[Mathf.Clamp(maxShittinessLevel - level, 0, shittyMaterials.Length - 1)];
        }
    }

}
}
