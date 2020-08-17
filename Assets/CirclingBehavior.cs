using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingBehavior : MonoBehaviour
{
    public float radius;
    public Transform circlingObj;

    public float testAngle;
    public Vector2 pos;
    void Update()
    {
        Vector2 dirToObject = (Vector2)(circlingObj.transform.position - transform.position);
        float objAngle = Vector3.SignedAngle(dirToObject, transform.up, Vector3.forward);
        pos = GetPos(objAngle) + (Vector2)transform.position;
    }
    private Vector2 GetPos(float angle)
    {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = Mathf.Cos(Mathf.Deg2Rad * angle);
        x *= radius;
        y *= radius;
        float rotation = Vector3.SignedAngle(Vector2.up, transform.up, Vector3.forward);
        return Quaternion.Euler(0, 0, rotation) * new Vector2(x, y);
    }
    private Vector2 GetFuturePos(Vector2 pos,float angle)
    {
        return Vector2.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, 1f);
    }
}
