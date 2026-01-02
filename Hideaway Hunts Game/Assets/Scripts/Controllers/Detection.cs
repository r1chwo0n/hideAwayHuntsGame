using UnityEngine;

public class Detection : MonoBehaviour
{
    public float viewDistance = 20f; // ระยะสูงสุดที่ bot จะสามารถมองเห็นได้
    public float viewAngle = 90f; // ซ้าย 45 องศา ขวา 45 องศา รวมเป็น 90 องศา
    public LayerMask targetMask; // เลเยอร์ของเป้าหมายที่ bot จะตรวจจับ
    public LayerMask obstacleMask; // obstacle layer for Raycast Checking
    // Raycast physical function ยิงลำแสงเพื่อเช็คว่าไปชนกับสิ่งกีดขวางหรือไม่

    public Transform DetectTarget() // คืน transform ของเป้าหมายตัวแรกที่เจอ
    // Tranform is a class that represents position, rotation and scale of an object in the scene
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewDistance, targetMask);
        // หาเป้าหมายที่อยู่ในรัศมี
        // transform.position คือ ตำแหน่งของ bot
        // viewDistance คือ รัศมีในการตรวจจับ
        // targetMask คือ เลเยอร์ของเป้าหมาย
        // Physics.OverlapSphere จะคืนค่าเป็น array ของ Collider ทั้งหมดที่อยู่ในรัศมีที่กำหนด

        foreach (var t in targets) // วนทุก collider ที่เจอ
        {
            // transform คือ component ที่อยู่ใน game object ที่มี collider นั้นๆ
            // normalized ได้ vector ที่มีความยาว 1
            // คืนค่า vector 3D ที่ชี้จาก bot ไปยังเป้าหมาย 
            Vector3 dirToTarget = (t.transform.position - transform.position).normalized;

            // Vector3.Angle(a,b) หามุมระหว่าง 2 vectors
            // transform.forward คือ ทิศที่ bot หันอยู่ (z local)
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            // ถ้ามุมระหว่าง bot กับเป้าหมาย อยู่ในมุมมองของ bot (ซ้ายขวาครึ่งนึงของ viewAngle)
            {
                float dist = Vector3.Distance(transform.position, t.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dist, obstacleMask))
                    return t.transform;   
            }
        }
        return null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
