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
    [SerializeField] GameObject progressCanvas, particles1;
    [SerializeField] Image progressFill;
    [SerializeField] Transform startPos;
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
    bool isCollecting = false;

    float collectingTime = 3.0f;
    float timeElapsed = 0;

    bool isInShip = true;
    bool isWithinTether = true;
    float tetherTimer;
    float tetherTimerMax = 3.0f;

    [SerializeField] AudioSource collectingSFX, slurpSFX, voice, explosion;
    [SerializeField] AudioClip endClip;

    int currentElementBeingCollected;

    public bool raycastIsPaused = false;


    void Awake(){
        if(Instance==null){
            Instance=this;
        }
        cam = Camera.main;
    }


    void Start(){
        HUDControls.Instance.overlay.alpha = 1.0f;
        fpc.enabled = false;
        raycastIsPaused = true;
        StartCoroutine(StartOfGame());
    }

    IEnumerator StartOfGame(){
        explosion.Play();
        yield return new WaitForSeconds(1.0f);
        HUDControls.Instance.FadeIn();
        yield return new WaitForSeconds(1.0f);
        voice.Play();
        HUDControls.Instance.overlay.alpha = 0.0f;
        fpc.enabled = true;
        raycastIsPaused = false;
    }

    void Update(){
        if(raycastIsPaused){
            return;
        }


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
            
            if(currentLayer == LayerMask.NameToLayer("Interactive")){
                HUDControls.Instance.UpdateCursorText("[Hold E to Collect]");
            } 
            // HUDControls.Instance.UpdateCursorText(currentTarget.name);
            // Debug.Log("Current Object = " + currentTarget.gameObject.name);
        } else {
            ClearRaycast();
        }

        // Collecting
        // if(isHovering && currentLayer == LayerMask.NameToLayer("Interactive") && Input.GetMouseButton(0)){ 
        if(isHovering && currentLayer == LayerMask.NameToLayer("Interactive") && Input.GetKey(KeyCode.E)){ 
            StartCollecting();
            ContinueCollecting();
        }


        // Hover upgrade
        // if(isHovering && currentLayer == LayerMask.NameToLayer("Upgrade") && Input.GetMouseButtonDown(0)){
        if(isHovering && currentLayer == LayerMask.NameToLayer("Upgrade")){
            Upgrade upgradeScript = currentTarget.GetComponent<Upgrade>();
            if(upgradeScript!=null){
                bool canUpgrade = UpgradeControls.Instance.CheckToUpgrade(upgradeScript.upgradeNum);
                if(canUpgrade){
                    HUDControls.Instance.UpdateCursorText("[E to Upgrade]");
                } else {
                    HUDControls.Instance.UpdateCursorText("[More Components Needed]");
                }
                if(Input.GetKeyDown(KeyCode.E) && canUpgrade){
                    StartCoroutine(UpgradeControls.Instance.StartUpgrade(upgradeScript.upgradeNum));
                }
            }
        }

        
        // EndGameStuff upgrade
        // if(isHovering && currentLayer == LayerMask.NameToLayer("Upgrade") && Input.GetMouseButtonDown(0)){
        if(isHovering && currentLayer == LayerMask.NameToLayer("EndGame")){
            Upgrade upgradeScript = currentTarget.GetComponent<Upgrade>();
            if(upgradeScript!=null){
                bool canUpgrade = UpgradeControls.Instance.CheckToUpgrade(2);
                if(canUpgrade){
                    HUDControls.Instance.UpdateCursorText("[E to Upgrade]");
                } else {
                    HUDControls.Instance.UpdateCursorText("[More Components Needed]");
                }
                if(Input.GetKeyDown(KeyCode.E) && canUpgrade){
                    StartCoroutine(EndGame());
                }
            }
        }

        // Let go while collecting
        // if(isCollecting && Input.GetMouseButtonUp(0)){
        if(isCollecting && Input.GetKeyUp(KeyCode.E)){
            StopCollecting();
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
            HarmControls.Instance.tetherWarningText.text = "Returning you to ship in " + tetherTimer.ToString("f0")  + "...";
        }

    }

    float CheckDistance(){
        return Vector3.Distance(player.position, startPos.position);
    }

    Transform currentlyCollecting;

    Transform wobbler;

    void StartCollecting(){
        if(isCollecting){
            return;
        }
        isCollecting = true;
        HUDControls.Instance.HideCursorHover();

        currentlyCollecting = currentTarget;
        if(currentTarget.childCount > 0){
            wobbler = currentTarget.GetChild(0);
        } else {
            wobbler = null;
        }
        currentElementBeingCollected = currentlyCollecting.GetComponent<ElementInfo>().elementNum;
        timeElapsed = 0;
        // progressCanvas.transform.position = currentlyCollecting.position;
        // progressCanvas.transform.LookAt(player);
        progressCanvas.SetActive(true);
        particles1.transform.position = currentlyCollecting.position;
        particles1.SetActive(true);
        collectingSFX.transform.position = currentlyCollecting.position;
        AudioControls.Instance.StopFade(collectingSFX);
        collectingSFX.Play();
        wobbleStartPos = currentlyCollecting.position;
    }

    float wobbleTimer = 0.01f;
    float minWobbleTime = 0.01f;
    float maxWobbleTime = 0.02f;
    float minWobbleAmount = 0.01f;
    float maxWobbleAmount = 0.04f;
    Vector3 wobbleStartPos;

    void ContinueCollecting(){
        if(!isCollecting){
            return;
        }
        timeElapsed += Time.deltaTime;
        wobbleTimer -= Time.deltaTime;
        if(wobbleTimer <= 0.000f){
            wobbleTimer = Random.Range(minWobbleTime, maxWobbleTime);
            if(wobbler!=null){
                wobbler.position = wobbleStartPos + new Vector3(Random.Range(minWobbleAmount,maxWobbleAmount), Random.Range(minWobbleAmount,maxWobbleAmount), Random.Range(minWobbleAmount,maxWobbleAmount));
            } else {
                currentlyCollecting.position = wobbleStartPos + new Vector3(Random.Range(minWobbleAmount,maxWobbleAmount), Random.Range(minWobbleAmount,maxWobbleAmount), Random.Range(minWobbleAmount,maxWobbleAmount));
            }
        }

        if(timeElapsed >= collectingTime){
            Collected();
            timeElapsed = collectingTime;
            StopCollecting();
            StartCoroutine(CollectObject());
        }
        progressFill.fillAmount = timeElapsed/collectingTime;
    }

    void StopCollecting(){
        if(!isCollecting){
            return;
        }
        isCollecting = false;
        progressCanvas.SetActive(false);
        particles1.SetActive(false);
        AudioControls.Instance.FadeOutSource(collectingSFX);
        
        if(wobbler!=null){
            wobbler.position = wobbleStartPos;
        } else {
            currentlyCollecting.position = wobbleStartPos;
        }
        HUDControls.Instance.ShowCursorHover();
    }

    void Collected(){
        InventoryControls.Instance.elements[currentElementBeingCollected].amount++;
        HUDControls.Instance.UpdateElements();
        currentlyCollecting.GetComponent<Collider>().enabled = false;
        
    }

    float collectingDuration = 0.30f;

    IEnumerator CollectObject(){
        slurpSFX.Play();
        Vector3 startPos = currentlyCollecting.position;
        Vector3 endPos = player.position;
        float t = 0f;
        while(t < collectingDuration){
            t += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(t / collectingDuration);
            float easedT = 1f - Mathf.Pow(1f - normalizedTime, 3f);
            currentlyCollecting.position = Vector3.Lerp(startPos, endPos, easedT);

            // currentlyCollecting.position = Vector3.Lerp(startPos, endPos, t/collectingDuration);
            yield return null;
        }

        currentlyCollecting.position = endPos;
        Destroy(currentlyCollecting.gameObject);
    }

    void ClearRaycast(){
        if(!isHovering){
            return;
        }
        isHovering = false;
        StopCollecting();
        progressCanvas.SetActive(false);
        currentTarget = null;
        HUDControls.Instance.ClearCursor();
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
        HarmControls.Instance.warningTitleText.text = "TETHER DISTANCE EXCEEDED";
        HarmControls.Instance.tetherWarning.alpha = 1;
        HarmControls.Instance.warningSFX.Play();
    }

    void ReEnterTether(){
        if(isWithinTether){
            return;
        }
        isWithinTether = true;
        HarmControls.Instance.tetherWarning.alpha = 0;
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

    float fadeDuration = 3.0f;
    IEnumerator EndGame(){
        
        HUDControls.Instance.ClearCursor();
        raycastIsPaused = true;
        Instance.fpc.enabled = false;

        voice.clip = endClip;
        voice.Play();

        float startAlpha = 0.00f;
        float time = 0.0f;
        fadeDuration = 0.6f;
        while (time < fadeDuration){
            time += Time.deltaTime;
            HUDControls.Instance.overlay.alpha = Mathf.Lerp(startAlpha, 1.0f, time/fadeDuration);
            yield return null;
        }
        HUDControls.Instance.overlay.alpha = 1.0f;

        yield return new WaitForSeconds(3.0f);

        Application.Quit();
    }

}
