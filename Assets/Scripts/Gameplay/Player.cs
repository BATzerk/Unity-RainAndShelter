using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Components
    [SerializeField] private GameObject myCamGO;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.FirstPersonController myFPC;
    [SerializeField] private CharacterController charController;

    // Getters / Setters
    public Vector3 GetPos() { return gameObject.transform.position; }
    public void SetPos(Vector3 _pos) {
        charController.enabled = false;
        gameObject.transform.position = _pos;
        charController.enabled = true;
    }
    public float GetRotY() { return myFPC.transform.localEulerAngles.y; }
    public void SetRotY(float val) {
        myFPC.MouseLook.SetRotY(val);
    }




}
