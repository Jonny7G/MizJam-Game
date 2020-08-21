using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float playerFollowSpeed;
    [SerializeField] private float cameraPanSpeed;
    [SerializeField] private float offsetSpeed;
    [SerializeField] private bool followX, followY;
    [SerializeField, Range(0f, 1f)] private float reachedApprox;
    [SerializeField] private float offsetAmount;
    public Vector2 min, max;

    private PlayerController player;
    private bool reachedMinimum;
    private Vector2 currOffset;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    private void FixedUpdate()
    {
        if (reachedMinimum)
        {
            currOffset = Vector2.Lerp(currOffset, new Vector2(player.rb.velocity.normalized.x * offsetAmount,0), Time.deltaTime * offsetSpeed);
            Vector2 newPos = GoTowardPosition(playerFollowSpeed, (Vector2)player.transform.position+currOffset);
            newPos = new Vector2(Mathf.Clamp(newPos.x, min.x, max.x), Mathf.Clamp(newPos.y, min.y, max.y));
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
        else
        {
            Vector2 newPos = GoTowardPosition(cameraPanSpeed, new Vector2(min.x, player.transform.position.y));
            newPos = new Vector2(newPos.x, Mathf.Clamp(newPos.y, min.y, max.y));
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            if (Mathf.Abs(transform.position.x-min.x)< reachedApprox)
            {
                reachedMinimum = true;
            }
        }
    }
    private Vector2 GoTowardPosition(float speed, Vector2 target)
    {
        float xPos = transform.position.x;
        float vel = 0;
        if (followX)
        {
            xPos = Mathf.Lerp(xPos, target.x,speed*Time.deltaTime);
        }
        float yPos = transform.position.y;
        if (followY)
        {
            yPos = Mathf.Lerp(yPos, target.y,speed*Time.deltaTime);
        }

        return new Vector2(xPos, yPos);
    }
    public void SetNewMinimum(float newMin)
    {
        min.x = newMin;
        reachedMinimum = false;
    }
}
