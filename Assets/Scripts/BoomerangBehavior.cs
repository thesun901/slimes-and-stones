using Unity.VisualScripting;
using UnityEngine;

public class BoomerangBehavior : MonoBehaviour
{
    public enum State { Idle, Forward, Plateau, Back }

    public Transform player;
    public GameObject cosmeticBoomerang;
    public float forwardSpeed = 10f;
    public float backSpeed = 12f;
    public float forwardDuration = 1f;
    public float plateauDuration = 0.5f;
    public Vector2 forwardDirection;
    public float damageForward = 10f;
    public float damagePlateau = 5f;
    public float damageBack = 7f;

    private State currentState = State.Idle;
    private float stateTimer;
    private Collider2D triggerCollider;

    [HideInInspector]
    public PlayerController playerController;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.enabled = false;

        cosmeticBoomerang.SetActive(false);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                cosmeticBoomerang.SetActive(false);
                transform.position = player.position;
                if (triggerCollider != null) triggerCollider.enabled = false;
                break;

            case State.Forward:
                Move(forwardDirection.normalized, forwardSpeed);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    ChangeState(State.Plateau);
                }
                break;

            case State.Plateau:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    ChangeState(State.Back);
                }
                break;

            case State.Back:
                Vector2 dirToPlayer = (player.position - transform.position).normalized;
                Move(dirToPlayer, backSpeed);

                if (Vector2.Distance(transform.position, player.position) < 0.5f)
                {
                    ChangeState(State.Idle);
                    playerController.CatchBoomerang();
                }
                break;
        }
    }

    private void Move(Vector2 direction, float speed)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void Throw(Vector2 direction)
    {
        forwardDirection = direction;
        transform.position = player.position + (Vector3) direction;
        ChangeState(State.Forward);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                cosmeticBoomerang.SetActive(false);
                if (triggerCollider != null) triggerCollider.enabled = false;
                break;

            case State.Forward:
                cosmeticBoomerang.SetActive(true);
                if (triggerCollider != null) triggerCollider.enabled = true;
                stateTimer = forwardDuration;
                break;

            case State.Plateau:
                stateTimer = plateauDuration;
                break;

            case State.Back:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<IEnemyDamagable>();
        if (damageable != null)
        {
            float damage = currentState switch
            {
                State.Forward => damageForward,
                State.Plateau => damagePlateau,
                State.Back => damageBack,
                _ => 0f
            };
            damageable.TakeDamage(damage);
        }
    }
}
