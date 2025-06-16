using UnityEngine;

public class SlimeIdleState : SlimeState
{
    private float idleTimer;

    public SlimeIdleState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        // Ustaw parametr animatora na Idle (zakładam, że masz bool „isIdle” albo trigger „Idle”)
        slime.animator.SetBool("isIdle", true);
        slime.animator.SetBool("isWalking", false);
        slime.animator.SetBool("isAttacking", false);

        // Ustawiamy timer, po którym możemy zmienić np. w Patrol lub w ogóle czekać
        idleTimer = slime.stats.idleDuration;

        // Zatrzymujemy ruch
        if (slime.rb != null)
            slime.rb.linearVelocity = Vector2.zero;
    }

    public override void UpdateState()
    {
        // Jeżeli gracz w zasięgu detekcji → natychmiast przejdź w Chase albo Flee
        if (slime.IsPlayerInRange(slime.stats.detectionRadius))
        {
            // Jeśli HP jest poniżej progu → ucieczka, w przeciwnym przypadku pościg
            if (slime.currentHealth <= slime.stats.fleeHealthThreshold)
            {
                slime.ChangeState(slime.fleeState);
            }
            else
            {
                slime.ChangeState(slime.GetMovementState());
            }
            return;
        }

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
        {
            slime.ChangeState(slime.patrolState);
        }
    }

    public override void ExitState()
    {

    }
}
