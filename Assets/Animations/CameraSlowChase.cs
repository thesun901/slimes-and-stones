using UnityEngine;

public class CameraSlowChase : MonoBehaviour
{
    public Transform target;

    [Range(0f, 1f)]
    public float smoothing = 0.125f;

    private Rigidbody2D rb;
    private Vector3 offset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (target != null)
            offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector2 smoothedPosition = Vector2.Lerp(rb.position, desiredPosition, smoothing);

        rb.MovePosition(smoothedPosition);
    }
}