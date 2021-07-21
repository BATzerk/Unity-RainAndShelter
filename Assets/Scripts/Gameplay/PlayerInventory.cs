using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventory {
    public int numSticks;


    public PlayerInventory() {
        numSticks = 0;
    }


    // Doers
    public void CollectStick() {
        numSticks++;
    }

}
