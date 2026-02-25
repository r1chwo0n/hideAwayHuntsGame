using UnityEngine;
using TMPro;

public class MinimapUI : MonoBehaviour
{
    public PlayerManager playerManager;
    public TextMeshProUGUI nearbyBotText;
    public float detectRadius = 30f;
    public LayerMask BotLayer;

    void Update()
    {
        if (playerManager.CurrentPlayer == null) return;

        Vector3 playerPos = playerManager.CurrentPlayer.transform.position;

        Collider[] hits = Physics.OverlapSphere(
            playerPos,
            detectRadius,
            BotLayer
        );

        int count = hits.Length;

        nearbyBotText.text = "Bots Nearby: " + count;
    }
}