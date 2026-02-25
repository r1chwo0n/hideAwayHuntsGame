using UnityEngine;
using TMPro;

public class MinimapUI : MonoBehaviour
{
    public PlayerManager playerManager;
    public TextMeshProUGUI nearbyBotText;
    public float detectRadius = 30f;

    void Update()
    {
        if (playerManager.CurrentPlayer == null) return;

        Vector3 playerPos = playerManager.CurrentPlayer.transform.position;

        Killable[] bots = FindObjectsByType<Killable>(FindObjectsSortMode.None);

        int count = 0;

        foreach (var bot in bots)
        {
            if (Vector3.Distance(playerPos, bot.transform.position) <= detectRadius)
                count++;
        }

        nearbyBotText.text = "Bots Nearby: " + count;
    }
}