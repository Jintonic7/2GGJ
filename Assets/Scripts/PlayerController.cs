using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // player declarations
    public Rigidbody2D body;
    public PlayerInputActions playerControls;
    PlayerInput playerInput;

    // ground
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;
    private bool isGrounded;

    // Input action variables
    private InputAction move;
    private InputAction fire;
    private InputAction crouch;
    private InputAction jump;

    // movement parameters
    Vector2 moveDirection = Vector2.zero;
    public float moveSpeed = 5f;
    public float playerSpeed = 5f;
    private bool isMoving;

    // jump parameters
    public float jumpForce = 7f;
    public float holdJumpForce = 5f; // Additional force while holding space
    public float maxHoldTime = 0.5f; // Max time for holding jump
    private bool jumping;
    private float holdTime;

    // crouch parameters
    public float crouchHeight = 0.5f;
    private bool isCrouching;

    // slide parameters
    public float slideSpeed = 10f;
    public float slideDuration = 1f;
    public float slideForce = 8f;
    private bool isSliding;

    // runs on game start
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    // runs on game awake?
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerControls = new PlayerInputActions();
    }

    // Unity enable thing
    private void OnEnable()
    {
        // enables player movement
        move = playerControls.Player.Move;
        move.Enable();

        // enables attack
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += Fire;

        // enables crouch
        crouch = playerControls.Player.Crouch;
        crouch.Enable();
        crouch.performed += Crouch;

        // enables jump
        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.started += StartJump;  // Called when spacebar is first pressed
        jump.canceled += StopJump;  // Called when spacebar is released
    }

    // allows unity to disable
    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        jump.Disable();
        crouch.Disable();
    }

    // updates every frame?
    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        // Check if the player is grounded using the specified radius
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // Debug: Draw the GroundCheck circle in the Scene view to visualize it
        UnityEngine.Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, Color.red);
        if (moveDirection.x <= 0 || !jumping)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
        if (jumping && holdTime < maxHoldTime) // Add additional force while holding space
        {
                holdTime += Time.deltaTime;
                body.AddForce(new Vector2(0, holdJumpForce * Time.deltaTime), ForceMode2D.Force); // Continuously apply force
        }
    }

    // Updates more better? for physics stuff
    private void FixedUpdate()
    {
        // controlls body movement
        body.linearVelocityX = moveDirection.x * playerSpeed;
    }

    private void StartJump(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("Jump Input Triggered");
        // if player is grounded, add vertical force
        if (isGrounded)
        {
            jumping = true;
            holdTime = 0f;  // Reset hold time
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);  // Initial jump force
            UnityEngine.Debug.Log("Jump started!");
        }
    }
    
    private void StopJump(InputAction.CallbackContext context)
    {
        jumping = false; // Stop applying additional force when the spacebar is released
        UnityEngine.Debug.Log("Jump stopped!");

        if (body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.5f); // Reduce upward velocity when space is released
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("We Fired!");
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("crouched");
        if (isGrounded)
        {
            if (isMoving)
            {
                body.AddForce(new Vector2(slideForce, 0), ForceMode2D.Impulse);
                //AddForce(new Vector2(slideForce, 0), ForceMode2D.Impluse);
            }
            else
            {

            }
        }
    }
}
