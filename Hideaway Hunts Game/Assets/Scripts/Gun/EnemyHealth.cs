using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("ศัตรูโดนยิง! เหลือเลือด: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
