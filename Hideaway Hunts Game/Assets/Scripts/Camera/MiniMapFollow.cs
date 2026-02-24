using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public PlayerManager playerManager;
    public float height = 15f;   // ความสูงของกล้อง

    void LateUpdate()
    {
        if (playerManager == null) return;
        if (playerManager.CurrentPlayer == null) return;

        Transform target = playerManager.CurrentPlayer.transform;

        // ตามตำแหน่ง X Z ของ player
        transform.position = new Vector3(
            target.position.x,
            target.position.y + height,
            target.position.z
        );

        // มองลงตรง ๆ
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}