using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LevelHandler : MonoBehaviour
{
    public LevelSaveData levelSave;
    public CameraFollow cam;
    public List<LevelCheckPoint> AllCheckpoints;
    public CheckpointHandler currentCheckpoint;
    
    private List<MapBoundary> boundaries;
    private PlayerController player;
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
        currentCheckpoint = AllCheckpoints[levelSave.activeCheckPoint].checkPoint;
        player = FindObjectOfType<PlayerController>();
        player.transform.position = currentCheckpoint.transform.position;
        cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y,cam.transform.position.z);
        foreach (LevelCheckPoint checkP in AllCheckpoints)
        {
            checkP.checkPoint.OnActivated += () => SetNewCheckPoint(checkP);
        }
    }
    private void RestartAtCheckpoint(PlayerController player)
    {
        
    }
    private void SetNewCheckPoint(LevelCheckPoint cEvent)
    {
        Debug.Log("new point added");
        cam.SetNewMinimum(cEvent.checkPointEvent.newCameraX);
        currentCheckpoint = cEvent.checkPoint;

        for(int i = 0; i < AllCheckpoints.Count; i++)
        {
            if(currentCheckpoint == AllCheckpoints[i].checkPoint)
            {
                levelSave.activeCheckPoint = i;
                break;
            }
        }

        Debug.Log(currentCheckpoint);
    }
}
