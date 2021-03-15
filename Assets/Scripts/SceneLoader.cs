using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{

    public void LoadLearnScene()
    {
        SceneManager.LoadSceneAsync("learn");
    }
    public void LoadReviewScene()
    {
        SceneManager.LoadSceneAsync("review");
    }
    public void LoadBossScene()
    {
        SceneManager.LoadSceneAsync("boss");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("menu");
    }
}
