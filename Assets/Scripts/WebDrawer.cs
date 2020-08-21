using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebDrawer : MonoBehaviour
{
    public GrappleHookDrawer drawer;
    public Vector2 direction;
    public LayerMask toDrawToMask;
    private Vector2 endPos;
    private void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1000, toDrawToMask);
        if (hit)
        {
            endPos = hit.point;
        }
        else
        {
            endPos = transform.position + (Vector3)direction * 1000;
        }
        drawer.SetLines(endPos);
    }
    private void Update()
    {
        drawer.UpdateLines(endPos);
    }
}
