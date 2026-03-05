using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotUI : MonoBehaviour
{
    public Image avatarImage;
    public TMP_Text nameText;
    public Image background;

    PlayerController player;
    Killable killable;

    Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        if (killable == null) return;

        if (killable.isDead)
        {
            SetAlpha(0.3f);
            if (outline != null)
                outline.enabled = false;
        }
        else
        {
            SetAlpha(1f);
        }
    }

    public void Setup(PlayerController p)
    {
        player = p;
        killable = p.GetComponent<Killable>();

        nameText.text = p.name;

        if (avatarImage != null && p.avatarSprite != null)
            avatarImage.sprite = p.avatarSprite;
    }

    public void SetActive(bool active)
    {
        if (outline == null) return;

        if (killable != null && killable.isDead)
            outline.enabled = false;
        else
            outline.enabled = active;
    }

    void SetAlpha(float value)
    {
        if (avatarImage == null) return;

        Color c = avatarImage.color;
        c.a = value;
        avatarImage.color = c;
    }
}