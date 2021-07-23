using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlaceableType {
    Undefined,
    SimpleHut,
}


public class Placeable : MonoBehaviour
{
    // Properties
    public PlaceableType MyType { get; private set; }
    // References
    [SerializeField] private Material m_grayDark;
    [SerializeField] private Material m_simpleHut;


    // ----------------------------------------------------------------
    //  Setters
    // ----------------------------------------------------------------
    public void SetMaterial(Material mat) {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.material = mat;
        }
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, PlaceableData data) {
        Initialize(parent, data.pos,data.rot,data.scale, data.myType);
    }
    public void Initialize(Transform parent, Transform tf, PlaceableType _myType) {
        Initialize(parent, tf.position, tf.eulerAngles, tf.localScale, _myType);
    }
    public void Initialize(Transform parent, Vector3 pos, Vector3 rot, Vector3 scale, PlaceableType _myType) {
        this.MyType = _myType;
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;

        SetMaterial(m_simpleHut);
    }


}
