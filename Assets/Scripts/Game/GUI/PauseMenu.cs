using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUI;

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameState.State = eGameState.RUNNING;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameState.State = eGameState.PAUSED;
    }

    public void LoadMenu()
    {
        Debug.Log("Back to main menu");
        //SceneManager.LoadScene("Menu");
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenClosePauseMenu(InputAction.CallbackContext context)
    {
        if(GameState.State == eGameState.PAUSED)
            Resume();
        else
            Pause();
    }
}
