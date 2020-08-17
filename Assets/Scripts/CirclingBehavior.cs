using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingBehavior : MonoBehaviour
{
    public float radius;
    public Transform circlingObj;

    public float testAngle;
    public Vector2 pos;
    private GrappleInstance grappleInstance;

    void Update()
    {
        grappleInstance = new GrappleInstance(transform.up, transform.position, radius);
        Vector2 circlePos = grappleInstance.GetPosition(circlingObj.transform.position);
        Debug.DrawLine(grappleInstance.Position, circlePos, Color.red);
        Debug.DrawLine(grappleInstance.Position, grappleInstance.GetFuturePos(circlePos, testAngle),Color.blue);

        //Debug.DrawRay(transform.position, crossDir*12, Color.cyan);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, 1f);
    }
}
