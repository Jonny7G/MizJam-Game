using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb = default;
    [SerializeField] private GroundedStateHandler groundedState = default;
    [SerializeField] private AimingBehavior aiming;
    [SerializeField] private Transform playerSprite = default;
    [SerializeField] private LayerMask wallMask;
    [Header("ground movement")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("Grapple")]
    [SerializeField] private float grappleDistance;
    [SerializeField] private float minGrappleDist;
    [SerializeField] private float grapplePullSpeed;
    [SerializeField] private float grapplePullAccel;
    [Header("air movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private Timer timeBeforeDrag = default;
    [SerializeField, Range(0f, 1f)] private float airDrag;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float airAccelSpeed;
    [SerializeField] private float maxAirManueverSpeed;
    [SerializeField] private float stoppingSpeed = 1f;
    [SerializeField] private Vector2 moveAxis = default;

    private Controls controls;
    private bool jumpPressed;
    private Vector2 currentJumpDirection;
    private Vector2 manueverVel;
    private Vector2 jumpVel;
    private GrappleInstance currentGrapple;
    private bool grappling = false;
    private float grappleDir;

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
        if (rb.velocity.magnitude > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aiming.GetAimDir(), grappleDistance, wallMask);
            if (hit)
            {
                currentGrapple = new GrappleInstance(hit.normal, hit.point, hit.distance);

                grappleDir = Mathf.Sign(currentGrapple.GetAngle((Vector2)transform.position + rb.velocity) - currentGrapple.GetAngle(transform.position));

                grappling = true;
            }
        }
    }
    private void StopGrapple()
    {
        grappling = false;
    }
    private void Update()
    {
        if (groundedState.IsGrounded) //not jumping
        {
            GroundMovement();
        }
        if (!grappling)//jumping
        {
            Jumping();
        }
    }
    private void Grappling()
    {
        if (currentGrapple.IsOutRange(transform.position))
        {
            currentGrapple.Distance -= grapplePullSpeed * Time.deltaTime;
            transform.position = currentGrapple.GetPosition(transform.position);
            if (currentGrapple.Distance < minGrappleDist)
            {
                grappling = false;
            }
            Vector2 dirToGrapple = (currentGrapple.Position - (Vector2)transform.position).normalized;
            rb.velocity = Quaternion.Euler(0, 0, 90 * grappleDir) * dirToGrapple * rb.velocity.magnitude;
            currentJumpDirection = rb.velocity.normalized;
        }
        else
        {
            float dist = Vector2.Distance(transform.position, currentGrapple.Position);
            currentGrapple.Distance = dist;
            rb.velocity += rb.velocity.normalized * grapplePullAccel * Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (grappling)
        {
            Grappling();
        }
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
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
        currentJumpDirection = moveAxis;
        timeBeforeDrag.RestartTimer();
        rb.velocity = currentJumpDirection * jumpSpeed;
    }
    private void StopJump()
    {
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
