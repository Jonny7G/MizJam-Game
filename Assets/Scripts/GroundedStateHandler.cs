using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedStateHandler : MonoBehaviour
{
    public bool IsGrounded { get; private set; }
    public float groundCheckDistance;
    public float groundedDistance;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundedPoint;
    [SerializeField] private Vector2 boxCastSize;
    private void Update()
    {
        IsGrounded = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundedPoint.position, boxCastSize);
        Gizmos.DrawLine(groundedPoint.position, (Vector2)groundedPoint.position + Vector2.down * groundedDistance);
    }
}
