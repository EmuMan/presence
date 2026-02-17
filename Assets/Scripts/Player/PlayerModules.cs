using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModules : MonoBehaviour
{
    public Module headModule;
    public Module coreModule;
    public Module leftArmModule;
    public Module rightArmModule;
    public Module movementModule;

    public InputTracker headInputTracker = new InputTracker();
    public InputTracker coreInputTracker = new InputTracker();
    public InputTracker leftArmInputTracker = new InputTracker();
    public InputTracker rightArmInputTracker = new InputTracker();
    public InputTracker movementInputTracker = new InputTracker();

    public InputAction headInputAction;
    public InputAction coreInputAction;
    public InputAction leftArmInputAction;
    public InputAction rightArmInputAction;
    public InputAction movementInputAction;

    private InputAction lookInputAction;
    private Vector3 lastTargetDirection;

    void Start()
    {
        // Initialize modules with the player object
        headModule?.Initialize(gameObject);
        coreModule?.Initialize(gameObject);
        leftArmModule?.Initialize(gameObject);
        rightArmModule?.Initialize(gameObject);
        movementModule?.Initialize(gameObject);

        // Find input actions
        headInputAction = InputSystem.actions.FindAction("HeadAction");
        coreInputAction = InputSystem.actions.FindAction("CoreAction");
        leftArmInputAction = InputSystem.actions.FindAction("LeftArmAction");
        rightArmInputAction = InputSystem.actions.FindAction("RightArmAction");
        movementInputAction = InputSystem.actions.FindAction("MovementAction");

        lookInputAction = InputSystem.actions.FindAction("Look");
        lookInputAction.Enable();
    }

    void Update()
    {
        UpdateLastTargetPosition();

        // Update input trackers based on current input states
        headInputTracker.SetPressed(headInputAction?.IsPressed() ?? false);
        coreInputTracker.SetPressed(coreInputAction?.IsPressed() ?? false);
        leftArmInputTracker.SetPressed(leftArmInputAction?.IsPressed() ?? false);
        rightArmInputTracker.SetPressed(rightArmInputAction?.IsPressed() ?? false);
        movementInputTracker.SetPressed(movementInputAction?.IsPressed() ?? false);
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        // Perform module actions if their inputs are active
        headModule?.PerformActionIfAvailable(headInputTracker.IsPressed(), deltaTime, lastTargetDirection);
        coreModule?.PerformActionIfAvailable(coreInputTracker.IsPressed(), deltaTime, lastTargetDirection);
        leftArmModule?.PerformActionIfAvailable(leftArmInputTracker.IsPressed(), deltaTime, lastTargetDirection);
        rightArmModule?.PerformActionIfAvailable(rightArmInputTracker.IsPressed(), deltaTime, lastTargetDirection);
        movementModule?.PerformActionIfAvailable(movementInputTracker.IsPressed(), deltaTime, lastTargetDirection);
    }

    void OnDrawGizmos()
    {
        // This function is just for debug visualization
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + lastTargetDirection * 5);
        }
    }

    private void UpdateLastTargetPosition()
    {
        // This is the potential input from a controller
        Vector2 lookInput = lookInputAction.ReadValue<Vector2>();
        if (lookInput != Vector2.zero)
        {
            // If there is nonzero input from the controller, use that.
            lastTargetDirection = new Vector3(lookInput.x, 0, lookInput.y).normalized;
        }
        else
        {
            // Otherwise, use the mouse position to determine the target direction.
            // This assumes a top-down perspective where the player is on the XZ plane and the camera is looking down from above.
            // Create a ray from the camera through the mouse position and find where it intersects with the player's y-plane.
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                lastTargetDirection = (hitPoint - transform.position).normalized;
            }
        }
    }
}
