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
    public bool IsInRain { get; private set; }
    public bool IsUnderShelter { get; private set; }
    public bool IsDebugWarpSpeed;
    // References
    [SerializeField] private WeatherController weatherController;

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
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        //bool pIsInRain = IsInRain;

        // Update IsUnderShelter!
        IsUnderShelter = Physics.Raycast(transform.position, Vector3.up, out hit, 9999);
        IsInRain = weatherController.IsRaining && !IsUnderShelter;

        // Update charController speed!
        if (IsDebugWarpSpeed) {
            myFPC.SetWalkRunJumpSpeeds(100, 100, 20);
        }
        else {
            if (IsInRain) myFPC.SetWalkRunJumpSpeeds(3.5f, 4f, 5f);
            else myFPC.SetWalkRunJumpSpeeds(5, 6, 7);
        }

        //if (IsInRain != pIsInRain) {
        //    EventBus.Instance.OnPlayerIsInRainChanged();
        //}
    }


}
