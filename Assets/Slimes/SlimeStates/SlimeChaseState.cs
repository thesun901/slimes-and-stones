using UnityEngine;

public class SlimeChaseState : SlimeState
{
    public SlimeChaseState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        slime.animator.SetBool("isWalking", true);
        slime.animator.SetBool("isRunning", false);
        slime.animator.SetBool("isAttacking", false);
    }

    public override void UpdateState()
    {


        slime.animator.SetBool("isRunning", true);
        Vector2 dir = (slime.player.position - slime.transform.position).normalized;
        Vector2 vel = dir * slime.stats.chaseSpeed;

        if (slime.rb != null)
            slime.rb.linearVelocity = vel;
        else
            slime.transform.position += (Vector3)vel * Time.deltaTime;


        if(slime.IsPlayerInRange(slime.stats.attackRadius))
        {
            slime.ChangeState(slime.GetAttackState());
        }
    }

    public override void ExitState()
    {
        slime.animator.SetBool("isRunning", false);
        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;
    }
}
