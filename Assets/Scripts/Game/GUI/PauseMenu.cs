using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused=false;
    public GameObject PauseMenuUI;
    Button FirstButton;

    void Start()
    {
        FirstButton=transform.GetChild(0).GetChild(0).GetComponent<Button>();
        FirstButton.Select();
    }
    

    public void Resume(){
        PauseMenuUI.SetActive(false);
        Time.timeScale=1f;
        GameIsPaused=false;
    }

    public void Pause(){
        PauseMenuUI.SetActive(true);
        Time.timeScale=0f;
        GameIsPaused=true;
    }

    public void LoadMenu()
    {
        Debug.Log("Back to main menu");
        Resume();
        //SceneManager.LoadScene("Menu");
    }

    public void ResetGame()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenClosePauseMenu(InputAction.CallbackContext context)
    {
        if(GameIsPaused){
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
