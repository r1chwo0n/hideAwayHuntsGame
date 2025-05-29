using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] players;
    public ThirdPersonCameraWithCollision cameraFollow;

    void Start()
    {
        SwitchToPlayer(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToPlayer(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToPlayer(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToPlayer(2);
    }

    public void SwitchToPlayer(int index)
{
    if (index < 0 || index >= players.Length)
    {
        Debug.LogError("Index out of range");
        return;
    }

    if (players[index] == null)
    {
        Debug.LogError("Player at index is missing!");
        return;
    }

    if (cameraFollow == null)
    {
        Debug.LogError("⚠️ CameraFollow script is not assigned!");
        return;
    }

   // Keep all characters active
    for (int i = 0; i < players.Length; i++)
    {
        players[i].SetActive(true);

        var movement = players[i].GetComponent<PlayerController>();
        if (movement != null)
            movement.enabled = (i == index); // Only enable control for the selected one
    }
    
    // Update camera to follow the new player
        cameraFollow.target = players[index].transform;
}

}
