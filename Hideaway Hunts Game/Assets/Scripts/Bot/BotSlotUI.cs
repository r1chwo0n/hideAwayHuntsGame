using UnityEngine;
using UnityEngine.UI;

public class BotSlotUI : MonoBehaviour
{
    public Image deadOverlay;

    Killable killable;

    public void Setup(Killable k)
    {
        killable = k;
    }

    void Update()
    {
        if (killable != null)
            deadOverlay.gameObject.SetActive(killable.isDead);
    }
}