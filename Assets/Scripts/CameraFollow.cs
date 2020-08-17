using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private bool followX, followY;
    [SerializeField] private Vector2 min, max;
    private void FixedUpdate()
    {
        float xPos = transform.position.x;
        if (followX)
        {
            xPos = Mathf.Lerp(xPos, target.position.x, Time.deltaTime * speed);
        }
        float yPos = transform.position.y;
        if (followY)
        {
            yPos = Mathf.Lerp(yPos, target.position.y, Time.deltaTime * speed);
        }
        xPos = Mathf.Clamp(xPos, min.x, max.x);
        yPos = Mathf.Clamp(yPos, min.y, max.y);
        transform.position = new Vector3(xPos, yPos, transform.position.z);
    }
}
