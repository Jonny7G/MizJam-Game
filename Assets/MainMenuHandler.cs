using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuHandler : MonoBehaviour
{
    public GameProgressData progress;
    public List<MenuLevel> allLevels;
    public MenuLevel selectedLevel;
    private void Start()
    {
        if (progress.farthestLevel < 1)
        {
            progress.farthestLevel = 1;
        }
        for (int i = 0; i < progress.farthestLevel; i++)
        {
            if (i < allLevels.Count)
            {
                allLevels[i].SetUnlocked();
            }
        }
    }
    public void WasSelected(MenuLevel level)
    {
        if (level.Unlocked)
        {
            if (selectedLevel != null)
                selectedLevel.Select(false);

            selectedLevel = level;
            selectedLevel.Select(true);
        }
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
        return;
        if (selectedLevel != null)
        {
            for (int i = 0; i < allLevels.Count; i++)
            {
                if (selectedLevel == allLevels[i])
                {
                    SceneManager.LoadScene(i + 1);
                    break;
                }
            }
        }
    }
}
