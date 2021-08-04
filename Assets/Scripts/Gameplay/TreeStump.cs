using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TreeStump : MonoBehaviour, IClickable {
    // Components
    [SerializeField] MeshRenderer mr_bodyHighlight;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, TreeStumpData data) {
        Initialize(parent, data.pos, data.rot, data.scale);//, data.bushType, data.numResourcesLeft);
    }
    public void Initialize(Transform parent, Vector3 pos,Vector3 rot,Vector3 scale) {
        // Transform
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;

        // Setup basics.
        mr_bodyHighlight.enabled = false;
    }





    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public bool IsClickable(Tool tool) {
        return tool.MyType == ToolType.Shovel;
    }
    public CursorType CurrCursorForMe(Tool tool) {
        if (tool.MyType==ToolType.Shovel) return CursorType.Dig;
        return CursorType.Undefined;
    }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        // It's a shovel...!
        if (player.Hand.currTool.MyType == ToolType.Shovel) {
            // Damage the tool used.
            player.Hand.DamageCurrTool(0.1f);
            // Simply destroy me for now ;)
            Destroy(this.gameObject);
        }
    }

    public void OnHoverOver() {
        //mr_bodyHighlight.enabled = true;
    }
    public void OnHoverOut() {
        //if (mr_bodyHighlight == null) { return; } // Safety check.
        //mr_bodyHighlight.enabled = false;
    }


}
