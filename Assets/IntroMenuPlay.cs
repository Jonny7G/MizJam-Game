using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroMenuPlay : MonoBehaviour
{
    public int sceneToLoad;

    private bool loaded;
    public void LoadScene()
    {
        if (!loaded)
        {
            SceneManager.LoadScene(sceneToLoad);
            loaded = true;
        }
    }
}
