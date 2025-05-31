using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Room Bounds")]
    public Rect roomBounds;

    private Rigidbody2D rb;

    public InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        inputActions.Player.Enable();
    }

    void FixedUpdate()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector2 delta = moveInput * moveSpeed * Time.fixedDeltaTime;
        Vector2 newPos = rb.position + delta;

        rb.MovePosition(newPos);
    }
}