using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    public Rigidbody2D body;
    PlayerInput playerInput;

    // player controls
    public PlayerInputActions playerControls;

    // direction vector variable
    Vector2 moveDirection = Vector2.zero;

    // Input action variables
    private InputAction move;
    private InputAction fire;
    private InputAction crouch;

    // action speeds
    public float moveSpeed = 5f;
    public float slideSpeed = 10f;
    public float slideDuration = 1f;
    public float crouchHeight = 0.5f;

    // action checks
    private bool isSliding;
    private bool isCrouching;

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

    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        body.linearVelocityX = moveDirection.x * moveSpeed;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("We Fired!");
    }

    private void Crouch(InputAction);
}
