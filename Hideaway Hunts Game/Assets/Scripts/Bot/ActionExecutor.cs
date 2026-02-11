using UnityEngine;
using UnityEngine.AI;

public enum ActionType
{
    Idle,
    Patrol,
    Attack,
    Retreat,
    Flank,
    Defend
}

public class ActionExecutor : MonoBehaviour
{
    [Header("References")]
    public Transform actor;                 // ร่างที่กำลัง active
    public NavMeshAgent agent;
    public GunShooter gun;                  // ยิง
    //public CoverSystem cover;               // (ถ้ามี)
    public Animator animator;

    [Header("State")]
    public ActionType currentAction;

    [Header("Combat")]
    public Transform target;

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

            //case ActionType.Flank:
            //    DoFlank();
            //    break;

            //case ActionType.Defend:
            //    DoDefend();
            //    break;
        }
        UpdateMovementAnimation();
    }

    void UpdateMovementAnimation()
    {
        if (!animator || !agent) return;

        float speed01 = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speed01);
    }

    // ===== Action implementations =====

    void DoIdle()
    {
        if (agent)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }


    void DoPatrol()
    {
        if (!agent) return;

        agent.isStopped = false;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 random = Random.insideUnitSphere * 5f;
            random += actor.position;

            if (NavMesh.SamplePosition(random, out var hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void DoAttack()
    {
        if (!agent || target == null)
            return;

        agent.isStopped = false;
        agent.SetDestination(target.position);

        Vector3 dir = target.position - actor.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            actor.rotation = Quaternion.LookRotation(dir);

        if (animator)
            animator.SetBool("IsFiring", true);

        if (gun)
            gun.Fire();
    }

    void DoRetreat()
    {
        if (!agent || target == null)
            return;

        agent.isStopped = false;

        Vector3 awayDir = (actor.position - target.position).normalized;
        Vector3 retreatPos = actor.position + awayDir * 4f; // ระยะถอย

        if (NavMesh.SamplePosition(retreatPos, out var hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    void DoFlank()
    {
        if (!agent || target == null)
            return;

        agent.isStopped = false;

        bool goRight = Random.value > 0.5f;
        float dir = goRight ? 1f : -1f;

        Vector3 toTarget = (target.position - actor.position).normalized;
        Vector3 flankDir =
            goRight
            ? Vector3.Cross(Vector3.up, toTarget)
            : Vector3.Cross(toTarget, Vector3.up);

        Vector3 flankPos = actor.position + flankDir * 3f;

        if (NavMesh.SamplePosition(flankPos, out var hit, 3f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        animator.SetFloat("FlankDir", dir);
    }

    void DoDefend()
    {
        if (!agent)
            return;

        agent.isStopped = true;
        agent.ResetPath();

        if (target)
        {
            Vector3 dir = target.position - actor.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                actor.rotation = Quaternion.LookRotation(dir);
        }
    }

}
