using UnityEngine;

public class SlimeFleeState : SlimeState
{
    public SlimeFleeState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        slime.animator.SetBool("isRunning", true);
        slime.animator.SetBool("isWalking", false);
        slime.animator.SetBool("isAttacking", false);
    }

    public override void UpdateState()
    {
        Vector2 directionAwayFromPlayer = (slime.transform.position - slime.player.position).normalized;
        Vector2 fleeVelocity = directionAwayFromPlayer * slime.stats.runSpeed;

        if (slime.rb != null)
            slime.rb.linearVelocity = fleeVelocity;
        else
            slime.transform.position += (Vector3)fleeVelocity * Time.deltaTime;
    }

    public override void ExitState()
    {
        slime.animator.SetBool("isRunning", false);
        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;
    }
}
