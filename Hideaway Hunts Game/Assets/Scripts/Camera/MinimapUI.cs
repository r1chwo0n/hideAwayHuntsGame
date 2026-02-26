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

    // ให้ DetectCircle ตาม player
    Vector3 playerPos = playerManager.CurrentPlayer.transform.position;

    detectCircle.position = new Vector3(
        playerPos.x,
        detectCircle.position.y,
        playerPos.z
    );

    detectCircle.gameObject.SetActive(count > 0);
}
}