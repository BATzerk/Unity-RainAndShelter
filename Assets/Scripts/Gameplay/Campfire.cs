using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IClickable {
    //// Components
    //[SerializeField] MeshRenderer bodyMesh;
    [SerializeField] private Light pointLight;
    // Properties
    public bool IsLit { get; private set; }
    private RaycastHit hit;
    // References
    //[SerializeField] Material m_bodyNormal;
    //[SerializeField] Material m_bodyHovered;



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



    private void Update() {
        // Flicker light!
        if (IsLit) {
            pointLight.intensity = Random.Range(2.5f, 3.25f);
        }

        // Raycast up occasionally to see if we should be put out.
        if (IsLit) {
            if (Time.frameCount % 10 == 0) {
                if (Physics.Raycast(transform.position + new Vector3(0,1f,0), Vector3.up, out hit, 2000)) { // HARDCODED starting the ray ABOVE the campfire. So we don't collide with it. :P
                    GameObject go = hit.transform.gameObject;
                    if (go.tag == Tags.RainingSky) {
                        SetIsLit(false);
                    }
                }
            }
        }
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public bool IsClickable(Tool tool) { return true; }
    public CursorType CurrCursorForMe(Tool tool) { return CursorType.Circle; }
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
