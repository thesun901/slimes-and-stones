using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StraightProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 5f;
    public int damage = 5;

    private Vector2 direction;
    private Rigidbody2D rb;

    public void Initialize(Transform playerTransform)
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = playerTransform.position;
        direction = (targetPos - startPos).normalized;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Transform playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        if (playerTransform != null )
        {
            Initialize(playerTransform);
        }
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var dmg = col.gameObject.GetComponent<PlayerController>();
        if (dmg != null)
            dmg.RecieveDamage(damage);

        Destroy(gameObject);
    }
}
