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
    [SerializeField] private float maxAirManueverSpeed;
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
            GroundMovement();
        }
        else //jumping
        {
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
    private Vector2 jumpVel;
    private void AirAcceleration()
    {
        float beforeMag = rb.velocity.magnitude;
        jumpVel = currentJumpDirection * beforeMag;
        Vector2 addVel = moveAxis * airAccelSpeed * Time.deltaTime;
        if (manueverVel.normalized != moveAxis.normalized && moveAxis.normalized != Vector2.zero)
        {
            manueverVel = moveAxis.normalized;
        }
        else if(addVel!=Vector2.zero)
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


}
