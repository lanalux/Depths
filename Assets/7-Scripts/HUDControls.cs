using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDControls : MonoBehaviour
{
    public static HUDControls Instance {get;set;}

    [SerializeField] List<TextMeshProUGUI> elementText = new List<TextMeshProUGUI>();

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    public void UpdateElements(){
        for(int i=0; i<elementText.Count; i++){
            elementText[i].text = InventoryControls.Instance.elements[i].amount.ToString();
        }
        
    }


}
