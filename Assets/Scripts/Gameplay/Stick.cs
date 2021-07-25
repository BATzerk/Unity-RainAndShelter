﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour, IClickable {
    // Components
    [SerializeField] MeshRenderer bodyMeshA;
    [SerializeField] MeshRenderer bodyMeshB;
    // References
    [SerializeField] Material m_bodyNormal;
    [SerializeField] Material m_bodyHovered;

    // Getters
    public bool IsClickable() { return true; }



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
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        GameManagers.Instance.DataManager.PlayerInventory.ChangeSticks(1);
        GameObject.Destroy(this.gameObject);
    }

    public void OnHoverOver() {
        bodyMeshA.material = m_bodyHovered;
        bodyMeshB.material = m_bodyHovered;
    }
    public void OnHoverOut() {
        if (bodyMeshA == null) { return; } // Safety check.
        bodyMeshA.material = m_bodyNormal;
        bodyMeshB.material = m_bodyNormal;
    }


}