

using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour
{
    public bool isDead { get; private set; }
    public System.Action<Transform> OnKilled;


    [Header("Settings")]
    public bool isPlayer = false;
    public float destroyDelay = 1.5f;

    public void TakeHit()
    {
        if (isDead) return;

        if (isPlayer) 
        KillImmediate();


        else
            StartCoroutine(KillWithAnimation());
    }

    // =====================
    // PLAYER
    // =====================
    void KillImmediate()
    {
        isDead = true;

        OnKilled?.Invoke(transform);


        Destroy(gameObject);
    }

    // =====================
    // BOT
    // =====================
    IEnumerator KillWithAnimation()
    {
        isDead = true;

        OnKilled?.Invoke(transform);

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}