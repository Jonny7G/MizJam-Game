using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float grappleDistance;
    [SerializeField] private Rigidbody2D rb = default;
    [SerializeField] private GroundedStateHandler groundedState = default;
    [SerializeField] private Transform playerSprite = default;
    [SerializeField] private Timer timeBeforeDrag = default;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float airAccelSpeed;
    [SerializeField] private float maxAirManueverSpeed;
    [SerializeField, Range(0f, 1f)] private float airDrag;
    [SerializeField] private float stoppingSpeed = 1f;
    [SerializeField] private Vector2 moveAxis = default;

    private Controls controls;
    private bool jumpPressed;
    private Vector2 currentJumpDirection;
    private Vector2 manueverVel;
    private Vector2 jumpVel;
    private GrappleInstance currentGrapple;
    private bool grappling = false;
    private void Start()
    {
        controls = new Controls();
        controls.Player.Movement.performed += SetMoveAxis;
        controls.Player.Jump.started += (x) => SetJump(true);
        controls.Player.Jump.canceled += (x) => SetJump(false);
        controls.Player.Action.started += (x) => StartGrapple();
        controls.Player.Action.canceled += (x) => StopGrapple();
        controls.Enable();
    }
    private void SetMoveAxis(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }
    private void SetJump(bool val)
    {
        jumpPressed = val;
        StartJump();
    }
    private void StartGrapple()
    {
        Debug.Log("grappled");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveAxis, grappleDistance, wallMask);
        if (hit)
        {
            currentGrapple = new GrappleInstance(hit.normal, hit.point, hit.distance);
            grappling = true;
        }
    }
    private void StopGrapple()
    {
        grappling = false;
    }
    private void Update()
    {
        if (groundedState.IsGrounded && !groundedState.IsAirborne) //not jumping
        {
            GroundMovement();
        }
        if (grappling)//jumping
        {
            Grappling();
        }
        else
        {
            Jumping();
        }
    }
    private void Grappling()
    {
        if (currentGrapple.IsOutRange(transform.position))
        {
            transform.position = currentGrapple.GetPosition(transform.position);
        }
    }
    private void FixedUpdate()
    {
        if (groundedState.IsAirborne && timeBeforeDrag.TimerEnded)
        {
            rb.velocity *= airDrag;
        }
    }
    private void Jumping()
    {
        if (rb.velocity.magnitude < stoppingSpeed)
        {
            StopJump();
        }
        if (!timeBeforeDrag.TimerEnded)
        {
            timeBeforeDrag.UpdateTimer();
        }
        AirAcceleration();
    }

    private void AirAcceleration()
    {
        float beforeMag = rb.velocity.magnitude;
        jumpVel = currentJumpDirection * beforeMag;
        Vector2 addVel = moveAxis * airAccelSpeed * Time.deltaTime;
        if (manueverVel.normalized != moveAxis.normalized && moveAxis.normalized != Vector2.zero)
        {
            manueverVel = moveAxis.normalized;
        }
        else if (addVel != Vector2.zero)
        {
            manueverVel += addVel;
            if (manueverVel.magnitude > maxAirManueverSpeed)
            {
                manueverVel = manueverVel.normalized * maxAirManueverSpeed;
            }
        }
        else
        {
            manueverVel = Vector2.MoveTowards(manueverVel, Vector2.zero, Time.deltaTime * 5);
        }
        rb.velocity = (jumpVel + manueverVel).normalized * beforeMag;
    }
    private void GroundMovement()
    {
        rb.velocity = moveAxis.normalized * moveSpeed;
    }
    private void StartJump()
    {
        groundedState.IsAirborne = true;
        currentJumpDirection = moveAxis;
        timeBeforeDrag.RestartTimer();
        rb.velocity = currentJumpDirection * jumpSpeed;
    }
    private void StopJump()
    {
        groundedState.IsAirborne = false;
        currentJumpDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
    }
    private void OnDrawGizmos()
    {
        if (grappling)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentGrapple.Position);
        }
    }

}
public class GrappleInstance
{
    public Vector2 UpVector;
    public Vector2 Position;
    public float Distance;

    public GrappleInstance(Vector2 upVector, Vector2 position, float distance)
    {
        this.UpVector = upVector;
        this.Position = position;
        this.Distance = distance;
    }
    public Vector2 GetPosition(float angle)
    {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = Mathf.Cos(Mathf.Deg2Rad * angle);
        x *= Distance;
        y *= Distance;
        float rotation = Vector3.SignedAngle(Vector2.up, UpVector, Vector3.forward);
        return ((Vector2)(Quaternion.Euler(0, 0, rotation) * new Vector2(x, y)) + Position);
    }
    public Vector2 GetPosition(Vector2 pos)
    {
        return GetPosition(GetAngle(pos));
    }
    public float GetAngle(Vector2 pos)
    {
        Vector2 dirToPos = (pos - Position).normalized;
        return Vector3.SignedAngle(dirToPos, UpVector, Vector3.forward);
    }
    public bool IsOutRange(Vector2 testPos)
    {
        return Vector2.Distance(testPos, Position) > Distance;
    }
}