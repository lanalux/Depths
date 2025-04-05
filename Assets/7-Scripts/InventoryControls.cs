using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Elements{
    public int amount;
}

public class InventoryControls : MonoBehaviour
{
    public static InventoryControls Instance {get;set;}

    public List<Elements> elements = new List<Elements>();

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }
}
