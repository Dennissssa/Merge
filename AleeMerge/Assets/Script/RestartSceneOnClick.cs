using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneOnClick : MonoBehaviour
{
    [Header("Optional Delay")]
    public float delayBeforeRestart = 0f;

    public void RestartScene()
    {
        if (delayBeforeRestart > 0f)
        {
            Invoke(nameof(DoRestart), delayBeforeRestart);
        }
        else
        {
            DoRestart();
        }
    }

    void DoRestart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
