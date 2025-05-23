using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDControls : MonoBehaviour
{
    public static HUDControls Instance {get;set;}

    [SerializeField] List<TextMeshProUGUI> elementText = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> upgradesText = new List<TextMeshProUGUI>();
    public CanvasGroup overlay;
    [SerializeField] TextMeshProUGUI minText, secText, cursorText;
    [SerializeField] GameObject timerGO, cursorGO, cursorBG;



    float timeRemaining;

    bool timerIsOn = false;



    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    void Start(){
        UpdateAllReqs();
    }

    void Update(){
        if(timerIsOn){
            timeRemaining -= Time.deltaTime;
            if(timeRemaining <= 0){
                timeRemaining = 0;
                StartCoroutine(MainControls.Instance.ResetPlayer());
                timerIsOn = false;
            }
            UpdateTime();
        }
    }

    void UpdateTime(){
        float minutes = 0.0f;
        float seconds;
        
        if(timeRemaining >= 60.0f){
            minutes = Mathf.Floor(timeRemaining / 60.0f);
            seconds = Mathf.Floor(timeRemaining % 60.0f);
        } else {
            seconds = timeRemaining;
        }

        // minText.text = timeRemaining.ToString("00");
        minText.text = minutes.ToString("00");
        secText.text = seconds.ToString("00");
    }

    public void StartTimer(){
        timeRemaining = MainControls.Instance.timeFromShip;
        // if upgraded
        if(UpgradeControls.Instance.upgrades[0].level>0){
            timeRemaining = MainControls.Instance.timeFromShip * UpgradeControls.Instance.upgrades[0].level * UpgradeControls.Instance.upgrades[0].levelMult;
        }
        timerIsOn = true;
        timerGO.SetActive(true);
    }

    public void StopTimer(){
        timerIsOn = false;
        timerGO.SetActive(false);
    }

    public void UpdateElements(){
        for(int i=0; i<elementText.Count; i++){
            elementText[i].text = InventoryControls.Instance.elements[i].amount.ToString();
        }
    }


    public void UpdateUpgrades(){
        for(int i=0; i<upgradesText.Count; i++){
            if(UpgradeControls.Instance.upgrades[i].level == 5){
                upgradesText[i].text = "Max Level";
            } else {
                upgradesText[i].text = "Level " + UpgradeControls.Instance.upgrades[i].level.ToString();
            }
            
        }
    }

    public void UpdateAllReqs(){
        for(int i = 0; i < UpgradeControls.Instance.upgradeReqsParent.Count; i++){
            UpdateUpgradeReqs(i); // LATER ADD OTHER ELEMENTS
        }
    }

    public void UpdateUpgradeReqs(int upgradeNum){
        List<int> newElementsToShow = new List<int>();
        switch(UpgradeControls.Instance.upgrades[upgradeNum].level){
            case 0:
                for (int i=0; i<UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired0.Count; i++){
                    newElementsToShow.Add(UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired0[i]);
                }
                break;

            case 1:
                for (int i=0; i<UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired1.Count; i++){
                    newElementsToShow.Add(UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired1[i]);
                }
                break;

            case 2:
                for (int i=0; i<UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired2.Count; i++){
                    newElementsToShow.Add(UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired2[i]);
                }
                break;

            case 3:
                for (int i=0; i<UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired3.Count; i++){
                    newElementsToShow.Add(UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired4[i]);
                }
                break;
            case 4:
                for (int i=0; i<UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired4.Count; i++){
                    newElementsToShow.Add(UpgradeControls.Instance.upgrades[upgradeNum].elementsRequired4[i]);
                }
                break;
        }

        // Clear current list of reqs
        foreach(Transform t in UpgradeControls.Instance.upgradeReqsParent[upgradeNum]){
            Destroy(t.gameObject);
        }
        for(int i=0; i < newElementsToShow.Count; i++){
            GameObject reqImage = Instantiate(InventoryControls.Instance.elementImages[newElementsToShow[i]], UpgradeControls.Instance.upgradeReqsParent[upgradeNum]);
        }

        UpgradeControls.Instance.upgradeLevelText[upgradeNum].text = "Level " + UpgradeControls.Instance.upgrades[upgradeNum].level;

    }

    public void FadeOut(){
        overlay.alpha = 1.0f;
    }

    public void FadeIn(){
        overlay.alpha = 0.0f;
    }

    public void UpdateCursorText(string message){
        cursorText.text = message;
        cursorBG.SetActive(true);
    }

    public void HideCursorHover(){
        cursorGO.SetActive(false);
    }

    public void ShowCursorHover(){
        cursorGO.SetActive(true);
    }

    public void ClearCursor(){
        UpdateCursorText("");
        cursorBG.SetActive(false);
    }


}
