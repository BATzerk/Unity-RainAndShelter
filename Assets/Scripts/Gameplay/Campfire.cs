using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IClickable {
    //// Components
    //[SerializeField] MeshRenderer bodyMesh;
    [SerializeField] private Light pointLight;
    // Properties
    public bool IsLit { get; private set; }
    // References
    //[SerializeField] Material m_bodyNormal;
    //[SerializeField] Material m_bodyHovered;

    // Getters
    public bool IsClickable() { return true; }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, Transform myTF, bool isLit) {
        Initialize(parent, myTF.position,myTF.eulerAngles, isLit);
    }
    public void Initialize(Transform parent, CampfireData data) {
        Initialize(parent, data.pos, data.rot, data.isLit);
    }
    public void Initialize(Transform parent, Vector3 _pos, Vector3 _rot, bool _isLit) {
        this.transform.parent = parent;
        this.transform.position = _pos;
        this.transform.eulerAngles = _rot;
        SetIsLit(_isLit);
    }


    private void SetIsLit(bool val) {
        IsLit = val;

        pointLight.enabled = IsLit;
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public CursorType CurrCursorForMe() { return CursorType.Circle; }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        SetIsLit(!IsLit);
    }

    public void OnHoverOver() {
        //bodyMesh.material = m_bodyHovered;
    }
    public void OnHoverOut() {
        //if (bodyMesh == null) { return; } // Safety check.
        //bodyMesh.material = m_bodyNormal;
    }


}
