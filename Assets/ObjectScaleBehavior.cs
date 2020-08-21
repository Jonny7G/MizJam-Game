using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScaleBehavior : MonoBehaviour
{
    public float scaleSpeed;
    public float rotSpeed;
    public Quaternion targetRotation;
    public bool retracting;
    private float defaultScale;
    private float xTargetScale, yTargetScale;
    private void Start()
    {
        defaultScale = transform.localScale.x;
        xTargetScale = transform.localScale.x;
        yTargetScale = transform.localScale.y;
    }
    private void Update()
    {
        if (transform.localScale.x != xTargetScale || transform.localScale.y != yTargetScale)
        {
            transform.localScale = new Vector3(Mathf.MoveTowards(transform.localScale.x, xTargetScale, Time.deltaTime * scaleSpeed),
                Mathf.MoveTowards(transform.localScale.y, yTargetScale, Time.deltaTime * scaleSpeed), 1);

            if (transform.localScale.x == xTargetScale && transform.localScale.y == yTargetScale)
            {
                retracting = true;
                xTargetScale = defaultScale;
                yTargetScale = defaultScale;
            }
        }
        if (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
            if(transform.rotation == targetRotation)
            {
                targetRotation = Quaternion.Euler(0,0,0);
            }
        }
    }
    public void SetTargetScale(float targetX, float targetY)
    {
        retracting = false;
        xTargetScale = targetX;
        yTargetScale = targetY;
    }
    public bool IsAnimating()
    {
        return transform.localScale.x == xTargetScale && transform.localScale.y == yTargetScale && transform.rotation == targetRotation;
    }
    public void SetTargetRot(float target)
    {
        targetRotation = Quaternion.Euler(0,0,target);
    }
}
