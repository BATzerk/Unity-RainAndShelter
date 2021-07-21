using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventory {
    // Properties
    [SerializeField] private int numSticks;

    // Getters / Setters
    public int NumSticks { get { return numSticks; } }
    public void SetNumSticks(int val) {
        numSticks = val;
        EventBus.Instance.OnPlayerInventoryChanged();
    }


    // Constructor
    public PlayerInventory() {
        numSticks = 0;
    }


    // Doers
    public void CollectStick() {
        SetNumSticks(numSticks + 1);
    }

}
