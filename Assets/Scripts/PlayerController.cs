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
    public System.Action OnGrappleShoot { get; set; }
    [Header("Movement")]
    [SerializeField] private float gravity = 25f;
    [SerializeField] private float maxGravityPull = 15f;
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
    [Header("Grounded")]
    public LayerMask groundLayer = default;
    [SerializeField] private float toGroundHeight = 0.7f;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.62f, 0.15f);
    [Header("Grapple")]
    [SerializeField] private Timer grappleTimer;
    public float maxGrappleAirSpeed;
    [SerializeField] private float maxRetractSpeed;
    [SerializeField] private float maxTugSpeed;
    [SerializeField] private float maxTugAngle;
    [SerializeField] private float maxSwingAngle;
    [SerializeField] private float grappleStartSpeed;
    [SerializeField] private float wallCheckDist = 1f;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private Vector2 wallCheckOffset;
    [SerializeField] private float grappleGravity = 25f;
    [SerializeField] private float grappleDistance;
    [SerializeField] private float grapplePullAccel;
    [SerializeField] private float grappleDistChangeSpeed;
    [SerializeField] private float minGrappleDist = 1;
    public float maxGrappleSpeed;
    public float grappleSpeed;
    [Range(0f, 1f)] public float grappleAirDrag;
    [Header("Grapple Pull")]
    [SerializeField] private float targetedyBoostVel;
    [SerializeField] private float targetedxBoostVel;
    [SerializeField] private float grapplePullSpeed;
    [SerializeField] private float targetingRadius;
    [SerializeField] private LayerMask targetableMask;
    [Space(20)]
    [SerializeField] private AimingBehavior aiming;
    [SerializeField] private GrappleHookDrawer grappleDrawer;
    [SerializeField] private LayerMask wallMask;
    [Space(20)]
    public Rigidbody2D rb = default;
    public Vector2 velocity;
    public Vector2 MoveAxis { get; private set; }
    public bool Jumping { get; private set; }
    public bool Grounded { get; private set; }

    private bool jumpPressed = false;
    private bool canJump = false;
    [SerializeField] private Controls controls;
    private GrappleInstance currentGrapple;
    private Targetable targetedObject;
    public bool grappling { get; private set; }
    private float attackVelocity;
    private bool pulling = false;
    private float activeTopSpeed;
    private void Start()
    {
        activeTopSpeed = airTopSpeed;
        controls = new Controls();
        controls.Player.Movement.performed += SetMoveAxis;
        controls.Player.Jump.started += StartJump;
        controls.Player.Jump.canceled += EndJump;
        controls.Player.Action.started += CheckForGrapple;
        controls.Player.Action.canceled += CheckForStopGrapple;
        controls.Enable();
    }
    private void OnDestroy()
    {
        if (controls != null)
        {
            controls.Disable();
            controls.Player.Movement.performed -= SetMoveAxis;
            controls.Player.Jump.started -= StartJump;
            controls.Player.Jump.canceled -= EndJump;
            controls.Player.Action.started -= CheckForGrapple;
            controls.Player.Action.canceled -= CheckForStopGrapple;
        }
    }
    private void CheckForGrapple(InputAction.CallbackContext context)
    {
        if (!Grounded && !grappling && grappleTimer.TimerEnded)
        {
            RaycastHit2D targetableHit = Physics2D.CircleCast(transform.position, targetingRadius, aiming.GetAimDir(), grappleDistance, targetableMask);
            if (targetableHit)
            {
                targetedObject = targetableHit.collider.GetComponent<Targetable>();
                StartGrapple(targetedObject.targetPosition.position, Vector2.Distance(transform.position, targetedObject.targetPosition.position));
                targetedObject.SetGrappledState(true);
                attackVelocity = Mathf.Sign((targetedObject.targetPosition.position.x - transform.position.x)) * Mathf.Clamp(Mathf.Abs(rb.velocity.x), 1, maxGrappleAirSpeed);
                pulling = true;
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, aiming.GetAimDir(), grappleDistance, wallMask);
                if (hit)
                {
                    StartGrapple(hit.point, hit.distance);
                    if (hit.point.y < transform.position.y)
                    {
                        pulling = true;
                    }
                    else
                    {
                        pulling = false;
                    }
                }
            }

        }
    }
    private void CheckForStopGrapple(InputAction.CallbackContext context)
    {
        StopGrapple();
    }
    private void StartGrapple(Vector2 grapplePoint, float grappleDist)
    {
        currentGrapple = new GrappleInstance(Vector2.down, grapplePoint, grappleDist);

        grappleSpeed = -rb.velocity.x;
        if (grappleSpeed < grappleStartSpeed)
        {

            Vector2 dirToGrapple = (grapplePoint - (Vector2)transform.position).normalized;

            if (Mathf.Abs(rb.velocity.y) * dirToGrapple.x > grappleStartSpeed)
            {
                grappleSpeed = Mathf.Abs(rb.velocity.y) * -dirToGrapple.x;
            }
            else
            {
                grappleSpeed = Mathf.Sign(grappleSpeed) * (Mathf.Abs(grappleSpeed) + grappleStartSpeed);
            }
        }
        grappleDrawer.SetLines(currentGrapple.Position);
        grappling = true;
        activeTopSpeed = maxGrappleAirSpeed;
        OnGrappleShoot?.Invoke();
        grappleTimer.RestartTimer();
    }
    private void GrapplePull()
    {
        if (currentGrapple.Distance > minGrappleDist)
        {
            rb.velocity = (currentGrapple.Position - (Vector2)transform.position).normalized * grapplePullSpeed;
            currentGrapple.Distance = Vector2.Distance(transform.position, currentGrapple.Position);
        }
        else
        {
            StopGrapple();
            pulling = false;
        }
    }
    private void StopGrapple()
    {
        if (grappling)
        {
            grappling = false;
            if (!pulling)
            {
                rb.velocity = GetEndSwingGrappleVelocity();
            }
            else
            {
                if (targetedObject == null || currentGrapple.Distance > minGrappleDist + 0.2f)
                {
                    rb.velocity = GetEndSwingGrappleVelocity();
                }
                else
                {
                    rb.velocity = GetEndTargetedGrappleVelocity();
                }
                if (targetedObject != null)
                {
                    targetedObject.GetComponent<Damageable>()?.Damage(1);
                    targetedObject.SetGrappledState(false);
                    targetedObject = null;
                }
            }
            grappleDrawer.ClearLines();
        }
    }
    private Vector2 GetEndTargetedGrappleVelocity()
    {
        float xVel = Mathf.Sign(attackVelocity) * (Mathf.Abs(attackVelocity) + targetedxBoostVel);
        xVel = Mathf.Clamp(xVel, -maxGrappleAirSpeed, maxGrappleAirSpeed);
        float yVel = targetedyBoostVel;
        Bouncable bouncObj = targetedObject.GetComponent<Bouncable>();
        if (bouncObj != null)
        {
            yVel = bouncObj.bounceVel;
        }
        return new Vector2(xVel, targetedyBoostVel);
    }
    private Vector2 GetEndSwingGrappleVelocity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 1);
        }
        float vel = -grappleSpeed;
        vel = Mathf.Clamp(vel, -maxGrappleAirSpeed, maxGrappleAirSpeed);
        return new Vector2(vel, rb.velocity.y);
    }
    private void SetGrappleVelocity(float direction, float speed)
    {
        Vector2 swingVel = currentGrapple.GetSwingVelocityDirection(direction, transform.position);
        rb.velocity = swingVel.normalized * speed;
    }
    private bool isAgainstWall;
    private Vector2 wallPoint;
    private void UpdateWallState()
    {
        RaycastHit2D isWall = Physics2D.BoxCast((Vector2)transform.position + wallCheckOffset, wallCheckSize, 0f, currentGrapple.GetSwingVelocityDirection(grappleSpeed, transform.position), wallCheckDist, wallMask);
        if (isWall)
        {
            wallPoint = isWall.point;
        }
        isAgainstWall = isWall;
    }
    private void Grappling()
    {
        if (pulling)
        {
            GrapplePull();
        }
        else
        {
            UpdateWallState();
            HandleSwingVelocity();
            if (currentGrapple.IsOutRange(transform.position))
            {
                Vector2 newPos = currentGrapple.GetPosition(transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (newPos - (Vector2)transform.position).normalized, Vector2.Distance(transform.position, newPos), wallMask);
                if (hit)
                {
                    newPos = hit.point + hit.normal * wallCheckDist;
                }
                transform.position = newPos;

            }
            else if (currentGrapple.Distance > Vector2.Distance(transform.position, currentGrapple.Position))
            {
                float dist = Vector2.Distance(transform.position, currentGrapple.Position);
                currentGrapple.Distance = dist;
                rb.velocity += gravity * Vector2.down * Time.deltaTime;
            }

        }
        if (Grounded)
        {
            StopGrapple();
        }
    }
    private float GetGrappleGravDir()
    {
        float playerAngle = currentGrapple.GetAngle(transform.position);
        float gravAngle = currentGrapple.GetAngle(currentGrapple.Position + currentGrapple.Distance * Vector2.down);
        float diff = (gravAngle - playerAngle);
        return Mathf.Sign(diff);
    }
    private float angleDiff;
    private void HandleSwingVelocity()
    {
        if (currentGrapple.IsOutRange(transform.position))
        {
            float playerAngle = currentGrapple.GetAngle(transform.position);
            float gravAngle = currentGrapple.GetAngle(currentGrapple.Position + currentGrapple.Distance * Vector2.down);
            float diff = (gravAngle - playerAngle);
            float gravDir = Mathf.Sign(diff);
            angleDiff = Mathf.Abs(diff);
            grappleSpeed += gravDir * grappleGravity * Time.deltaTime;

            if (Mathf.Sign(MoveAxis.x) != 0) //player caused vel
            {
                if (Mathf.Abs(grappleSpeed) < maxTugSpeed && Mathf.Abs(diff) < maxTugAngle) //independent tug vel
                {
                    grappleSpeed -= MoveAxis.x * Time.deltaTime * grapplePullAccel;
                }
                else if (-Mathf.Sign(MoveAxis.x) == gravDir && Mathf.Abs(diff) < maxSwingAngle) //push with gravity
                {
                    grappleSpeed += gravDir * Time.deltaTime * grapplePullAccel;
                }
            }
            grappleSpeed *= grappleAirDrag;
            grappleSpeed = Mathf.Clamp(grappleSpeed, -maxGrappleSpeed, maxGrappleSpeed);

            SetGrappleVelocity(Mathf.Sign(grappleSpeed), Mathf.Abs(grappleSpeed));
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
            grappleSpeed = 0;
            if (MoveAxis.x != 0)
            {
                rb.velocity = new Vector2(AccelerationSpeed(MoveAxis, groundAcceleration, groundDeceleration, groundTopSpeed, groundTopSpeed, groundFriction), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            ApplyGravity();
        }
        if (grappling)
        {
            grappleDrawer.UpdateLines(currentGrapple.Position);
            if (MoveAxis.y != 0 && !pulling)
            {
                if ((MoveAxis.y < 0 || currentGrapple.Distance > minGrappleDist) && MoveAxis.x == 0 && angleDiff < 90)
                {
                    currentGrapple.Distance -= MoveAxis.y * grappleDistChangeSpeed * Time.deltaTime;
                    grappleSpeed = Mathf.MoveTowards(grappleSpeed, GetGrappleGravDir()*grappleGravity, Time.deltaTime * grappleGravity);
                    SetGrappleVelocity(Mathf.Sign(grappleSpeed), Mathf.Abs(grappleSpeed));
                    if (angleDiff > 45)
                    {
                        
                        
                        //grappleSpeed += GetGrappleGravDir() * grappleDistChangeSpeed * Time.deltaTime * currentGrapple.Distance;
                        //grappleSpeed = Mathf.Clamp(grappleSpeed, -maxRetractSpeed, maxRetractSpeed);
                    }
                    transform.position = currentGrapple.GetPosition(transform.position);
                }
            }
        }
        if (!Grounded && !grappling)
        {
            rb.velocity = new Vector2(AccelerationSpeed(MoveAxis, airAcceleration, airDeceleration, activeTopSpeed, airTopSpeed, airFriction), rb.velocity.y);
        }
        JumpBehavior();
        velocity = rb.velocity;
        if (!grappleTimer.TimerEnded)
        {
            grappleTimer.UpdateTimer();
        }
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
        if (rb.velocity.y > -maxGravityPull && !grappling)
        {
            rb.velocity += Vector2.down * gravity * Time.deltaTime;
        }
    }
    private void JumpBehavior()
    {
        if (Jumping)
        {
            if (!grappling)
            {
                UpdateJump();
            }
            else
            {
                Jumping = false;
            }
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
    private void FixedUpdate()
    {
        if (grappling)
        {
            Grappling();
        }
    }
    private void StartJump()
    {
        if (!grappling)
        {
            activeTopSpeed = airTopSpeed;
        }
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
    private float AccelerationSpeed(Vector2 moveInputs, float accel, float decel, float topSpeed, float topAccelSpeed, float friction)
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
            else if (speed < topAccelSpeed)
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
            else if (speed > -topAccelSpeed)
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
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, Vector2.down * toGroundHeight);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay((Vector2)transform.position + Vector2.right * 0.02f, Vector2.down * groundCheckDistance);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(transform.position, groundCheckSize);
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireCube((Vector2)transform.position + wallCheckOffset, wallCheckSize);
        //Gizmos.DrawLine(transform.position + (Vector3)wallCheckOffset, transform.position + (Vector3)wallCheckOffset + Vector3.right * wallCheckDist);
        //if (grappling)
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawLine(currentGrapple.Position, currentGrapple.Position + -currentGrapple.UpVector * 3);
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(transform.position, currentGrapple.Position);
        //}
    }
}
