using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuHandler : MonoBehaviour
{
    public GameProgressData progress;
    public PlayerController player;
    public List<MenuLevel> allLevels;
    //public MenuLevel selectedLevel;
    private void Start()
    {
        if (progress.farthestLevel < 0)
        {
            progress.farthestLevel = 0;
        }
        for (int i = 0; i <= progress.farthestLevel; i++)
        {
            if (i < allLevels.Count)
            {
                allLevels[i].SetUnlocked();
            }
        }
        if (progress.CurrentLevel > allLevels.Count-1)
        {
            progress.CurrentLevel = allLevels.Count - 1;
        }
        player.transform.position = allLevels[progress.CurrentLevel].transform.position;
    }
    public void Play(MenuLevel level)
    {

        if (level != null)
        {
            for (int i = 0; i < allLevels.Count; i++)
            {
                if (level == allLevels[i])
                {
                    progress.CurrentLevel = i;
                    SceneManager.LoadScene(i + 1);
                    break;
                }
            }
        }
    }
}
