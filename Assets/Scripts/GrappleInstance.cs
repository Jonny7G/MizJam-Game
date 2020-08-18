using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleInstance
{
    public Vector2 UpVector;
    public Vector2 Position;
    public float Distance;

    public GrappleInstance(Vector2 upVector, Vector2 position, float distance)
    {
        this.UpVector = upVector;
        this.Position = position;
        this.Distance = distance;
    }
    public Vector2 GetFuturePos(Vector2 pos, float angle)
    {
        return GetPosition(GetAngle(pos) + angle);
    }
    public Vector2 GetPosition(float angle)
    {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = Mathf.Cos(Mathf.Deg2Rad * angle);
        x *= Distance;
        y *= Distance;

        float rotation = Vector3.SignedAngle(Vector2.up, UpVector, Vector3.forward);
        Vector2 orientedToUpVect = Quaternion.Euler(0, 0, rotation) * new Vector2(x, y);

        return (orientedToUpVect + Position);
    }
    public Vector2 GetSwingVelocityDirection(float direction, Vector2 pos)
    {
        Vector2 dirToGrapple = (Position - pos).normalized;
        Vector2 swingVel = Quaternion.Euler(0, 0, 90 * direction) * dirToGrapple;

        return swingVel;
    }
    public Vector2 GetPosition(Vector2 pos)
    {
        return GetPosition(GetAngle(pos));
    }
    public float GetAngle(Vector2 pos)
    {
        Vector2 dirToPos = (pos - Position).normalized;
        return Vector3.SignedAngle(dirToPos, UpVector, Vector3.forward);
    }
    public bool IsOutRange(Vector2 testPos)
    {
        return Vector2.Distance(testPos, Position) > Distance;
    }
}
