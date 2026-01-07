using UnityEngine;

public class Health : MonoBehaviour
{
    public float hp = 100f;

    public System.Action<Transform> OnDeath;

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        OnDeath?.Invoke(transform);
        Destroy(gameObject);
    }
}
