using UnityEngine;
using UnityEngine.AI;

public enum ActionType
{
    Idle,
    Patrol,
    Attack,
    Retreat,
    Flank,
    Defend // ยึดตำแหน่งและคุมมุม
}

public class ActionExecutor : MonoBehaviour
{
    [Header("References")]
    public Transform actor;                 // ร่างที่กำลัง active
    public NavMeshAgent agent;
    public GunShooter gun;                  // ยิง
    public Animator animator;

    [Header("State")]
    public ActionType currentAction;

    [Header("Combat")]
    public Transform target;

    private float aimTimer = 0f;
    public float aimTimeRequired = 0.35f; // เวลาเผื่อเล็ง
    public float reactionDelay = 0.2f;
    private float reactionTimer = 0f;

    public PerceptionController perception;

    private Vector3 lastKnownEnemyPosition;
    private bool hasLastKnownPosition; // ยังจำได้อยู่มั้ยว่าศัตรูอยู่ตรงไหน
    private float memoryDuration = 6f; // ระยะเวลาที่จำได้
    private float lastSeenTime; // เวลาที่เห็นศัตรูล่าสุด

    void Awake()
    {
        currentAction = ActionType.Idle;
        target = null;
    }

    public void SetActor(Transform newActor)
    {
        actor = newActor;

        currentAction = ActionType.Idle;
        target = null;

        if (!actor)
            return;

        agent = actor.GetComponent<NavMeshAgent>();
        animator = actor.GetComponentInChildren<Animator>();
        gun = actor.GetComponentInChildren<GunShooter>();
        perception = actor.GetComponent<PerceptionController>();
    }

    public void SetTarget(Transform t)
    {
        // กัน null ก่อนเสมอ
        if (t == null)
        {
            target = null;

            if (gun)
                gun.target = null;

            return;
        }

        // กันกรณีตายแล้ว
        if (t.GetComponent<Killable>()?.isDead == true)
        {
            target = null;

            if (gun)
                gun.target = null;

            return;
        }

        // อัปเดต memory
        lastKnownEnemyPosition = t.position;
        hasLastKnownPosition = true;
        lastSeenTime = Time.time;

        target = t;

        if (gun)
            gun.target = t;
    }


    public void Execute(ActionType action)
    {
        if (action != currentAction)
        {
            currentAction = action;
            OnActionChanged(action);
        }

        TickAction(action);
    }

    void OnActionChanged(ActionType action)
    {
        if (!animator) return;

        animator.SetInteger("ActionState", (int)action);

        //animator.SetBool("IsFiring", false);

        if (gun)
            gun.enabled = (action == ActionType.Attack);
    }

    void TickAction(ActionType action)
    {
        switch (action)
        {
            case ActionType.Idle:
                DoIdle();
                break;

            case ActionType.Patrol:
                DoPatrol();
                break;

            case ActionType.Attack:
                DoAttack();
                break;

            case ActionType.Retreat:
                DoRetreat();
                break;

            case ActionType.Defend:
                DoDefend();
                break;
        }
        UpdateMovementAnimation();
    }

