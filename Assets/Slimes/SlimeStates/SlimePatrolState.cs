using UnityEngine;

public class SlimePatrolState : SlimeState
{
    private float patrolTimer;
    private Vector2 patrolDirection;

    public SlimePatrolState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        slime.animator.SetBool("isWalking", true);

        // Losujemy losowy kierunek (np. -1 lub +1 w osi X)
        float dirX = Random.value < 0.5f ? -1f : 1f;
        patrolDirection = new Vector2(dirX, 0f).normalized;

        patrolTimer = Mathf.Lerp(1f, 3f, Random.value); 
    }

    public override void UpdateState()
    {
        if (slime.IsPlayerInRange(slime.stats.detectionRadius))
        {
            if (slime.currentHealth <= slime.stats.fleeHealthThreshold)
                slime.ChangeState(slime.fleeState);
            else
                slime.ChangeState(slime.GetMovementState());

            return;
        }

        Vector2 newVelocity = patrolDirection * slime.stats.walkSpeed;
        if (slime.rb != null)
            slime.rb.linearVelocity = newVelocity;
        else
            slime.transform.position += (Vector3)newVelocity * Time.deltaTime;

        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            if (Random.value < slime.stats.jumpChance)
            {
                slime.ChangeState(slime.jumpState);
            }
            else
            {
                slime.ChangeState(slime.idleState);
            }
        }
    }

    public override void ExitState()
    {
        slime.animator.SetBool("isWalking", false);
        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;
    }
}
