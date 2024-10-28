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
    public float playerRunSpeed = 15f;
    private bool isMoving;

    // jump parameters
    public float jumpInitialForce = 7f; //      initial jump force?
    public float jumpHoldForce = 5f; //         Additional force while holding space
    public float jumpMaxHoldTime = 0.5f; //     Max time for holding jump
    private float jumpHoldTime; //              Float to track length of button press
    private bool jumping; //                    bool to track if jumping

    // crouch parameters
    public float crouchHeight = 0.5f;//         Height of character when crouching?
    private bool isCrouching; //                Bool to track crouching

    // slide parameters
    public float slideInitialForce = 4f; //   initial slide force?
    //public float slideHoldForce = 0.3f; //        additional force while holding crouch
    public float slideMaxHoldTime = 1f; //          max time for holding jump
    public float slideFriction = 5f;

    private float slideDirection;
    private float slideTimer; //             float to track length of putton press
    private bool sliding; //                    bool to track if sliding
    private float currentSlideSpeed;
    private float slideMovement;

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
        // retrieves from player controls the input things, stored in crouch
        crouch = playerControls.Player.Crouch;
        crouch.Enable();
        crouch.performed += Crouch;
        crouch.started += StartSlide;
        crouch.canceled += StopSlide;


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
        // check movement input handled by input handler
        moveDirection = move.ReadValue<Vector2>();

        // Check if the player is grounded using the specified radius
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Debug: Draw the GroundCheck circle in the Scene view to visualize it
        UnityEngine.Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, Color.red);

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
        }
        /*
        if (sliding && slideHoldTime < slideMaxHoldTime)
        {
            slideHoldTime += Time.deltaTime;
            // add force as long as within time restraint
            body.AddForce(new Vector2(slideHoldForce * Time.deltaTime, 0), ForceMode2D.Force);
        }
        */
        

    }

    // Updates more better? for physics stuff
    private void FixedUpdate()
    {
        // controlls body movement
        if (sliding && slideTimer >= 0 && isGrounded)
        {
            // decrease timer
            slideTimer -= Time.deltaTime;
            // apply slide movement
            currentSlideSpeed = slideInitialForce * (slideTimer / slideMaxHoldTime);
            //slideMovement = currentSlideSpeed * slideDirection;

            // move the player
            body.linearVelocityX = moveDirection.x * currentSlideSpeed;
        }
        if (!sliding)
        {
            body.linearVelocityX = moveDirection.x * playerRunSpeed;
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

    private void StartSlide(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("Slide Input Triggered");
        // if player is grounded, add vertical force
        if (isGrounded)
        {
            // set the slide direction to the charaters forward direction
            slideDirection = moveDirection.x;

            slideTimer = slideMaxHoldTime;  // Reset hold time
            sliding = true;
            // body.AddForce(new Vector2(slideInitialForce, 0), ForceMode2D.Impulse);  // Initial jump force
            UnityEngine.Debug.Log("Slide started!");
        }
    }

    private void StopSlide(InputAction.CallbackContext context)
    {
        sliding = false; // Stop applying additional force when the spacebar is released
        UnityEngine.Debug.Log("Slide stopped!");

        /*
        if (body.linearVelocity.x > playerRunSpeed)
        {
            // Reduce horizontal velocity when space is released
            body.linearVelocity = new Vector2(body.linearVelocity.x * 0.5f, body.linearVelocity.y);
        }
        */
    }

    private void Fire(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("We Fired!");
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (isGrounded && !isMoving) {
            UnityEngine.Debug.Log("crouched");
            // code for crouch animation, slowed speed, lowered hitbox
        }
    }
}
