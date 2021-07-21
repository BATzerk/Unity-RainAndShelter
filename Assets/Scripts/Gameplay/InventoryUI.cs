using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    // Components
    [SerializeField] TextMeshProUGUI t_numSticks;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Start() {
        UpdateTexts();

        // Add event listeners!
        EventBus.Instance.PlayerInventoryChangedEvent += OnPlayerInventoryChanged;
    }
    private void OnDestroy() {
        // Remove event listeners!
        EventBus.Instance.PlayerInventoryChangedEvent -= OnPlayerInventoryChanged;
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnPlayerInventoryChanged() {
        UpdateTexts();
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    private void UpdateTexts() {
        t_numSticks.text = "sticks: " + dm.PlayerInventory.NumSticks;
    }


}
