using UnityEngine;

public class Killable : MonoBehaviour
{
    public bool isDead { get; private set; }

    public System.Action<Transform> OnKilled;

    public void Kill()
    {
        if (isDead) return;

        isDead = true;
        OnKilled?.Invoke(transform);
        Destroy(gameObject);
    }

    public void TakeHit()
    {
        Kill();
    }
}
