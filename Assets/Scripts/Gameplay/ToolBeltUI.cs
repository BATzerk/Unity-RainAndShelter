using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBeltUI : MonoBehaviour
{
    // Components
    [SerializeField] ToolIconTile[] iconTiles;
    //[SerializeField] Image i_selectedToolBorder;
    // References
    [SerializeField] private GameController gameController;


    // ----------------------------------------------------------------
    //  Awake / Destroy
    // ----------------------------------------------------------------
    private void Awake() {
        // Add event listeners!
        EventBus.Instance.PlayerToolBeltChangedEvent += OnPlayerToolBeltChanged;
    }
    private void OnDestroy() {
        // Remove event listeners!
        EventBus.Instance.PlayerToolBeltChangedEvent -= OnPlayerToolBeltChanged;
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnPlayerToolBeltChanged() {
        PlayerToolBelt tb = gameController.Player.ToolBelt;
        PlayerHand ph = gameController.Player.Hand;
        for (int i=0; i<iconTiles.Length; i++) {
            bool isSelected = i == ph.currToolIndex;
            iconTiles[i].SetVisualsFromTool(tb.tools[i], isSelected);
        }
    }

}
