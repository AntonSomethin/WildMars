using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Table 2 Stats")]
    public float maxHealth;    // (%)
    public float armor;        // (%)

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} got {amount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} deactivated!");
        Destroy(gameObject);
    }
}