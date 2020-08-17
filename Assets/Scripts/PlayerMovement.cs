using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb = default;
    [SerializeField] private GroundedStateHandler groundedState = default;
    [SerializeField] private Transform playerSprite = default;
    [SerializeField] private Timer timeBeforeDrag = default;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float airAccelSpeed;
    [SerializeField] private float airManueverSpeed;
    [SerializeField, Range(0f, 1f)] private float airDrag;
    [SerializeField] private float stoppingSpeed = 1f;
    [SerializeField] private Vector2 moveAxis = default;

    private Controls controls;
    private bool jumpPressed;
    private Vector2 currentJumpDirection;

    private void Start()
    {
        controls = new Controls();
        controls.Player.Movement.performed += SetMoveAxis;
        controls.Player.Jump.started += (x) => SetJump(true);
        controls.Player.Jump.canceled += (x) => SetJump(false);
        controls.Enable();
    }
    private void SetMoveAxis(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }
    private void SetJump(bool val)
    {
        jumpPressed = val;
        //if (groundedState.IsGrounded && !groundedState.IsAirborne)
        //{
            StartJump();
        //}
    }
    private void Update()
    {
        if (groundedState.IsGrounded && !groundedState.IsAirborne) //not jumping
        {
            Debug.Log("walking");
            GroundMovement();
        }
        else //jumping
        {
            Debug.Log("jumping");
            Jumping();
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
    private Vector2 manueverVel;
    private void AirAcceleration()
    {
        float beforeMag = rb.velocity.magnitude;
        manueverVel = moveAxis * airAccelSpeed;
        Vector2 addVel = moveAxis * airAccelSpeed * Time.deltaTime;
        rb.velocity = (rb.velocity + addVel).normalized * beforeMag;
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


}
