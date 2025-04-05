using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")){
            MainControls.Instance.EnterShip();
        }
    }
    void OnTriggerExit(Collider col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Player")){
            MainControls.Instance.ExitShip();
        }
    }
}
