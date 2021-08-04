using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerToolBelt
{
    // Properties
    [SerializeField] public Tool[] tools = new Tool[6];
    // References
    [NonSerialized] private Player myPlayer;


    // Constructor
    public PlayerToolBelt() {
    }
    public void SetMyPlayer(Player myPlayer) {
        this.myPlayer = myPlayer;
    }


    // Doers
    public void TryToCraftTool(ToolType toolType) {
        // Find the first available slot to add a tool.
        int openSlot = -1;
        for (int i=0; i<tools.Length; i++) {
            if (tools[i]==null || tools[i].MyType == ToolType.Hand) {
                openSlot = i;
                break;
            }
        }
        // If there IS an open slot...!
        if (openSlot != -1) {
            CraftableInfo craftableInfo = CraftableInfo.GetInfoFromType(toolType);
            if (craftableInfo.CanAfford(myPlayer.Inventory)) {
                Tool newTool = new Tool(toolType, 1);
                tools[openSlot] = newTool;
                myPlayer.Inventory.PayForCost(craftableInfo);
                EventBus.Instance.OnPlayerToolBeltChanged();
            }
            else {
                Debug.Log("Feedback: Insufficient resources to make a " + toolType + ".");
            }
        }
    }


}
