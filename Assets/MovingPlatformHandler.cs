using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformHandler : MonoBehaviour
{
    public int Direction;
    public float Speed;
    [SerializeField] private GameObject platform;
    [SerializeField] private List<Transform> points;

    public int currentPoint = 1;
    private bool hasLetGo = true;
    private void Update()
    {
        if (currentPoint <= points.Count - 1 && currentPoint >= 0)
        {
            platform.transform.position = Vector2.MoveTowards(platform.transform.position, points[currentPoint].position, Time.deltaTime * Speed);
            if ((Vector2)platform.transform.position == (Vector2)points[currentPoint].position)
            {
                if (Direction == 1 && currentPoint < points.Count - 1 || Direction == -1 && currentPoint > 0)
                {
                    currentPoint += Direction;
                }
                else
                {
                    Direction = -Direction;
                    currentPoint += Direction;
                }
            }
        }
    }
}
