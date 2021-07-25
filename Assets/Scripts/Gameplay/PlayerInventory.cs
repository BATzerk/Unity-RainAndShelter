using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventory {
    // Properties
    [SerializeField] private int numSticks;
    [SerializeField] private int numStones;

    // Getters / Setters
    public int NumSticks { get { return numSticks; } }
    public int NumStones { get { return numStones; } }
    public void SetNumSticks(int val) {
        numSticks = val;
        EventBus.Instance.OnPlayerInventoryChanged();
    }
    public void SetNumStones(int val) {
        numStones = val;
        EventBus.Instance.OnPlayerInventoryChanged();
    }


    // Constructor
    public PlayerInventory() {
        numSticks = 0;
        numStones = 0;
    }


    // Doers
    public void ChangeSticks(int delta) {
        SetNumSticks(numSticks + delta);
    }
    public void ChangeStones(int delta) {
        SetNumStones(numStones + delta);
    }
    public void PayForCost(PlaceableInfo placeableInfo) {
        ChangeSticks(-placeableInfo.SticksCost);
        ChangeStones(-placeableInfo.StonesCost);
    }

}
