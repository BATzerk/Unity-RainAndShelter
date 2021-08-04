using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventory {
    // Properties
    [SerializeField] private int numSticks;
    [SerializeField] private int numStones;
    [SerializeField] private int numTwines;

    // Getters / Setters
    public int NumSticks { get { return numSticks; } }
    public int NumStones { get { return numStones; } }
    public int NumTwines { get { return numTwines; } }
    public void SetNumSticks(int val) {
        numSticks = val;
        EventBus.Instance.OnPlayerInventoryChanged();
    }
    public void SetNumStones(int val) {
        numStones = val;
        EventBus.Instance.OnPlayerInventoryChanged();
    }
    public void SetNumTwines(int val) {
        numTwines = val;
        EventBus.Instance.OnPlayerInventoryChanged();
    }


    // Constructor
    public PlayerInventory() {
        numSticks = 0;
        numStones = 0;
        numTwines = 0;
    }


    // Doers
    public void ChangeSticks(int delta) {
        SetNumSticks(numSticks + delta);
    }
    public void ChangeStones(int delta) {
        SetNumStones(numStones + delta);
    }
    public void ChangeTwines(int delta) {
        SetNumTwines(numTwines + delta);
    }
    public void PayForCost(CraftableInfo craftableInfo) {
        ChangeSticks(-craftableInfo.SticksCost);
        ChangeStones(-craftableInfo.StonesCost);
        ChangeTwines(-craftableInfo.TwinesCost);
    }

}
