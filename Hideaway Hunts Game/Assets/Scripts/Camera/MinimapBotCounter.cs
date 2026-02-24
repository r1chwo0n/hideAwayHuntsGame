using UnityEngine;
using TMPro;

public class MinimapBotCounter : MonoBehaviour
{
    public PlayerManager playerManager;
    public TMP_Text botText;
    public float radius = 30f;

    void Update()
    {
        int count = playerManager.CountNearbyBots(radius);
        botText.text = "Nearby Bots: " + count;
    }
}