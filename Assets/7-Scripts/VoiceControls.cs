using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceControls : MonoBehaviour
{
    public static VoiceControls Instance {get;set;}
    [SerializeField] AudioSource voiceSource;

    void Awake(){
        if(Instance == null){
            Instance = this;
        }
    }

    public void PlayThenDelete(GameObject go, AudioClip clip){
        voiceSource.clip = clip;
        voiceSource.Play();
        Destroy(go);
    }
}
