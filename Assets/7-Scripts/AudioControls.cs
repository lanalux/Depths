using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControls : MonoBehaviour
{
    public static AudioControls Instance {get;set;}

    Coroutine fadeOutClip;

    float fadeDuration = 0.3f;


    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }


    public void FadeOutSource(AudioSource source){
        fadeOutClip = StartCoroutine(FadeOutAudio(source, fadeDuration));
    }

    public void StopFade(AudioSource source){
        if(fadeOutClip != null){
            StopCoroutine(fadeOutClip);
            source.volume = 1.0f;
        }
    }


    public IEnumerator FadeOutAudio(AudioSource source, float duration){
        float startVolume = source.volume;
        float t=0f;
        while(t < duration){
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;
    }
}
