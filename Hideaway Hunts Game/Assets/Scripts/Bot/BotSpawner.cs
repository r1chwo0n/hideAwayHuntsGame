using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation; 

public class BotSpawner : MonoBehaviour
{
    public BotFormManager formManager;
    public NavMeshSurface navSurface; 

    void Start()
    {
        SpawnBots();
    }

    [ContextMenu("Randomize Positions")]
    public void SpawnBots()
    {
        if (navSurface == null || formManager == null) return;

        // ดึง Bounds (ขอบเขตสีฟ้า) จากตัว NavMesh Data โดยตรง
        Bounds navBounds = navSurface.navMeshData.sourceBounds;

        // ตำแหน่ง Center ของ Surface ในโลก World Space
        Vector3 surfaceCenter = navSurface.transform.position;

        foreach (Transform form in formManager.forms)
        {
            Vector3 randomTarget = GetValidPoint(surfaceCenter, navBounds);

            // ใช้ Warp สำหรับ NavMeshAgent เพื่อไม่ให้เกิดอาการดีดกลับ
            NavMeshAgent agent = form.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(randomTarget);
            }
            else
            {
                form.position = randomTarget;
            }

            Debug.Log($"Spawned {form.name} at {randomTarget}");
        }
    }

    Vector3 GetValidPoint(Vector3 center, Bounds bounds)
    {
        for (int i = 0; i < 30; i++) // ลองสุ่ม 30 ครั้งเพื่อให้เจอจุดที่อยู่บนพื้นสีฟ้าจริง ๆ
        {
            // สุ่มภายในขอบเขต X และ Z ของ Box Collider ที่คุณตั้งไว้
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 randomPos = new Vector3(center.x + x, center.y + 2f, center.z + z);

            // คำสั่งสำคัญ: เช็คว่าจุดที่สุ่ม "สัมผัสพื้นสีฟ้า" หรือไม่ ในระยะ 5 เมตร
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center; // ถ้าหาไม่เจอจริงๆ ให้เกิดตรงกลาง
    }
}