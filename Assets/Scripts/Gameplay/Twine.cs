using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twine : MonoBehaviour, IClickable {



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, StickData data) {
        Initialize(parent, data.pos, data.rot);
    }
    public void Initialize(Transform parent, Vector3 _pos, Vector3 _rot) {
        this.transform.parent = parent;
        this.transform.position = _pos;
        this.transform.eulerAngles = _rot;
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public bool IsClickable(Tool tool) { return true; }
    public CursorType CurrCursorForMe(Tool tool) { return CursorType.Hand; }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        player.Inventory.ChangeTwines(1);
        GameObject.Destroy(this.gameObject);
    }

    public void OnHoverOver() {
        //bodyMeshA.material = m_bodyHovered;
        //bodyMeshB.material = m_bodyHovered;
    }
    public void OnHoverOut() {
        //if (bodyMeshA == null) { return; } // Safety check.
        //bodyMeshA.material = m_bodyNormal;
        //bodyMeshB.material = m_bodyNormal;
    }


}
