using UnityEngine;
using UnityEngine.AI;

public class ActionExecutor : MonoBehaviour
{
    [Header("References")]
    public Transform actor;                 // ร่างที่กำลัง active
    //public CharacterController movement;    // หรือ NavMeshAgent
    public NavMeshAgent agent;
    public GunShooter gun;                  // ยิง
    //public CoverSystem cover;               // (ถ้ามี)
    public Animator animator;

    [Header("State")]
    public ActionType currentAction;

    [Header("Combat")]
    public Transform target;

    //void Awake()
    //{
    //    if (agent == null)
    //        agent = GetComponent<NavMeshAgent>();
    //}

    void Awake()
    {
        currentAction = ActionType.Idle;
        target = null;
    }


    public void SetActor(Transform newActor)
    {
        actor = newActor;

        if (!actor)
            return;

        agent = actor.GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = actor.GetComponentInChildren<Animator>();
        gun = actor.GetComponentInChildren<GunShooter>();
    }


    public void SetTarget(Transform t)
    {
        target = t;

        if (gun)
            gun.target = t;   // ถ้า GunShooter รองรับ target
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

        animator.SetBool("IsFiring", false);

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

            case ActionType.Flank:
                DoFlank();
                break;

            case ActionType.Defend:
                DoDefend();
                break;
        }
    }

    // ===== Action implementations =====

    void DoIdle()
    {
        // ยืนเฉย / หันกล้อง
    }

    void DoPatrol()
    {
        // เดินสุ่ม หรือ waypoint
        // movement.Move(...)
    }

    //void DoAttack()
    //{
    //    if (!animator || !gun) return;

    //    animator.SetBool("IsFiring", true);

    //    gun.Fire();
    //}

    void DoAttack()
    {
        if (!animator || !gun || target == null)
            return;

        // หันหน้าไปหาเป้า
        Vector3 dir = target.position - actor.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            actor.rotation = Quaternion.LookRotation(dir);

        animator.SetBool("IsFiring", true);
        gun.Fire();
    }



    void DoRetreat()
    {
        // ถอยจากศัตรู
        // movement.Move(-forward)
    }

    void DoFlank()
    {
        // เคลื่อนด้านข้าง
        // movement.Move(right)
    }

    void DoDefend()
    {
        // หา cover / หมอบ
        //if (cover)
        //    cover.TakeCover();
    }
}
