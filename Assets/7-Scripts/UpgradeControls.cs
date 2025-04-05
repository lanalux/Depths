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

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    public void UpgradeThis(int upgradeNum){
        bool hasElementsNeeded = true;
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

        if(hasElementsNeeded){
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
        }
    }
}
