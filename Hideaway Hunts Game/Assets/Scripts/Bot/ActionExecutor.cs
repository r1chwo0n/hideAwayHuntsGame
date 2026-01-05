using UnityEngine;

public class ActionExecutor : MonoBehaviour
{
    [Header("References")]
    public Transform actor;                 // ร่างที่กำลัง active
    public CharacterController movement;    // หรือ NavMeshAgent
    public GunShooter gun;                  // ยิง
    //public CoverSystem cover;               // (ถ้ามี)
    public Animator animator;

    [Header("State")]
    public ActionType currentAction;

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
        if (gun) gun.enabled = false;

        if (animator)
            animator.SetInteger("ActionState", (int)action);
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

    void DoAttack()
    {
        if (gun)
            gun.enabled = true; // GunShooter จะยิงเอง
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
