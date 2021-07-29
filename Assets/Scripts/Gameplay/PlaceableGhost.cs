using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableGhost : MonoBehaviour {
    // Components
    GameObject bodyGO; // added/changed dynamically.
    // References
    [SerializeField] private Material m_ghost_placeable; // a-ok to place.
    [SerializeField] private Material m_ghost_cantAfford;
    //[SerializeField] private Material m_ghost_;
    // Properties
    public PlaceableType MyType { get; private set; }


    // ----------------------------------------------------------------
    //  Setters
    // ----------------------------------------------------------------
    public void SetMaterial(Material mat) {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.material = mat;
        }
    }

    public void SetMyType(PlaceableType _type) {
        this.MyType = _type;

        // Destroy body.
        if (bodyGO != null) { Destroy(bodyGO); }
        bodyGO = Instantiate(ResourcesHandler.Instance.GetPlaceableBody(MyType, WeatheredState.Good));
        GameUtils.ParentAndReset(bodyGO, this.transform);

        SetMaterial(m_ghost_placeable);
        // Disable all colliders. Ghosts don't push things.
        foreach (Collider col in bodyGO.GetComponentsInChildren<Collider>()) {
            col.enabled = false;
        }
    }
    public void SetCanAfford(bool canAfford) {
        SetMaterial(canAfford ? m_ghost_placeable : m_ghost_cantAfford);
    }



}
