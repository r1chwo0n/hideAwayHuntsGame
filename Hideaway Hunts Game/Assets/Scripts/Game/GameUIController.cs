using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text lifeText;

    [Header("Player")]
    public PlayerManager playerManager;

    private int lifeCount;

    void Start()
    {
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager not assigned!");
            return;
        }
        playerManager.OnLifeChanged += UpdateLifeUI;
        playerManager.OnAmmoChanged += UpdateAmmoUI;

        
        UpdateAmmoUI(playerManager.sharedAmmo);
        UpdateLifeUI(playerManager.AliveCount);
    }

    void UpdateLifeUI(int value)
    {
        lifeText.text = value.ToString();
    }

    void UpdateAmmoUI(int value)
    {
        ammoText.text = value.ToString();
    }

    public void SetAmmo(int value)
    {
        ammoText.text = value.ToString();
    }
}