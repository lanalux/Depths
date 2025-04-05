using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour
{
    public static GameControls Instance {get;set;}

    public bool gameIsPaused = false;

    [SerializeField] GameObject pauseScreen;

    void Awake(){
        if(Instance==null){
            Instance = this;
        }
    }

    public void PauseGame(){
        if(gameIsPaused){
            return;
        }
        gameIsPaused = true;
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        HUDControls.Instance.HideCursorHover();
        MainControls.Instance.fpc.enabled = false;
    }

    public void ResumeGame(){
        if(!gameIsPaused){
            return;
        }
        gameIsPaused = false;
        Time.timeScale = 1.0f;
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HUDControls.Instance.ShowCursorHover();
        MainControls.Instance.fpc.enabled = true;
    }

    public void QuitGame(){
        Application.Quit();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