    void UpdateMovementAnimation()
    {
        if (!animator || !agent) return;

        float speed_norm = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speed_norm);
    }

    // ===== Action implementations =====
    // Navmesh ใช้ A* หาเส้นทางที่สั้นที่สุด (Shortest path) จากจุดเริ่มต้นไปที่จุดหมาย โดยเดินผ่านพื้นที่ที่เดินได้เท่านั้น
    void DoIdle()
    {
        if (agent)
        {
            agent.isStopped = true;
            agent.ResetPath(); // ลบ path ที่เคยคำนวณเอาไว้
        }
    }

    void DoPatrol()
    {
        if (!agent || !actor) return;
        // ถ้าเวลาเกิน memory duration แล้ว → ลืมตำแหน่งสุดท้ายที่เคยเห็นศัตรู
        if (hasLastKnownPosition && Time.time - lastSeenTime > memoryDuration)
        {
            hasLastKnownPosition = false;
        }

        agent.isStopped = false;
        // ถ้าไม่มี path หรือเดินไปเกือบถึงจุดหมายแล้ว → หาเป้าหมายใหม่
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 patrolTarget;

            if (hasLastKnownPosition) // มีตำแหน่งศัตรูล่าสุดในความทรงจำ
            {
                // เดินไปทิศที่เคยเห็นศัตรู
                Vector3 toLast = lastKnownEnemyPosition - actor.position;
                if (toLast.sqrMagnitude < 0.5f) // ถ้าใกล้มากแล้ว ลืมมันไป ไม่ต้องเดินไปตรงนั้นอีก 
                {
                    hasLastKnownPosition = false;
                    return;
                }
                Vector3 dir = toLast.normalized; // เปลี่ยนเป็นทิศทาง

                // เพิ่ม lateral เล็กน้อย ไม่ให้ตรงเกินไป
                Vector3 side = Vector3.Cross(Vector3.up, dir); // cross product เพื่อหาทิศทางด้านข้าง
                patrolTarget = actor.position + dir * 6f + side * Random.Range(-3f, 3f);
            }
            else // กรณีไม่เคยเห็นใครเลย หรือ ลืมตำแหน่งสุดท้ายที่เคยเห็นแล้ว
            {
                // ถ้ายังไม่เคยเห็นใคร → สำรวจพื้นที่รอบตัว
                Vector3 random = Random.insideUnitSphere * 8f;
                random.y = 0f;
                patrolTarget = actor.position + random;
            }
            // ตรวจ navmesh ว่าจุดที่สุ่มมาเดินได้จริงไหม ถ้าไม่ได้ให้ลองใหม่ (ถ้าใกล้ ๆ กันก็ยังโอเค)
            if (NavMesh.SamplePosition(patrolTarget, out var hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void DoAttack()
    {
        if (!agent || target == null || gun == null || perception == null)
            return;

        float dist = Vector3.Distance(actor.position, target.position);

        // 1. หมุนตัวแนวราบ
        Vector3 flatDir = target.position - actor.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir);
            actor.rotation = Quaternion.Slerp(
                actor.rotation,
                targetRot,
                Time.deltaTime * 7f
            );
        }
        // forward คือทิศที่ตัวละครกำลังหันหน้าไป และ flatDir คือทิศทางจากตัวละครไปยังเป้าหมาย
        float angle = Vector3.Angle(actor.forward, flatDir);
        
        // 2. ถ้าเกินระยะยิง → เข้าใกล้
        if (dist > gun.shootRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            aimTimer = 0f;
            return;
        }

        // 3. อยู่ในระยะยิง → หยุด
        agent.isStopped = true;

        // 4. ต้องมี LOS จริง
        bool hasLOS = perception.HasLineOfSight(target); 

        if (!hasLOS)
        {
            aimTimer = 0f;
            return;
        }

        // 5. เล็งตรง + ตั้งศูนย์
        if (angle < 5f)
        {
            reactionTimer += Time.deltaTime; // reaction time ก่อนยิง

            if (reactionTimer < reactionDelay)
                return;

            aimTimer += Time.deltaTime;

            if (aimTimer >= aimTimeRequired && gun.CanFire())
            {
                gun.Fire();

                if (animator)
                    animator.SetTrigger("Fire");

                RepositionAfterShot();

                aimTimer = 0f;
                reactionTimer = 0f;
            }
        }
        else
        {
            aimTimer = 0f;
            reactionTimer = 0f;
        }

    }

    void RepositionAfterShot()
    {
        Vector3 side = Vector3.Cross(Vector3.up, actor.forward);
        float dir = Random.value > 0.5f ? 1f : -1f;

        Vector3 newPos = actor.position + side * dir * 4f;

        if (NavMesh.SamplePosition(newPos, out var hit, 3f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    void DoRetreat()
    {
        if (!agent || !actor || target == null || perception == null)
            return;

        float dist = Vector3.Distance(actor.position, target.position);
        // ทิศหนีออกจากศัตรู
        Vector3 awayDir = (actor.position - target.position).normalized;
        // อยากให้เปลี่ยนตามอะไร
        bool panic = dist < 6f; // threshold ปรับตรงนี้

        if (panic)
        {
            // Panic Mode → หันหลังวิ่ง
            actor.rotation = Quaternion.Slerp(
                actor.rotation,
                Quaternion.LookRotation(awayDir),
                Time.deltaTime * 8f
            );
        }
        else
        {
            // Tactical Retreat → ยังหันหน้า
            Vector3 flatDir = target.position - actor.position;
            flatDir.y = 0f;

            actor.rotation = Quaternion.Slerp(
                actor.rotation,
                Quaternion.LookRotation(flatDir),
                Time.deltaTime * 6f
            );
        }

        bool hasLOS = perception.HasLineOfSight(target);

        if (!hasLOS)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 destination;

            if (TryFindCover(out Vector3 coverPos))
            {
                destination = coverPos;
            }
            else
            {
                destination = actor.position + awayDir * (panic ? 8f : 6f);
            }

            if (NavMesh.SamplePosition(destination, out var hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    // หา cover แล้วส่งตำแหน่งกลับ
    bool TryFindCover(out Vector3 coverPosition)
    {
        coverPosition = Vector3.zero;

        if (!target || !perception)
            return false;

        Vector3 awayDir = (actor.position - target.position).normalized;

        for (int i = 0; i < 8; i++)
        {
            Vector3 side = Vector3.Cross(Vector3.up, awayDir);
            Vector3 randomOffset =
                awayDir * Random.Range(4f, 8f) +
                side * Random.Range(-4f, 4f);

            Vector3 candidate = actor.position + randomOffset;

            if (NavMesh.SamplePosition(candidate, out var hit, 5f, NavMesh.AllAreas))
            {
                // sim เช็คดู
                bool hasLOS =
                    perception.HasLineOfSightFromPosition(hit.position, target);

                if (!hasLOS)
                {
                    coverPosition = hit.position;
                    return true;
                }
            }
        }

        return false;
    }

    void DoDefend() // ไม่เคลื่อนที่ ยังไม่จำเป็นต้องหนี
    {
        if (!agent || !actor)
            return;

        agent.isStopped = true;

        if (target)
        {
            Vector3 dir = target.position - actor.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                actor.rotation = Quaternion.Slerp(
                    actor.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * 6f
                );
            }

            if (perception && gun)
            {
                bool hasLOS = perception.HasLineOfSight(target);
                float dist = Vector3.Distance(actor.position, target.position);

                if (hasLOS && dist <= gun.shootRange)
                {
                    gun.Fire();
                    animator?.SetTrigger("Fire");
                }
            }
        }
    }

}
