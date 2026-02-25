using UnityEngine;

public class TopBarUI : MonoBehaviour
{
    public PlayerManager playerManager;

    public Transform playerParent;
    public Transform botParent;

    public GameObject playerSlotPrefab;
    public GameObject botSlotPrefab;

    public Killable[] bots;

    PlayerSlotUI[] playerSlots;

    void Start()
    {
        SetupPlayers();
        SetupBots();

        playerManager.OnLifeChanged += OnLifeChanged;
    }

    

    void SetupPlayers()
    {
        int count = playerManager.players.Length;
        playerSlots = new PlayerSlotUI[count];

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(playerSlotPrefab, playerParent);
            var slot = obj.GetComponent<PlayerSlotUI>();
            slot.Setup(playerManager.players[i]);
            playerSlots[i] = slot;
        }
    }

    void SetupBots()
    {
        foreach (var bot in bots)
        {
            var obj = Instantiate(botSlotPrefab, botParent);
            obj.GetComponent<BotSlotUI>().Setup(bot);
        }
    }

    void Update()
    {
        // highlight current player
        for (int i = 0; i < playerSlots.Length; i++)
        {
            bool isActive = 
                playerManager.players[i] == playerManager.CurrentPlayer;

            playerSlots[i].SetActive(isActive);
        }
    }

    void OnLifeChanged(int aliveCount)
    {
        Debug.Log("Players alive: " + aliveCount);
    }
}