using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Upgrades{
    public string name;
    public int level = 0;
    public float levelMult;
    public List<int> elementsRequired0;
    public List<int> elementsRequired1;
    public List<int> elementsRequired2;
    public List<int> elementsRequired3;
    public List<int> elementsRequired4;
}
public class UpgradeControls : MonoBehaviour
{
    public static UpgradeControls Instance {get;set;}

    public List<Upgrades> upgrades = new List<Upgrades>();
    
    public List<Transform> upgradeReqsParent = new List<Transform>();
    
    public List<TextMeshProUGUI> upgradeLevelText = new List<TextMeshProUGUI>();
    [SerializeField] AudioSource workingSFX;

    bool hasElementsNeeded;

    bool startedUpgrade = false;

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    public bool CheckToUpgrade(int upgradeNum){
        hasElementsNeeded = true;
        List<int> elementsNeeded = new List<int>();
        switch(upgrades[upgradeNum].level){
            case 0:
                foreach(int i in upgrades[upgradeNum].elementsRequired0){
                    elementsNeeded.Add(i);
                }
                break;
            case 1:
                foreach(int i in upgrades[upgradeNum].elementsRequired1){
                    elementsNeeded.Add(i);
                }
                break;
            case 2:
                foreach(int i in upgrades[upgradeNum].elementsRequired2){
                    elementsNeeded.Add(i);
                }
                break;
            case 3:
                foreach(int i in upgrades[upgradeNum].elementsRequired3){
                    elementsNeeded.Add(i);
                }
                break;
            case 4:
                foreach(int i in upgrades[upgradeNum].elementsRequired4){
                    elementsNeeded.Add(i);
                }
                break;
            default:
                hasElementsNeeded = false;
                break;
        }
        List<int> amountOwned = new List<int>();
        foreach(Elements e in InventoryControls.Instance.elements){
            amountOwned.Add(e.amount);
        }
        for(int i = elementsNeeded.Count-1; i >= 0; i--){
            if(amountOwned[elementsNeeded[i]] > 0 ){
                amountOwned[elementsNeeded[i]]--;
                elementsNeeded.RemoveAt(i);
            } else {
                hasElementsNeeded = false;
            }
        }
        return hasElementsNeeded;
    }

    float fadeDuration;
    

    public IEnumerator StartUpgrade(int upgradeNum){
        if(startedUpgrade || !hasElementsNeeded){
            yield break;
        }
        startedUpgrade = true;
        
        HUDControls.Instance.ClearCursor();
        MainControls.Instance.raycastIsPaused = true;
        MainControls.Instance.fpc.enabled = false;
        float startAlpha = 0.00f;
        float time = 0.0f;
        fadeDuration = 0.6f;
        while (time < fadeDuration){
            time += Time.deltaTime;
            HUDControls.Instance.overlay.alpha = Mathf.Lerp(startAlpha, 1.0f, time/fadeDuration);
            yield return null;
        }
        HUDControls.Instance.overlay.alpha = 1.0f;
        workingSFX.Play();



        switch(upgrades[upgradeNum].level){
            case 0:
                for (int i=0; i<upgrades[upgradeNum].elementsRequired0.Count; i++){
                    InventoryControls.Instance.elements[upgrades[upgradeNum].elementsRequired0[i]].amount--;
                }
                break;
            case 1:
                for (int i=0; i<upgrades[upgradeNum].elementsRequired1.Count; i++){
                    InventoryControls.Instance.elements[upgrades[upgradeNum].elementsRequired1[i]].amount--;
                }
                break;
            case 2:
                for (int i=0; i<upgrades[upgradeNum].elementsRequired2.Count; i++){
                    InventoryControls.Instance.elements[upgrades[upgradeNum].elementsRequired2[i]].amount--;
                }
                break;
            case 3:
                for (int i=0; i<upgrades[upgradeNum].elementsRequired3.Count; i++){
                    InventoryControls.Instance.elements[upgrades[upgradeNum].elementsRequired3[i]].amount--;
                }
                break;
            case 4:
                for (int i=0; i<upgrades[upgradeNum].elementsRequired4.Count; i++){
                    InventoryControls.Instance.elements[upgrades[upgradeNum].elementsRequired4[i]].amount--;
                }
                break;
        }
        upgrades[upgradeNum].level ++;
        HUDControls.Instance.UpdateUpgrades();
        HUDControls.Instance.UpdateElements();
        HUDControls.Instance.UpdateUpgradeReqs(upgradeNum);

        yield return new WaitForSeconds(2.5f);

        startAlpha = 1.00f;
        time = 0.0f;
        fadeDuration = 1.2f;            
        while (time < 1.0f){
            time += Time.deltaTime;
            HUDControls.Instance.overlay.alpha = Mathf.Lerp(startAlpha, 0.0f, time/fadeDuration);
            yield return null;
        }

        HUDControls.Instance.overlay.alpha = 0;

    
        MainControls.Instance.fpc.enabled = true;
        MainControls.Instance.raycastIsPaused = false;
        startedUpgrade = false;

        if(upgradeNum == 3){
            switch(upgrades[upgradeNum].level){
                case 1:
                    MainControls.Instance.fpc.m_WalkSpeed = 5;
                    MainControls.Instance.fpc.m_RunSpeed = 7;
                    break;
                case 2:
                    MainControls.Instance.fpc.m_WalkSpeed = 6;
                    MainControls.Instance.fpc.m_RunSpeed = 8;
                    break;
                case 3:
                    MainControls.Instance.fpc.m_WalkSpeed = 7;
                    MainControls.Instance.fpc.m_RunSpeed = 9;
                    break;
                case 4:
                    MainControls.Instance.fpc.m_WalkSpeed = 8;
                    MainControls.Instance.fpc.m_RunSpeed = 10;
                    break;
                case 5:
                    MainControls.Instance.fpc.m_WalkSpeed = 9;
                    MainControls.Instance.fpc.m_RunSpeed = 11;
                    break;
            }
        }

    }
}
