using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Components
    [SerializeField] private GameObject myCamGO;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.FirstPersonController myFPC;
    [SerializeField] private CharacterController charController;
    // Properties
    private RaycastHit hit;
    public bool IsUnderShelter { get; private set; }

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




    // ----------------------------------------------------------------
    //  FixedUpdate
    // ----------------------------------------------------------------
    private void FixedUpdate() {
        // Update IsInRain!
        IsUnderShelter = Physics.Raycast(transform.position, Vector3.up, out hit, 9999);
    }


}
