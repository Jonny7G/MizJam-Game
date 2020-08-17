using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedStateHandler : MonoBehaviour
{
    public bool IsGrounded { get; private set; }
    public bool IsAirborne { get; set; }
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundedPoint;
    [SerializeField] private Vector2 boxCastSize;
    private void Update()
    {
        if (!IsAirborne)
        {
            IsGrounded = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, 0.01f, groundLayer);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundedPoint.position, boxCastSize);
    }
}
