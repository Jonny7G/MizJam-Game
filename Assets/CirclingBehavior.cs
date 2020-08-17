using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingBehavior : MonoBehaviour
{
    public float radius;
    public Transform circlingObj;

    public float testAngle;
    void Update()
    {
        //float x = Mathf.Sin(Mathf.Deg2Rad * testAngle);
        //float y = Mathf.Cos(Mathf.Deg2Rad * testAngle);
        //x *= radius;
        //y *= radius;
        //float rotation = Vector3.SignedAngle(Vector2.up, transform.up,Vector3.forward);
        //Vector2 local = Quaternion.Euler(0, 0, rotation) * new Vector2(x, y);

        Vector2 dirToObject = (Vector2)(circlingObj.transform.position - transform.position);
        float objAngle = Vector3.SignedAngle(dirToObject, transform.up, Vector3.forward);

        Debug.DrawRay(transform.position, GetLocal(objAngle), Color.red);
    }
    private Vector2 GetLocal(float angle)
    {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = Mathf.Cos(Mathf.Deg2Rad * angle);
        x *= radius;
        y *= radius;
        float rotation = Vector3.SignedAngle(Vector2.up, transform.up, Vector3.forward);
        return Quaternion.Euler(0, 0, rotation) * new Vector2(x, y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
    }
}
