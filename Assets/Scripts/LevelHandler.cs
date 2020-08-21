using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class LevelHandler : MonoBehaviour
{
    public GameProgressData progress;
    public LevelSaveData levelSave;
    public CameraFollow cam;
    public List<LevelCheckPoint> AllCheckpoints;
    public CheckpointHandler currentCheckpoint;

    private List<MapBoundary> boundaries;
    private PlayerController player;
    private bool leaving;

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
        player.GetComponent<Damageable>().OnKill += TransitionOut;

        cam.transform.position = new Vector3(AllCheckpoints[levelSave.activeCheckPoint].checkPointEvent.newCameraX, player.transform.position.y, cam.transform.position.z);
        foreach (LevelCheckPoint checkP in AllCheckpoints)
        {
            checkP.checkPoint.OnActivated += () => SetNewCheckPoint(checkP);
        }
    }
    private void Update()
    {
        if (leaving)
        {
            if (!cam.GetComponent<TransitionEffect>().isAnimating)
            {
                RestartAtCheckpoint();
            }
        }
    }
    private void TransitionOut()
    {
        cam.GetComponent<TransitionEffect>().Transition(true);
        leaving = true;
    }
    private void RestartAtCheckpoint()
    {
        SceneManager.LoadScene(progress.CurrentLevel+1);
    }
    private void EndOfLevel()
    {
        if (progress.farthestLevel <= progress.CurrentLevel)
        {
            progress.farthestLevel = progress.CurrentLevel+1;
        }
        SceneManager.LoadScene(0);
    }
    private void SetNewCheckPoint(LevelCheckPoint cEvent)
    {
        cam.SetNewMinimum(cEvent.checkPointEvent.newCameraX);
        currentCheckpoint = cEvent.checkPoint;

        for (int i = 0; i < AllCheckpoints.Count; i++)
        {
            if (currentCheckpoint == AllCheckpoints[i].checkPoint)
            {
                if (i == AllCheckpoints.Count - 1)
                {
                    levelSave.activeCheckPoint = 0;
                    EndOfLevel();
                    break;
                }
                levelSave.activeCheckPoint = i;
                break;
            }
        }
        Debug.Log(currentCheckpoint);
    }
}
