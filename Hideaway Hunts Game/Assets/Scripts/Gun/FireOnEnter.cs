using UnityEngine;

public class FireOnEnter : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        GunShooter gun = animator.GetComponent<GunShooter>();
        if (gun != null)
        {
            gun.Fire();
        }
    }
}
