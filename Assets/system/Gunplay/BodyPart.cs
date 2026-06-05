using System.Collections;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public enum PartType { HeadNeck, BodyArm, PelvisLegs }

    [Header("Body Part Settings")]
    public PartType partType;
    public float partMaxHealth = 100f;
    public float partCurrentHealth;

    [Header("Damage Multiplier")]
    public float damageMultiplier = 1.0f;

    [Header("Visual Flash Settings")]
    [Tooltip("The actual Collider game object that will flash to highlight the zone.")]
    public GameObject colliderVisualObject;
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;

    private Health mainHealth;
    private bool isDestroyed = false;
    private Material debugMaterial;
    private MeshRenderer visualRenderer;
    private Color originalColor;

    private void OnValidate()
    {
        switch (partType)
        {
            case PartType.HeadNeck: damageMultiplier = 2.0f; break;
            case PartType.BodyArm: damageMultiplier = 0.8f; break;
            case PartType.PelvisLegs: damageMultiplier = 1.2f; break;
        }
    }

    void Start()
    {
        partCurrentHealth = partMaxHealth;
        mainHealth = GetComponentInParent<Health>();

        if (mainHealth == null)
        {
            Debug.LogError($"[BodyPart] Main Health script not found in parents for: {gameObject.name}!");
        }

        if (colliderVisualObject == null)
        {
            colliderVisualObject = gameObject;
        }

        visualRenderer = colliderVisualObject.GetComponent<MeshRenderer>();
        if (visualRenderer != null)
        {
            debugMaterial = visualRenderer.material;
            if (debugMaterial.HasProperty("_BaseColor"))
                originalColor = debugMaterial.GetColor("_BaseColor");
            else if (debugMaterial.HasProperty("_Color"))
                originalColor = debugMaterial.color;

            visualRenderer.enabled = false;
        }
        else
        {
            Debug.LogError($"[BodyPart] Missing MeshRenderer component on {colliderVisualObject.name}! Flash visualization will not work.");
        }
    }

    public void TakePartDamage(float amount)
    {
        if (isDestroyed) return;

        float finalDamage = amount * damageMultiplier;
        partCurrentHealth -= finalDamage;
        partCurrentHealth = Mathf.Clamp(partCurrentHealth, 0, partMaxHealth);

        TriggerFlash();

        if (mainHealth != null)
        {
            mainHealth.TakeDamageFromPart(finalDamage, partType);
        }

        if (partCurrentHealth <= 0)
        {
            DestroyPart();
        }
    }

    private void TriggerFlash()
    {
        if (visualRenderer == null)
        {
            Debug.LogError($"[BodyPart DEBUG] {gameObject.name} cannot flash because visualRenderer is null!");
            return;
        }

        Debug.Log($"[BodyPart DEBUG] {gameObject.name} is starting flash coroutine. Initial state Renderer.enabled = {visualRenderer.enabled}");

        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        visualRenderer.enabled = true;
        Debug.Log($"[BodyPart DEBUG] {gameObject.name} set Mesh Renderer enabled to true. Testing shader properties...");

        bool colorChanged = false;

        if (debugMaterial.HasProperty("_BaseColor"))
        {
            debugMaterial.SetColor("_BaseColor", flashColor);
            Debug.Log($"[BodyPart DEBUG] {gameObject.name}: Successfully applied flashColor to '_BaseColor'");
            colorChanged = true;
        }

        if (debugMaterial.HasProperty("_Color"))
        {
            debugMaterial.color = flashColor;
            Debug.Log($"[BodyPart DEBUG] {gameObject.name}: Successfully applied flashColor to '_Color'");
            colorChanged = true;
        }

        if (!colorChanged)
        {
            Debug.LogWarning($"[BodyPart DEBUG] {gameObject.name}: Material shader '{debugMaterial.name}' does not support standard '_Color' or '_BaseColor' modifications!");
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log($"[BodyPart DEBUG] {gameObject.name}: Flash duration expired. Reverting color and disabling visibility.");

        if (debugMaterial.HasProperty("_BaseColor"))
            debugMaterial.SetColor("_BaseColor", originalColor);

        if (debugMaterial.HasProperty("_Color"))
            debugMaterial.color = originalColor;

        visualRenderer.enabled = false;
    }

    private void DestroyPart()
    {
        isDestroyed = true;
        Debug.LogWarning($"[BodyPart] {partType} has been critically damaged!");
    }
}