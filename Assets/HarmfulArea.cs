using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmfulArea : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")){
            HarmControls.Instance.StartTakingDamage();
        }
    }

    void OnTriggerExit(Collider col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")){
            HarmControls.Instance.StopTakingDamage();
        }
    }
}
