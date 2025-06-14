using UnityEngine;

[System.Serializable]
public class SlimeStats
{
    // --- WYBIERZ TRYB RUCHU (z Inspektora) ---
    public enum MovementMode
    {
        Crawl,      
        Jump,       
        MicroJump,
        Flee,
        Patrol
    }
    [Header("Tryb ruchu")]
    public MovementMode movementMode = MovementMode.Crawl;

    // --- WYBIERZ TRYB ATAKU (z Inspektora) ---
    public enum AttackMode
    {
        Melee,      
        JumpAttack, 
        Shoot       
    }
    [Header("Tryb ataku")]
    public AttackMode attackMode = AttackMode.Melee;

    [Header("¯ycie i zachowanie")]
    public float maxHealth = 20f;
    public float fleeHealthThreshold = 5f;
    public float detectionRadius = 5f;
    public float attackRadius = 1.5f;
    public float jumpChance = 0.2f;
    public int damage = 10;

    [Header("Prêdkoœci ruchu")]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float chaseSpeed = 2.5f;
    public float jumpForce = 4f;
    public float jumpInterval = 1.5f;
    public float crawlSpeed = 1.5f;

    [Header("Czasy i cooldowny")]
    public float idleDuration = 1f;
    public float attackCooldown = 1f;
    public float plateauDuration = 0.5f;



    [Header("Parametry MicroJump")]
    public float microJumpForce = 2f; // si³a mikroskoku
    public float microJumpInterval = 1f; // odstêp pomiêdzy mikroskokami
}
