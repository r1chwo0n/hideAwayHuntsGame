using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotSlotUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Image avatarImage;      // ถ้ามีรูป
    public Image deadOverlay;      // รูป X หรือสีทับแดง

    Killable killable;

    public void Setup(Killable k)
    {
        killable = k;

        nameText.text = k.gameObject.name;  // หรือดึงจาก script อื่นก็ได้
    }

    void Update()
    {
        if (killable == null) return;

        bool isDead = killable.isDead;

        // เปิด overlay ตอนตาย
        if (deadOverlay != null)
            deadOverlay.gameObject.SetActive(isDead);

        // ทำให้จางลงตอนตาย
        if (avatarImage != null)
        {
            Color c = avatarImage.color;
            c.a = isDead ? 0.3f : 1f;
            avatarImage.color = c;
        }

        if (nameText != null)
        {
            Color c = nameText.color;
            c.a = isDead ? 0.5f : 1f;
            nameText.color = c;
        }
    }
}