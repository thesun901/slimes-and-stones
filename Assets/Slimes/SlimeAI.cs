using UnityEngine;
using System.Collections.Generic;

public class SlimeAI : MonoBehaviour, IEnemyDamagable
{
    [Header("Referencje")]
    public Transform player;     
    public Animator animator;    
    public Rigidbody2D rb;       

    [Header("Statystyki Slima")]
    public SlimeStats stats;     

    [HideInInspector] public float currentHealth;

    // --- WSZYSTKIE MOZLIWE STANY (instancje) ---
    [HideInInspector] public SlimeIdleState idleState;
    [HideInInspector] public SlimePatrolState patrolState;
    [HideInInspector] public SlimeChaseState chaseState;

    // Stany ruchu:
    [HideInInspector] public SlimeCrawlState crawlState;
    [HideInInspector] public SlimeJumpState jumpState;
    [HideInInspector] public SlimeMicroJumpState microJumpState;

    // Stany ataku:
    [HideInInspector] public SlimeMeleeAttackState meleeAttackState;
    [HideInInspector] public SlimeJumpAttackState jumpAttackState;
    [HideInInspector] public SlimeShootAttackState shootAttackState;

    // Inne stany:
    [HideInInspector] public SlimeFleeState fleeState;

    private SlimeState currentState;


    public GameObject projectilePrefab;    // prefab pocisku
    public Transform firePoint;            // punkt, z którego wychodzi pocisk

    // (opcjonalnie) S³ownik do debugowania / iteracji
    [HideInInspector] public Dictionary<string, SlimeState> allStates;

    private void Awake()
    {
        // 1) Inicjalizacja HP
        currentHealth = stats.maxHealth;

        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        // 2) Tworzymy instancje wszystkich stanów
        idleState = new SlimeIdleState(this);
        patrolState = new SlimePatrolState(this);
        chaseState = new SlimeChaseState(this);

        crawlState = new SlimeCrawlState(this);
        jumpState = new SlimeJumpState(this);
        microJumpState = new SlimeMicroJumpState(this);

        meleeAttackState = new SlimeMeleeAttackState(this);
        jumpAttackState = new SlimeJumpAttackState(this);
        shootAttackState = new SlimeShootAttackState(this);



        fleeState = new SlimeFleeState(this);

        allStates = new Dictionary<string, SlimeState>()
        {
            { nameof(SlimeIdleState), idleState },
            { nameof(SlimePatrolState), patrolState },
            { nameof(SlimeChaseState), chaseState },
            { nameof(SlimeCrawlState), crawlState },
            { nameof(SlimeJumpState), jumpState },
            { nameof(SlimeMicroJumpState), microJumpState },
            { nameof(SlimeMeleeAttackState), meleeAttackState },
            { nameof(SlimeJumpAttackState), jumpAttackState },
            { nameof(SlimeShootAttackState), shootAttackState },
            { nameof(SlimeFleeState), fleeState }
        };


        ChangeState(idleState);


}

private void Update()
    {
        // Ka¿da klatka wywo³ujemy Update na obecnym stanie
        if (currentState != null)
            currentState.UpdateState();


    }

    /// <summary>
    /// G³ówna metoda zmiany stanu: Exit z poprzedniego, Entry w nowy.
    /// </summary>
    public void ChangeState(SlimeState newState)
    {
        if (currentState != null)
            currentState.ExitState();

        currentState = newState;

        if (currentState != null)
            currentState.EnterState();
    }


    public bool IsPlayerInRange(float radius)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= radius;
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 1f);
    }

    public SlimeState GetMovementState()
    {
        return stats.movementMode switch
        {
            SlimeStats.MovementMode.Crawl => (SlimeState)crawlState,
            SlimeStats.MovementMode.Jump => (SlimeState)jumpState,
            SlimeStats.MovementMode.MicroJump => (SlimeState)microJumpState,
            SlimeStats.MovementMode.Patrol => (SlimeState)patrolState,
            SlimeStats.MovementMode.Flee => (SlimeState)fleeState,
            _ => idleState
        };
    }


    public SlimeState GetAttackState()
    {
        return stats.attackMode switch
        {
            SlimeStats.AttackMode.Melee => (SlimeState)crawlState,
            SlimeStats.AttackMode.JumpAttack => (SlimeState)jumpAttackState,
            SlimeStats.AttackMode.Shoot => (SlimeState)shootAttackState,
            _ => meleeAttackState
        };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null )
            {
                playerController.RecieveDamage(stats.damage);

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRadius);
    }
}
