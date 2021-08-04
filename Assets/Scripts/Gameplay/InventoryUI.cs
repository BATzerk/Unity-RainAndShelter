using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    // Components
    [SerializeField] TextMeshProUGUI t_numSticks;
    [SerializeField] TextMeshProUGUI t_numStones;
    [SerializeField] TextMeshProUGUI t_numString;
    // References
    [SerializeField] private GameController gameController;


    // ----------------------------------------------------------------
    //  Awake / Destroy
    // ----------------------------------------------------------------
    private void Awake() {
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
        UpdateInventoryTexts();
    }

    private void UpdateInventoryTexts() {
        PlayerInventory pi = gameController.Player.Inventory;
        t_numSticks.text = pi.NumSticks.ToString();
        t_numStones.text = pi.NumStones.ToString();
        t_numString.text = pi.NumStrings.ToString();
    }

}
