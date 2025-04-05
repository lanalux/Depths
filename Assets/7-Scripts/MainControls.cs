using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class MainControls : MonoBehaviour
{
    public static MainControls Instance {get;set;}

    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject progressCanvas;
    [SerializeField] Image progressFill;
    [SerializeField] Transform startPos;
    [SerializeField] CanvasGroup tetherWarning;
    public Transform player;
    public FirstPersonController fpc;

    Camera cam;

    ////////////////// UPDATE THESE IN INSPECTOR! /////////////
    public float interactionDistance;
    public float timeFromShip;
    public float currentHealth;
    public float maxHealth;
    public float damageAmount;

    
    public float maxDistanceToShip;
    float bufferAmount = 0.3f;

    Transform currentTarget;
    RaycastHit hit;

    LayerMask currentLayer;

    bool isHovering = false;
    bool isMining = false;

    float miningTime = 2.0f;
    float timeElapsed = 0;

    bool isInShip = true;
    bool isWithinTether = true;
    float tetherTimer;
    float tetherTimerMax = 3.0f;
    [SerializeField] TextMeshProUGUI tetherWarningText;



    int currentElementBeingMined;


    void Awake(){
        if(Instance==null){
            Instance=this;
        }
        cam = Camera.main;
    }

    void Start(){
        HarmControls.Instance.ResetHealth();
    }

    void Update(){
        if(GameControls.Instance.gameIsPaused){
            if(Input.GetKeyDown(KeyCode.Escape)){
                GameControls.Instance.ResumeGame();
            }
            return;
        }

        // Pause Game
        if(Input.GetKeyDown(KeyCode.Escape)){
            GameControls.Instance.PauseGame();
        }


        // Check view
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if(Physics.Raycast(ray, out hit, interactionDistance, layerMask)){
            currentTarget = hit.transform;
            currentLayer = currentTarget.gameObject.layer;
            isHovering = true;
            
            HUDControls.Instance.UpdateCursorText(currentTarget.name);
            // Debug.Log("Current Object = " + currentTarget.gameObject.name);
        } else {
            ClearRaycast();
        }

        // Mining
        if(isHovering && currentLayer == LayerMask.NameToLayer("Interactive") && Input.GetMouseButton(0)){ 
            StartMining();
            ContinueMining();
        }

        // Upgrading
        if(isHovering && currentLayer == LayerMask.NameToLayer("Upgrade") && Input.GetMouseButtonDown(0)){
            Upgrade upgradeScript = currentTarget.GetComponent<Upgrade>();
            if(upgradeScript!=null){
                UpgradeControls.Instance.UpgradeThis(upgradeScript.upgradeNum);
            }
        }

        // Let go while mining
        if(isMining && Input.GetMouseButtonUp(0)){
            StopMining();
        }

        // Check Distance - Distance is upgrade 1
        if(!isInShip){
            float maxDistance = maxDistanceToShip;
            if(UpgradeControls.Instance.upgrades[1].level > 0){
                maxDistance = maxDistanceToShip * UpgradeControls.Instance.upgrades[1].levelMult * UpgradeControls.Instance.upgrades[1].level;
            }
            if(CheckDistance() > maxDistance + bufferAmount){
                ExitTether();
            }
        }

        // Tether timer - Distance is upgrade 1
        if(!isWithinTether){
            float maxDistance = maxDistanceToShip;
            if(UpgradeControls.Instance.upgrades[1].level > 0){
                maxDistance = maxDistanceToShip * UpgradeControls.Instance.upgrades[1].levelMult * UpgradeControls.Instance.upgrades[1].level;
            }
            if(CheckDistance() < maxDistance){
                ReEnterTether();
                return;
            }
            tetherTimer -= Time.deltaTime;
            if(tetherTimer <= 0){
                tetherTimer = 0;
                StartCoroutine(ResetPlayer());
            }
            tetherWarningText.text = "Returning you to ship in " + tetherTimer.ToString("f0")  + "...";
        }

    }

    float CheckDistance(){
        return Vector3.Distance(player.position, startPos.position);
    }

    void StartMining(){
        if(isMining){
            return;
        }
        isMining = true;
        currentElementBeingMined = currentTarget.GetComponent<ElementInfo>().elementNum;
        timeElapsed = 0;
        progressCanvas.transform.position = currentTarget.position;
        progressCanvas.transform.LookAt(player);
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
        InventoryControls.Instance.elements[currentElementBeingMined].amount++;
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
        HUDControls.Instance.UpdateCursorText("");
    }

    public void EnterShip(){
        if(isInShip){
            return;
        }
        isInShip = true;
        ReEnterTether();
        HarmControls.Instance.ResetHealth();
        HUDControls.Instance.StopTimer();
    }

    public void ExitShip(){
        if(!isInShip){
            return;
        }
        isInShip = false;
        HUDControls.Instance.StartTimer();
    }

    void ExitTether(){
        if(!isWithinTether){
            return;
        }
        tetherTimer = tetherTimerMax;
        isWithinTether = false;
        tetherWarning.alpha = 1;
    }

    void ReEnterTether(){
        if(isWithinTether){
            return;
        }
        isWithinTether = true;
        tetherWarning.alpha = 0;
    }

    public IEnumerator ResetPlayer(){
        HUDControls.Instance.FadeOut();
        fpc.enabled = false;
        player.SetPositionAndRotation(startPos.position, startPos.rotation);
        EnterShip();
        yield return new WaitForSeconds(1.0f);
        fpc.enabled = true;
        HUDControls.Instance.FadeIn();
    }



}
