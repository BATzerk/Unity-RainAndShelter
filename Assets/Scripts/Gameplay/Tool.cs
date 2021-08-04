using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ToolType : int {
    Hand,
    Axe, Shovel
}


[Serializable]
public class Tool
{
    // Properties
    public ToolType MyType;// { get; private set; }
    public float Health;// { get; private set; }

    // Getters
    public float HealthPercent() { return Health; } // TODO: This.


    public Tool(ToolType type, float health) {
        this.MyType = type;
        this.Health = health;
    }


}
