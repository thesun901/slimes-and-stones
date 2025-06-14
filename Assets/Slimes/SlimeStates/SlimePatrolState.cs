using UnityEngine;

public class SlimePatrolState : SlimeState
{
    private float patrolTimer;
    private Vector2 patrolDirection;

    public SlimePatrolState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        // Ustawiamy parametr animatora na chodzenie
        slime.animator.SetBool("isWalking", true);

        // Losujemy losowy kierunek (np. -1 lub +1 w osi X)
        float dirX = Random.value < 0.5f ? -1f : 1f;
        patrolDirection = new Vector2(dirX, 0f).normalized;

        // Ustawiamy timer patrolu � mo�emy u�y� idleDuration albo dedykowanego patrolDuration
        patrolTimer = Mathf.Lerp(1f, 3f, Random.value); // np. losowy czas 1�3s
    }

    public override void UpdateState()
    {
        // Je�li wykryjemy gracza, natychmiast przerwij patrol
        if (slime.IsPlayerInRange(slime.stats.detectionRadius))
        {
            if (slime.currentHealth <= slime.stats.fleeHealthThreshold)
                slime.ChangeState(slime.fleeState);
            else
                slime.ChangeState(slime.GetMovementState());

            return;
        }

        // Ruch w trybie �patrolu�
        Vector2 newVelocity = patrolDirection * slime.stats.walkSpeed;
        if (slime.rb != null)
            slime.rb.linearVelocity = newVelocity;
        else
            slime.transform.position += (Vector3)newVelocity * Time.deltaTime;

        // Decrement timer
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            // Co jaki� czas Slime mo�e skoczy� (je�li chcesz random jump)
            if (Random.value < slime.stats.jumpChance)
            {
                slime.ChangeState(slime.jumpState);
            }
            else
            {
                // Po sko�czeniu patrolu wracamy do Idle
                slime.ChangeState(slime.idleState);
            }
        }
    }

    public override void ExitState()
    {
        slime.animator.SetBool("isWalking", false);
        // Zatrzymaj fizyka/ruch
        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;
    }
}
