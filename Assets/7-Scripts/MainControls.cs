using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainControls : MonoBehaviour
{
    public static MainControls Instance {get;set;}


    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject progressCanvas;
    [SerializeField] Image progressFill;
    [SerializeField] Transform fpc;

    Camera cam;
    float interactionDistance = 8.0f;

    Transform currentTarget;
    RaycastHit hit;

    LayerMask currentLayer;

    bool isHovering = false;
    bool isMining = false;

    float miningTime = 2.0f;
    float timeElapsed = 0;

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
        cam = Camera.main;
    }

    void Update(){
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if(Physics.Raycast(ray, out hit, interactionDistance, layerMask)){
            currentTarget = hit.transform;
            currentLayer = layerMask;
            isHovering = true;
            // Debug.Log("Current Object = " + currentTarget.gameObject.name);
        } else {
            ClearRaycast();
        }

        if(isHovering && Input.GetMouseButton(0)){ //currentLayer == LayerMask.NameToLayer("Interactive") && 
            StartMining();
            ContinueMining();
        }

        if(isMining && Input.GetMouseButtonUp(0)){
            StopMining();
        }

    }

    void StartMining(){
        if(isMining){
            return;
        }
        isMining = true;
        timeElapsed = 0;
        progressCanvas.transform.position = currentTarget.position;
        progressCanvas.transform.LookAt(fpc);
        progressCanvas.SetActive(true);
    }


    void ContinueMining(){
        if(!isMining){
            return;
        }
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= miningTime){
            Collected();
            timeElapsed = miningTime;
            StopMining();
        }
        progressFill.fillAmount = timeElapsed/miningTime;
    }

    void StopMining(){
        if(!isMining){
            return;
        }
        isMining = false;
        progressCanvas.SetActive(false);
    }

    void Collected(){
        InventoryControls.Instance.elements[0].amount++;
        HUDControls.Instance.UpdateElements();
        Destroy(currentTarget.gameObject);
    }

    void ClearRaycast(){
        if(!isHovering){
            return;
        }
        isHovering = false;
        progressCanvas.SetActive(false);
        currentTarget = null;
        Debug.Log("Current Object = none");
    }
}
