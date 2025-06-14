using UnityEngine;

public class SlimeMeleeAttackState : SlimeState
{
    private float attackTimer;

    public SlimeMeleeAttackState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        attackTimer = slime.stats.attackCooldown;

        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;

    }

    public override void UpdateState()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;

        if (slime.IsPlayerInRange(slime.stats.attackRadius))
            slime.ChangeState(slime.meleeAttackState);
        else
            slime.ChangeState(slime.chaseState);
    }

    public override void ExitState()
    {

    }
}
