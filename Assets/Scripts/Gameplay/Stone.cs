using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour, IClickable {
    // Components
    [SerializeField] MeshRenderer bodyMesh;
    // References
    [SerializeField] Material m_bodyNormal;
    [SerializeField] Material m_bodyHovered;

    // Getters
    public bool IsClickable() { return true; }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, StoneData data) {
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
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        GameManagers.Instance.DataManager.PlayerInventory.ChangeStones(1);
        GameObject.Destroy(this.gameObject);
    }

    public void OnHoverOver() {
        bodyMesh.material = m_bodyHovered;
    }
    public void OnHoverOut() {
        if (bodyMesh == null) { return; } // Safety check.
        bodyMesh.material = m_bodyNormal;
    }


}
