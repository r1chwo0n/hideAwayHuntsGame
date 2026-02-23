using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour
{
    public bool isDead { get; private set; }
    // event ที่จะถูกเรียกเมื่อถูก kill โดยส่ง Transform ของตัวที่ถูกฆ่าไปด้วย
    public System.Action<Transform> OnKilled;

    [Header("Settings")]
    public bool isPlayer = false; // ติ๊กถูกที่ Inspector ถ้าเป็น Player
    public float destroyDelay = 0.5f; // เวลาให้เล่น Animation ก่อนตาย

    //public void Kill()
    //{
    //    if (isDead) return;

    //    isDead = true; 
    //    OnKilled?.Invoke(transform); // บอกทุกคนที่ติดตามว่า ถูก kill ละ พร้อมส่ง Transform ของตัวเองไปด้วย
    //    Destroy(gameObject); // ลบวัตถุออกจากฉาก
    //}

    public void TakeHit()
    {
        if (isDead) return;

        if (isPlayer)
        {
            // ถ้าเป็น Player ให้ตายทันที หายไปเลย
            KillImmediate();
        }
        else
        {
            // ถ้าเป็น Bot ให้เล่นท่าทางก่อน
            StartCoroutine(KillWithAnimation());
        }
    }

    private void KillImmediate()
    {
        isDead = true;
        OnKilled?.Invoke(transform); // บอกทุกคนที่ติดตามว่า ถูก kill ละ พร้อมส่ง Transform ของตัวเองไปด้วย
        Destroy(gameObject); // ลบวัตถุออกจากฉาก
    }

    private IEnumerator KillWithAnimation()
    {
        isDead = true;
        OnKilled?.Invoke(transform);

        // สั่งเล่น Animation "GetHit" หรือ "Die"
        // เราดึง Animator จากตัวมันเองหรือลูกของมัน
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die"); // หรือชื่อ Parameter ที่คุณตั้งไว้ใน Animator
        }

        // รอให้ Animation เล่นไปสักพัก
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}
