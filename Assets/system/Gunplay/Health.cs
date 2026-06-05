using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Status Flags")]
    private bool isDead = false;

    [Header("Loot Drop")]
    public GameObject ammoPickupPrefab;
    private string ammoTypeToConfigure = "revolver";
    private int ammoAmountToConfigure = 6;

    [Header("UI Setup")]
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
        }

        UpdateHealthUI();
    }

    public void SetupLoot(string weaponKey, int amount)
    {
        ammoTypeToConfigure = weaponKey;
        ammoAmountToConfigure = amount;
    }

    public void TakeDamageFromPart(float amount, BodyPart.PartType part)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took global damage. Remaining HP: {currentHealth}");

        switch (part)
        {
            case BodyPart.PartType.HeadNeck:
                Debug.LogWarning("Critical hit to Head/Neck area!");
                break;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthUI();
        Debug.Log($"{gameObject.name} took generic damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (!gameObject.CompareTag("Player")) return;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            Canvas.ForceUpdateCanvases();

            healthSlider.enabled = false;
            healthSlider.enabled = true;

            Debug.Log("Слайдер примусово оновлено до: " + currentHealth);
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} destroyed!");

        if (healthSlider != null)
        {
            healthSlider.value = 0;
        }

        if (ammoPickupPrefab != null)
        {
            GameObject droppedBox = Instantiate(ammoPickupPrefab, transform.position + Vector3.up * 0.1f, Quaternion.identity);

            AmmoPickup pickupScript = droppedBox.GetComponent<AmmoPickup>();
            if (pickupScript != null)
            {
                pickupScript.weaponTargetNameKey = ammoTypeToConfigure;
                pickupScript.ammoAmount = ammoAmountToConfigure;
            }
        }

        Destroy(gameObject);
    }
}