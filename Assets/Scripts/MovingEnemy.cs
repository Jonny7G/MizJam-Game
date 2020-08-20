using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    [SerializeField] private Targetable targetedHandler;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    [SerializeField] private Timer stopTime;
    [SerializeField] private bool stopAtMin, stopAtMax;
    
    private Vector2 currentTarget;
    private Vector2 currDir;
    private bool atMin;
    private void Start()
    {
        currDir = direction;
        currentTarget = (Vector2)transform.position + direction * distance;
    }
    private void Update()
    {
        if (!targetedHandler.IsGrappled)
        {
            if (stopTime.TimerEnded)
            {
                transform.position = Vector2.MoveTowards(transform.position, currentTarget, Time.deltaTime * speed);
                if ((Vector2)transform.position == currentTarget)
                {
                    currDir = -currDir;
                    currentTarget = (Vector2)transform.position + currDir * distance;
                    if (atMin && stopAtMin || !atMin && stopAtMax)
                    {
                        stopTime.RestartTimer();
                    }
                    atMin = !atMin;
                }
            }
            else
            {
                stopTime.UpdateTimer();
            }
        }
    }
}
