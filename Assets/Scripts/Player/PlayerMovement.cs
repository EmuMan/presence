using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum JumpState
    {
        Grounded,
        Jumping,
        Rising,
        Falling
    }

    public float speed = 5.0f;
    public float jumpStrength = 5.0f;
    public float extraGroundCheckDistance = 0.1f;
    public float fallMultiplier = 2.0f;

    public JumpState jumpState = JumpState.Grounded;

    private CharacterController controller;

    private Vector3 velocity;

    private InputAction moveAction;
    private Vector3 movementInput;

    private InputAction jumpAction;
    private bool jumpInput = false;
    private BufferedAction jumpBufferedAction = new BufferedAction(0.2f, 0.3f);

    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        GetInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyGravity();
        ApplyMovementInput();
        MoveCharacter();
        JumpCharacter();
    }

    void GetInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        movementInput = new Vector3(input.x, 0.0f, input.y);

        jumpInput = jumpAction.IsPressed();
    }

    void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            float fallDeltaSpeed = Physics.gravity.y * Time.fixedDeltaTime;
            if (jumpState == JumpState.Falling || jumpState == JumpState.Rising)
            {
                fallDeltaSpeed *= fallMultiplier;
            }
            velocity.y += fallDeltaSpeed;
        }
        else
        {
            velocity.y = 0.0f;
        }
    }

    void ApplyMovementInput()
    {
        Vector3 rotatedMove = transform.TransformDirection(movementInput);
        Vector3 horizontalVelocity = rotatedMove * speed;
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    void MoveCharacter()
    {
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    void JumpCharacter()
    {
        if (jumpBufferedAction.IsActing(jumpInput, controller.isGrounded))
        {
            velocity.y = jumpStrength;
            jumpState = JumpState.Jumping;
        }
        else
        {
            if (IsGrounded())
            {
                jumpState = JumpState.Grounded;
            }
            else if (velocity.y > 0)
            {
                jumpState = JumpState.Rising;
            }
            else
            {
                jumpState = JumpState.Falling;
            }
        }
    }

    bool IsGrounded()
    {
        // Get the bottom center of the character capsule
        float radius = controller.radius;
        float height = controller.height;
        Vector3 center = controller.transform.position + controller.center;
        Vector3 bottom = center - Vector3.up * (height / 2f);

        // Cast a sphere slightly below the character
        float castDistance = controller.skinWidth + extraGroundCheckDistance;

        return Physics.SphereCast(
            bottom,
            radius * 0.9f,  // Slightly smaller radius to avoid edge cases
            Vector3.down,
            out RaycastHit hit,
            castDistance,
            ~0,  // Or specify your ground layers
            QueryTriggerInteraction.Ignore
        );
    }
}
