using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    PlayerInput playerInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public PlayerInputActions playerControls;

    Vector2 moveDirection = Vector2.zero;
    private InputAction move;
    private InputAction fire;
    private InputAction jump;

    private bool isGrounded;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += Fire;

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += jump;
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        jump.Disable();
    }

    private void Update()
    {
        //float moveX = Input.GetAxis("Horizontal");
        //moveDirection = new Vector2(moveX, 0).normalized;

        moveDirection = move.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer); // Ground check

    }

    private void FixedUpdate()
    {
        body.linearVelocityX = moveDirection.x * moveSpeed;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Apply jump force
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("We Fired!");
    }
}
