using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField]private Vector2 moveAxis;
    private Controls controls;
    private bool jumpPressed;
    private void Start()
    {
        controls = new Controls();
        controls.Player.Movement.performed += SetMoveAxis;
        controls.Player.Jump.started += (x) => jumpPressed = true;
        controls.Player.Jump.canceled += (x) => jumpPressed = false;
        controls.Enable();
    }
    private void SetMoveAxis(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }
    private void Update()
    {
        rb.velocity = moveAxis.normalized * moveSpeed;
    }
}
