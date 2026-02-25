using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    public Image avatar;

    PlayerController player;
    Killable killable;

    Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void Setup(PlayerController p)
    {
        player = p;
        killable = p.GetComponent<Killable>();
    }

    void Update()
    {
        if (killable == null) return;

        if (killable.isDead)
        {
            SetAlpha(0.12f);
        }
        else
        {
            SetAlpha(1f);
        }
    }

    public void SetActive(bool active)
    {
        if (outline != null)
            outline.enabled = active;
    }

    void SetAlpha(float value)
    {
        Color c = avatar.color;
        c.a = value;
        avatar.color = c;
    }
}