using UnityEngine;

public class BotBody : MonoBehaviour
{
    public int health = 100;

    private bool isDead = false;

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // ปิดตัว bot
        gameObject.SetActive(false);

        Debug.Log(name + " is dead");
    }
}