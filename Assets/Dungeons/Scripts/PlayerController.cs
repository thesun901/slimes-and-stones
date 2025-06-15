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

    [Header("Cosmetics")]
    public Animator animator;
    public GameObject cosmeticBoomerang;

    [Header("Boomerang")]
    public GameObject bulletBoomerang;
    private GameObject bulletBoomerangInstance;
    public BoomerangBehavior boomBehavior;

    private bool isBoomerangReady;
    private Vector2 attackDirection;

    public GameObject eyes;
    public GameObject body;

    [Header("Stats")]
    public int hp = 100;

    private void Awake()
    {
        isBoomerangReady = true;
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inputActions.Player.Enable();
        bulletBoomerangInstance = GameObject.Instantiate(bulletBoomerang);
        boomBehavior = bulletBoomerangInstance.GetComponent<BoomerangBehavior>();
        boomBehavior.playerController = this;
        boomBehavior.player = this.transform;

        inputActions.Player.FinishDungeon.performed += context => OpenExitMenu();
    }

    private void Update()
    {
        Vector2 attackInput = inputActions.Player.AttackDirection.ReadValue<Vector2>();

        if (attackInput.sqrMagnitude > 0.01f)
        {
            StartBoomerangAnimation();
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector2 delta = moveInput * moveSpeed * Time.fixedDeltaTime;
        Vector2 newPos = rb.position + delta;

        animator.SetFloat("speed", delta.sqrMagnitude);
       
        if(delta.x > 0)
        {
            body.transform.rotation = Quaternion.Euler(0, 0, 0);
            eyes.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (delta.x < 0)
        {
            body.transform.rotation = Quaternion.Euler(0, 180, 0);
            eyes.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        rb.MovePosition(newPos);
    }


    public void StartBoomerangAnimation()
    {
        if(isBoomerangReady)
        {
            animator.SetTrigger("throw");
            isBoomerangReady = false;
            attackDirection = inputActions.Player.AttackDirection.ReadValue<Vector2>();
        }
    }

    public void ThrowBoomerang()
    {
        cosmeticBoomerang.SetActive(false);
        boomBehavior.Throw(attackDirection);
    }

    public void CatchBoomerang()
    {
        cosmeticBoomerang.SetActive(true);
        isBoomerangReady = true;
    }

    public void RecieveDamage(int amount)
    {
        hp -= amount;
    }

    private void OpenExitMenu()
    {
        DungeonMenuUI.Instance.gameObject.SetActive(true);
    }
}