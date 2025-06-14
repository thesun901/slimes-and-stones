using UnityEngine;

public class SlimeShootAttackState : SlimeState
{
    private float shootTimer;

    public SlimeShootAttackState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        shootTimer = slime.stats.attackCooldown;
    }

    public override void UpdateState()
    {
        if (slime.IsPlayerInRange(0.5f * slime.stats.attackRadius))
        {
            Vector2 directionAwayFromPlayer = (slime.transform.position - slime.player.position).normalized;
            Vector2 fleeVelocity = directionAwayFromPlayer * slime.stats.runSpeed;

            if (slime.rb != null)
                slime.rb.linearVelocity = fleeVelocity;
            else
                slime.transform.position += (Vector3)fleeVelocity * Time.deltaTime;
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer > 0) return;

        SpawnProjectile();

        if (slime.IsPlayerInRange(slime.stats.attackRadius))
            slime.ChangeState(slime.shootAttackState);

        else
            slime.ChangeState(slime.GetMovementState());
    }

    public override void ExitState()
    {

    }

    private void SpawnProjectile()
    {
        if (slime.stats.attackMode != SlimeStats.AttackMode.Shoot) return;

        GameObject proj = GameObject.Instantiate(
            slime.projectilePrefab,
            slime.gameObject.transform.position,
            Quaternion.identity
        );

    }
}
