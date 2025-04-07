using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVoiceThenDelete : MonoBehaviour
{
    public AudioClip clipToPlay;
    void OnTriggerEnter(Collider col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")){
            VoiceControls.Instance.PlayThenDelete(this.gameObject, clipToPlay);
        }
    }
}
