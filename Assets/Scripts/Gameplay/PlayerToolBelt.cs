using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerToolBelt
{
    // Properties
    [SerializeField] public Tool[] tools = new Tool[6];


    // Constructor
    public PlayerToolBelt() {
        //// QQQ TEMP!
        //tools[0] = new Tool(ToolType.Axe, 1);
        //tools[1] = new Tool(ToolType.Shovel, 1);
    }


}
