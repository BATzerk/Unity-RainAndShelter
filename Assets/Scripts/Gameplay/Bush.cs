﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BushType : int {
    Undefined,
    Type0, Type1, Type2, Type3, Type4
}

public class Bush : MonoBehaviour, IClickable {
    // Statics
    public static BushType GetRandomType() { return (BushType) Random.Range(1, 5); }

    // Components
    [SerializeField] MeshRenderer mr_bodyHighlight;
    [SerializeField] GameObject myBodyGO; // this is created dynamically!
    // Properties
    [SerializeField] private BushType myType;
    private int numTimesClicked = 0;

    // Getters
    public bool IsClickable() { return true; }
    public BushType MyType { get { return myType; } }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, BushData data) {
        Initialize(parent, data.pos,data.rot,data.scale, data.bushType);
    }
    public void Initialize(Transform parent, Vector3 pos,Vector3 rot,Vector3 scale, BushType bushType) {
        // Transform
        this.myType = bushType;
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;
        // Set my visuals based on my type!
        // First, delete the placeholder visual.
        Destroy(myBodyGO);
        // Now, add the correct one!
        myBodyGO = Instantiate(ResourcesHandler.Instance.GetBushBody(myType));
        GameUtils.ParentAndReset(myBodyGO, transform);

        // Setup basics.
        numTimesClicked = 0;
        mr_bodyHighlight.enabled = false;
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        // Maybe maybe spawn a stick!
        if (Random.Range(0f, 1f) > 0.5f) {
            Vector3 _pos = transform.position + new Vector3(0, 1, 0);// Random.Range(0.5f, 5f), 0);
            Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Stick newStick = GameObject.Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
            Rigidbody rigidbody = newStick.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));
            rigidbody.velocity = new Vector3(Random.Range(-1.2f, 1.2f), 4, Random.Range(-1.2f, 1.2f));
            newStick.Initialize(transform.parent, _pos, _rot);
        }

        // Shake it, baby!
        iTween.Stop();
        Invoke("ShakeBodyTween", 0.01f); // hacky?

        numTimesClicked++;
        if (numTimesClicked >= 6) {
            // I'm toast.
            Destroy(gameObject);
        }
    }
    private void ShakeBodyTween() {
        iTween.PunchRotation(myBodyGO, new Vector3(0, 60, 0), 1.5f);// gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
    }

    public void OnHoverOver() {
        mr_bodyHighlight.enabled = true;
    }
    public void OnHoverOut() {
        if (mr_bodyHighlight == null) { return; } // Safety check.
        mr_bodyHighlight.enabled = false;
    }


}