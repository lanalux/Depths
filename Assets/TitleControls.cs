using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControls : MonoBehaviour
{
    float fadeDuration = 2.0f;
    [SerializeField] CanvasGroup overlay;

    bool startedGame = false;

    Coroutine fadingIn;

    void Start(){
        fadingIn = StartCoroutine(FadeIn());
    }

    public void StartGame(){
        if(startedGame){
            return;
        }
        startedGame = true;
        StartCoroutine(FadeOutAndStart());
    }

    IEnumerator FadeIn(){
        float startAlpha = 1.0f;
        float time = 0;
        while( time < fadeDuration){
            time += Time.deltaTime;
            overlay.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }
        overlay.alpha = 0f;
    }

    IEnumerator FadeOutAndStart(){
        float startAlpha = 0f;
        float time = 0;
        while( time < fadeDuration){
            time += Time.deltaTime;
            overlay.alpha = Mathf.Lerp(startAlpha, 1.0f, time / fadeDuration);
            yield return null;
        }
        overlay.alpha = 1.0f;
        SceneManager.LoadScene("Main");
    }

    public void QuitGame(){
        Application.Quit();
    }
}
