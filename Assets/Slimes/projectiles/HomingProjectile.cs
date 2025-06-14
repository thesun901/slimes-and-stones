using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingProjectile : MonoBehaviour
{
    [Header("Ustawienia homingu")]
    public float speed = 4f;
    public float turnSpeed = 200f;     
    public float lifeTime = 6f;
    public int damage = 10;

    private Transform target;
    private Rigidbody2D rb;

    public void Initialize(Transform trackingTarget)
    {
        target = trackingTarget;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Initialize(GameObject.FindWithTag("Player").GetComponent<Transform>());
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = transform.up * speed;
            return;
        }

        Vector2 directionToTarget = (Vector2)target.position - rb.position;
        directionToTarget.Normalize();

        Vector2 currentUp = transform.up;

        float angleDiff = Vector2.SignedAngle(currentUp, directionToTarget);

        float maxRotate = turnSpeed * Time.fixedDeltaTime;
        float rotate = Mathf.Clamp(angleDiff, -maxRotate, maxRotate);

        rb.MoveRotation(rb.rotation + rotate);

        rb.linearVelocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var dmg = col.gameObject.GetComponent<PlayerController>();
        if (dmg != null)
            dmg.RecieveDamage(damage);

        Destroy(gameObject);
    }

}
