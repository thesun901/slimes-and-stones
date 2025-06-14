using System;
using UnityEngine;

public class SlimeJumpAttackState : SlimeState
{
    private bool hasJumped = false;
    private float attackTimer;
    private float microTimer = 0;

    public SlimeJumpAttackState(SlimeAI slime) : base(slime) { }

    public override void EnterState()
    {
        hasJumped = false;
        slime.rb.gravityScale = 0;
        slime.rb.linearVelocity = Vector2.zero;
    }

    public override void UpdateState()
    {

        if (!slime.IsPlayerInRange(slime.stats.attackRadius))
        {
            slime.ChangeState(slime.GetMovementState());
            return;
        }


        microTimer -= Time.deltaTime;
        if (microTimer <= 0f)
        {
            Vector2 directionToPlayer = (slime.player.position - slime.transform.position).normalized;
            float chaseSpeed = slime.stats.chaseSpeed;
            Vector2 newVelocity = new Vector2(directionToPlayer.x * chaseSpeed, directionToPlayer.y * chaseSpeed);
            slime.rb.linearVelocity = newVelocity;
            if (!hasJumped)
            {
                // Wykonujemy jednorazowy impuls w górę
                if (slime.rb != null)
                {
                    // ustawiamy grawitację i impuls
                    slime.rb.gravityScale = 1f;
                    // policz dokładnie totalFlightTime:
                    float v0 = slime.stats.microJumpForce / slime.rb.mass;
                    float g = Physics2D.gravity.y * slime.rb.gravityScale; // ujemne
                    float timeUp = v0 / -g;
                    float totalFlightTime = 2f * timeUp;

                    // używamy tego jako mikro-timer zamiast stałego interwału:
                    microTimer = totalFlightTime;

                    slime.rb.AddForce(Vector2.up * slime.stats.microJumpForce, ForceMode2D.Impulse);
                    slime.animator.SetBool("isMicroJumping", true);
                }
                hasJumped = true;
            }
            else
            {
                slime.rb.gravityScale = 0;
                microTimer = slime.stats.microJumpInterval;
                hasJumped = false;
                slime.animator.SetBool("isMicroJumping", false);

            }
        }
    }

    public override void ExitState()
    {
        slime.animator.SetBool("isMicroJumping", false);
        if (slime.rb != null)
        {
            slime.rb.linearVelocity = Vector2.zero;
            slime.rb.gravityScale = 0;
        }
    }

    /// <summary>
    /// Wywo�ywane z Eventu w animacji �JumpAttack� (np. w momencie oddawania skoku)
    /// </summary>
    public void PerformJumpAttack()
    {
        if (slime.rb != null)
        {
            // Wyrzucamy w powietrze
            slime.rb.linearVelocity = new Vector2(0f, 0f);
            slime.rb.AddForce(Vector2.up * slime.stats.jumpForce, ForceMode2D.Impulse);
        }
        hasJumped = true;

        // Gdy jeste�my w powietrzu, mo�emy sprawdzi� kolizj� z graczem
        // Mo�esz np. przeci�gn�� Collider2D na shinny layer i wykry� w OnCollisionEnter2D
    }
}
