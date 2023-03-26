using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }

        if (isPaused && Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0.0f;
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1.0f;
            Debug.Log("Game Unpaused");
        }
    }

    private void ResetScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}