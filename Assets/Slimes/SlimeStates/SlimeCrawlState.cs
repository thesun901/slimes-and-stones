using UnityEngine;

public class SlimeCrawlState : SlimeState
{
    private float crawlTimer;
    private Vector2 crawlDirection;

    public SlimeCrawlState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        slime.animator.SetBool("isWalking", true);

        float dirX = Random.value < 0.5f ? -1f : 1f;
        crawlDirection = new Vector2(dirX, 0f).normalized;

        crawlTimer = Mathf.Lerp(1f, 2f, Random.value);
    }

    public override void UpdateState()
    {
        if (slime.currentHealth <= slime.stats.fleeHealthThreshold)
        {
            slime.ChangeState(slime.fleeState);
            return;
        }

        if (slime.IsPlayerInRange(slime.stats.attackRadius))
        {
            slime.ChangeState(slime.GetAttackState());
            return;
        }

        Vector2 vel = crawlDirection * slime.stats.crawlSpeed;
        if (slime.rb != null)
            slime.rb.linearVelocity = vel;
        else
            slime.transform.position += (Vector3)vel * Time.deltaTime;

        crawlTimer -= Time.deltaTime;
        if (crawlTimer <= 0f)
        {
            slime.ChangeState(slime.idleState);
        }
    }

    public override void ExitState()
    {
        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;
    }
}
