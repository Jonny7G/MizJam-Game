using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LevelHandler : MonoBehaviour
{
    public CameraFollow cam;
    public List<LevelCheckPoint> AllCheckpoints;

    public CheckpointHandler currentCheckpoint;
    private List<MapBoundary> boundaries;

    [System.Serializable]
    public class LevelCheckPoint
    {
        public CheckpointHandler checkPoint;
        public CheckPointEventHandler checkPointEvent;
    }
    [System.Serializable]
    public class CheckPointEventHandler
    {
        public float newCameraX;
    }
    private void Awake()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<CameraFollow>();
        }
        boundaries = FindObjectsOfType<MapBoundary>().ToList();
        foreach (MapBoundary boundary in boundaries)
        {
            boundary.OnPlayerHitBoundary += RestartAtCheckpoint;
        }
    }
    private void Start()
    {
        if (currentCheckpoint == null)
            currentCheckpoint = AllCheckpoints[0].checkPoint;
        
        foreach (LevelCheckPoint checkP in AllCheckpoints)
        {
            checkP.checkPoint.OnActivated += () => SetNewCheckPoint(checkP);
        }
    }
    private void RestartAtCheckpoint(PlayerController player)
    {
        player.transform.position = currentCheckpoint.transform.position;
    }
    private void SetNewCheckPoint(LevelCheckPoint cEvent)
    {
        Debug.Log("new point added");
        cam.SetNewMinimum(cEvent.checkPointEvent.newCameraX);
        currentCheckpoint = cEvent.checkPoint;
        Debug.Log(currentCheckpoint);
    }
}
