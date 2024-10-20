using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    public Rigidbody2D body;
    public float groundCheckRadius = 0.1f;

    PlayerInput playerInput;

    // player controls
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public PlayerInputActions playerControls;

    // direction vector variable
    Vector2 moveDirection = Vector2.zero;

    // Input action variables
    private InputAction move;
    private InputAction fire;
    private InputAction crouch;
    private InputAction jump;

    // action speeds
    public float playerSpeed = 5f;
    public float slideSpeed = 10f;
    public float slideDuration = 1f;
    public float crouchHeight = 0.5f;


    // action checks
    private bool isSliding, isCrouching, isGrounded;

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

        crouch = playerControls.Player.Crouch;
        crouch.Enable();
        crouch.performed += Crouch;

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        jump.Disable();
        crouch.Disable();
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        // Check if the player is grounded using the specified radius
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // Debug: Draw the GroundCheck circle in the Scene view to visualize it
        UnityEngine.Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, Color.red);
    }

    private void FixedUpdate()
    {
        body.linearVelocityX = moveDirection.x * playerSpeed;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("Jump Input Triggered");
        // if player is grounded, add vertical force
        if (isGrounded)
        {
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Apply jump force
            UnityEngine.Debug.Log("We Jumped!");
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("We Fired!");
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        
    }
}
