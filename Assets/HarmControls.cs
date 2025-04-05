using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmControls : MonoBehaviour
{
    public static HarmControls Instance {get;set;}

    bool isTakingDamage = false;
    bool redFlashIsOn = false;

    float redFlashTime = 0.3f;
    float currentRedFlashTime = 0;

    [SerializeField] CanvasGroup redOverlay;
    [SerializeField] RectTransform healthBar;

    float healthBarWidth;
    float healthBarHeight;

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    void Start(){
        healthBarWidth = healthBar.sizeDelta.x;
        healthBarHeight = healthBar.sizeDelta.y;
    }

    void Update(){
        if(GameControls.Instance.gameIsPaused){
            return;
        }
        if(isTakingDamage){
            ContinuTakingDamage();
        }
    }

    public void StartTakingDamage(){
        if(isTakingDamage){
            return;
        }
        isTakingDamage = true;
        currentRedFlashTime = redFlashTime;
    }

    public void ContinuTakingDamage(){
        currentRedFlashTime -= Time.deltaTime;
        if(currentRedFlashTime <= 0 ){
            if(redFlashIsOn){
                redOverlay.alpha = 0.0f;
                redFlashIsOn = false;
            } else {    
                redOverlay.alpha = 1.0f;
                redFlashIsOn = true;
            }
            currentRedFlashTime = redFlashTime;
        }
        
        MainControls.Instance.currentHealth -= Time.deltaTime * MainControls.Instance.damageAmount;
        if(MainControls.Instance.currentHealth<=0){
            MainControls.Instance.currentHealth = 0;
            StartCoroutine(MainControls.Instance.ResetPlayer());
        }
        float currentWidth = (MainControls.Instance.currentHealth/MainControls.Instance.maxHealth) * healthBarWidth;
        healthBar.sizeDelta = new Vector2(currentWidth, healthBarHeight);
    }

    public void StopTakingDamage(){
        redOverlay.alpha = 0.0f;
        redFlashIsOn = true;
        if(!isTakingDamage){
            return;
        }
        isTakingDamage = false;
    }

    public void ResetHealth(){
        MainControls.Instance.currentHealth = MainControls.Instance.maxHealth;
        healthBar.sizeDelta = new Vector2(healthBarWidth, healthBarHeight);
    }

}
