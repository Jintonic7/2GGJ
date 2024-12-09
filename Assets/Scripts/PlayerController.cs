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
    private InputAction dash;
    private InputAction jump;

    // movement parameters
    Vector2 moveDirection = Vector2.zero;
    public float playerRunSpeed = 15f;
    private bool isMoving;

    // jump parameters
    public float jumpInitialForce = 7f; //      initial jump force?
    public float jumpHoldForce = 5f; //         Additional force while holding space
    public float jumpMaxHoldTime = 0.5f; //     Max time for holding jump
    private float jumpHoldTime; //              Float to track length of button press
    private bool jumping; //                    bool to track if jumping

    // dash parameters
    public float dashForce;

    // animation parameters
    Animator animator;
    bool isFacingRight = false;

    // runs on game start
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
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
        // retrieves from player controls the input things, stored in crouch
        dash = playerControls.Player.Dash;
        dash.Enable();
        dash.started += Dash;


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
        dash.Disable();
    }

    // updates every frame?
    private void Update()
    {
        // check movement input handled by input handler
        moveDirection = move.ReadValue<Vector2>();

        // Check if the player is grounded using the specified radius
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Debug: Draw the GroundCheck circle in the Scene view to visualize it
        UnityEngine.Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, Color.red);

        FlipSprite();

        // check if moving, set isMoving appropriately
        if (moveDirection.x <= 0 || !jumping)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        if (jumping && jumpHoldTime < jumpMaxHoldTime) // Add additional force while holding space
        {
            jumpHoldTime += Time.deltaTime;
            body.AddForce(new Vector2(0, jumpHoldForce * Time.deltaTime), ForceMode2D.Force); // Continuously apply force
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }

    }

    // Updates more better? for physics stuff
    private void FixedUpdate()
    {
        // controlls body movement
        if (dashForce > 0)
        {
            dashForce = dashForce - 2f;
            body.linearVelocityY = 0;
        }
        body.linearVelocityX = moveDirection.x * (playerRunSpeed + dashForce);
        // animator
        animator.SetFloat("xVelocity", Mathf.Abs(body.linearVelocity.x));
        animator.SetFloat("yVelocity", (body.linearVelocity.y));
    }

    void FlipSprite()
    {
        if (isFacingRight && moveDirection.x > 0 || !isFacingRight && moveDirection.x < 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void StartJump(InputAction.CallbackContext context)
    {
        // if player is grounded, add vertical force
        if (isGrounded)
        {
            jumping = true;
            jumpHoldTime = 0f;  // Reset hold time
            body.AddForce(new Vector2(0, jumpInitialForce), ForceMode2D.Impulse);  // Initial jump force
        }
    }
    
    private void StopJump(InputAction.CallbackContext context)
    {
        jumping = false; // Stop applying additional force when the spacebar is released

        if (body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.5f); // Reduce upward velocity when space is released
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        animator.SetBool("isJumping", !isGrounded);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        dashForce = 20.0f;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("We Fired!");
    }
}
