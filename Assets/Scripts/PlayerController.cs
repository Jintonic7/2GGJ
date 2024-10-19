using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    PlayerInput playerInput;
    public float moveSpeed = 5f;

    public PlayerInputActions playerControls;

    Vector2 moveDirection = Vector2.zero;
    private InputAction move;
    private InputAction fire;

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
    }

    private void OnDisable()
    {
        playerControls.Disable();
        move.Disable();
    }

    private void Update()
    {
        //float moveX = Input.GetAxis("Horizontal");
        //moveDirection = new Vector2(moveX, 0).normalized;

        moveDirection = move.ReadValue<Vector2>();

    }

    private void FixedUpdate()
    {
        body.linearVelocityX = moveDirection.x * moveSpeed;
    }
}
