using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Vector2 == direction jumping away from
    /// </summary>
    public System.Action<Vector2> OnJump { get; set; }
    [Header("Movement")]
    [SerializeField] private float gravity = 25f;
    [Header("ground")]
    [SerializeField] private float groundAcceleration = 45;
    [SerializeField] private float groundDeceleration = 90;
    [SerializeField] private float groundTopSpeed = 6;
    [SerializeField] private float groundFriction = 130;
    [Header("air")]
    [SerializeField] private float airAcceleration = 45;
    [SerializeField] private float airDeceleration = 90;
    [SerializeField] private float airTopSpeed = 9;
    [SerializeField] private float airFriction = 10;
    [Header("jump")]
    [SerializeField] private float jumpSpeed = 16f;
    [SerializeField] private float stopJumpSpeed = 6f;
    [Range(0f, 1f)] [SerializeField] private float airDrag = 0.94f;
    [SerializeField] private float minimumAirDragSpeed = 2f;
    [Header("Grounded")]
    public LayerMask groundLayer = default;
    [SerializeField] private float toGroundHeight = 0.7f;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.62f, 0.15f);
    [Header("Grapple")]
    [SerializeField] private float grappleDistance;
    [SerializeField] private float minGrappleDist;
    [SerializeField] private float grapplePullSpeed;
    [SerializeField] private float grapplePullAccel;
    [SerializeField] private AimingBehavior aiming;
    [SerializeField] private LayerMask wallMask;
    [Space(20)]
    public Rigidbody2D rb = default;
    public Vector2 MoveAxis { get; private set; }
    public bool Jumping { get; private set; }
    public bool Grounded { get; private set; }

    private bool jumpPressed = false;
    private bool canJump = false;
    private Controls controls;
    private GrappleInstance currentGrapple;
    private bool grappling = false;
    private float grappleDir;
    private void Start()
    {
        controls = new Controls();
        controls.Player.Movement.performed += SetMoveAxis;
        controls.Player.Jump.started += StartJump;
        controls.Player.Jump.canceled += EndJump;
        controls.Player.Action.performed += (x) => StartGrapple();
        controls.Player.Action.canceled += (x) => StopGrapple();
        controls.Enable();
    }
    private void StartGrapple()
    {
        if (rb.velocity.x > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aiming.GetAimDir(), grappleDistance, wallMask);
            if (hit)
            {
                currentGrapple = new GrappleInstance(hit.normal, hit.point, hit.distance);

                grappleDir = Mathf.Sign(currentGrapple.GetAngle((Vector2)transform.position + rb.velocity) - currentGrapple.GetAngle(transform.position));
                //grappleDir = -Mathf.Sign(rb.velocity.x);
                grappling = true;
            }
        }
    }
    private void StopGrapple()
    {
        grappling = false;
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
        }
        else
        {
            float dist = Vector2.Distance(transform.position, currentGrapple.Position);
            currentGrapple.Distance = dist;
            rb.velocity += rb.velocity.normalized * grapplePullAccel * Time.deltaTime;
        }
        if (Grounded)
        {
            grappling = false;
        }
    }
    private void SetMoveAxis(InputAction.CallbackContext context)
    {
        MoveAxis = context.ReadValue<Vector2>();
    }
    private void StartJump(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }
    private void EndJump(InputAction.CallbackContext context)
    {
        jumpPressed = false;
    }
    void Update()
    {
        Grounded = IsGrounded();
        if (Grounded)
        {
            if (MoveAxis.x != 0)
            {
                rb.velocity = new Vector2(AccelerationSpeed(MoveAxis, groundAcceleration, groundDeceleration, groundTopSpeed, groundFriction), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else if (/*!grappling*/true)
        {
            rb.velocity = new Vector2(AccelerationSpeed(MoveAxis, airAcceleration, airDeceleration, airTopSpeed, airFriction), rb.velocity.y);
            ApplyGravity();
        }
        JumpBehavior();
    }
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, groundCheckSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
        if (hit)
        {
            return hit.distance < toGroundHeight;
        }
        else //hit distance is 0 by default if nothing was hit
        {
            return false;
        }
    }
    private void ApplyGravity()
    {
        rb.velocity += Vector2.down * gravity * Time.deltaTime;
    }
    private void JumpBehavior()
    {
        if (Jumping)
        {
            UpdateJump();
        }
        if (!canJump) //variable is shared for jumps and walljumps
        {
            canJump = !jumpPressed && Grounded;
        }
        if (Grounded && jumpPressed && canJump) //check for jump first (prevents walljumping out of corners where player is grounded)
        {
            OnJump?.Invoke(Vector2.down);
            StartJump();
        }
    }
    /// <summary>
    /// The drag applied while mid air is called in fixed update to prevent framerate dependency
    /// </summary>
    private void AirDrag()
    {
        float ySpeed = rb.velocity.y;
        float xSpeed = rb.velocity.x;
        if (ySpeed > 0 && ySpeed < stopJumpSpeed && Mathf.Abs(xSpeed) > minimumAirDragSpeed) //air drag
        {
            xSpeed *= airDrag;
        }
        rb.velocity = new Vector2(xSpeed, ySpeed);
    }
    private void FixedUpdate()
    {
        if (grappling)
        {
            Grappling();
        }
    }
    private void StartJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        canJump = false;
        Jumping = true;
    }
    private void UpdateJump()
    {
        float currentSpeed = rb.velocity.y;
        if (!jumpPressed && currentSpeed > stopJumpSpeed) //variable jumping
        {
            currentSpeed = stopJumpSpeed;
        }
        rb.velocity = new Vector2(rb.velocity.x, currentSpeed);
        if (Grounded && rb.velocity.y < stopJumpSpeed)//sometimes we can still be considered grounded at the very beginning of a jump, so velocity is checked as well to prevent such edge cases.
        {
            Jumping = false;
        }
    }
    /// <summary>
    /// determine x velocity when player is grounded
    /// </summary>
    /// <param name="moveInputs"></param>
    private float AccelerationSpeed(Vector2 moveInputs, float accel, float decel, float topSpeed, float friction)
    {
        float speed = rb.velocity.x;
        if (moveInputs.x > 0) //right
        {
            if (speed < 0)
            {
                speed += decel * Time.deltaTime;
                if (speed >= 0)
                {
                    speed = decel * Time.deltaTime;
                }
            }
            else if (speed < topSpeed)
            {
                speed += accel * Time.deltaTime;
                if (speed >= topSpeed)
                {
                    speed = topSpeed;
                }
            }
        }
        else if (moveInputs.x < 0) //left
        {
            if (speed > 0)
            {
                speed -= decel * Time.deltaTime;
                if (speed <= 0)
                {
                    speed = -decel * Time.deltaTime;
                }
            }
            else if (speed > -topSpeed)
            {
                speed -= accel * Time.deltaTime;
                if (speed <= -topSpeed)
                {
                    speed = -topSpeed;
                }
            }
        }
        else if (Mathf.Abs(speed) != 0)
        {
            float sign = Mathf.Sign(speed);
            float absSpeed = Mathf.Abs(speed);
            if (absSpeed < friction * Time.deltaTime) //keeps from sliding off into infinity
            {
                speed = 0;
            }
            else
            {
                speed -= friction * sign * Time.deltaTime;
            }
        }

        return speed;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * toGroundHeight);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay((Vector2)transform.position + Vector2.right * 0.02f, Vector2.down * groundCheckDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, groundCheckSize);

        if (grappling)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentGrapple.Position);
        }
    }
}
