using UnityEngine;
using TMPro;

public class MinimapUI : MonoBehaviour
{
    public PlayerManager playerManager;
    public TextMeshProUGUI nearbyBotText;
    public float detectRadius = 30f;

    public Transform detectCircle; // ลาก DetectCircle มาใส่

   void Update()
{
    if (playerManager.CurrentPlayer == null) return;

    int count = playerManager.CountNearbyBots(detectRadius);

    nearbyBotText.text = "Bots Nearby: " + count;

    foreach (var player in playerManager.players)
    {
        bool isCurrent = player == playerManager.CurrentPlayer;
        bool hasBot = isCurrent && count > 0;

        player.UpdateDetectCircle(hasBot);
    }
}
}