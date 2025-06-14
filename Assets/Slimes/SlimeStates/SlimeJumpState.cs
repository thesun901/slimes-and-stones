using UnityEngine;

public class SlimeJumpState : SlimeState
{
    private float jumpTimer;
    private bool hasJumped = false;

    public SlimeJumpState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        slime.rb.gravityScale = 0;
        slime.rb.linearVelocity = Vector2.zero;
        jumpTimer = slime.stats.jumpInterval;
        hasJumped = false;

        slime.animator.SetTrigger("PrepareJump");
    }

    public override void UpdateState()
    {
        if (slime.currentHealth <= slime.stats.fleeHealthThreshold)
        {
            slime.ChangeState(slime.fleeState);
            return;
        }


        if (slime.IsPlayerInRange(slime.stats.attackRadius) && !slime.animator.GetBool("isJumping"))
        {
            slime.ChangeState(slime.GetAttackState());
            return;
        }

        if (!slime.IsPlayerInRange(slime.stats.detectionRadius))
        {
            slime.ChangeState(slime.idleState);
            return;
        }

        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f)
        {
            // najpierw ustawiamy ruch poziomy w stronę gracza
            Vector2 dir = (slime.player.position - slime.transform.position).normalized;
            Vector2 moveVel = dir * slime.stats.runSpeed;
            slime.rb.linearVelocity = moveVel;

            if (!hasJumped)
            {
                // obliczamy czas lotu na podstawie impulsu i grawitacji
                slime.rb.gravityScale = 1f;
                float v0 = slime.stats.jumpForce / slime.rb.mass;
                float g = Physics2D.gravity.y * slime.rb.gravityScale; // ujemne
                float timeUp = v0 / -g;
                float totalFlight = 2f * timeUp;

                // ustaw timer na cały lot
                jumpTimer = totalFlight;

                // dajemy impuls
                slime.rb.AddForce(Vector2.up * slime.stats.jumpForce, ForceMode2D.Impulse);

                // trigger animacji skoku
                slime.animator.SetBool("isJumping", true);

                hasJumped = true;
            }
            else
            {
                // po zakończeniu lotu
                slime.rb.gravityScale = 0f;
                hasJumped = false;
                slime.rb.linearVelocity = new Vector2(0,0);

                slime.animator.SetBool("isJumping", false);
                slime.animator.SetTrigger("PrepareJump");

                jumpTimer = slime.stats.jumpInterval;
            }
        }
    }

    public override void ExitState()
    {
        // czyścimy wszystko na wypadek przerwania stanu
        slime.animator.SetBool("isJumping", false);
        if (slime.rb != null)
        {
            slime.rb.linearVelocity = Vector2.zero;
            slime.rb.gravityScale = 0f;
        }
    }
}
