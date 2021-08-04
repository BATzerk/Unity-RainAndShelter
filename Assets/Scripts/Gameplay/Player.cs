using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Components
    [SerializeField] private GameObject myCamGO;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.FirstPersonController myFPC;
    [SerializeField] private CharacterController charController;
    public PlayerHand Hand;
    // Properties
    public PlayerInventory Inventory { get; private set; } // saved/loaded.
    public PlayerToolBelt ToolBelt { get; private set; } // saved/loaded.
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
    //  Load / Save
    // ----------------------------------------------------------------
    public void InitializeFromSave() {
        // Transform
        {
            Vector3 _pos = SaveStorage.GetVector3(SaveKeys.PlayerPos(), GetPos()); // default to the player's current (aka default, fresh-save) pos
            float rotY = SaveStorage.GetFloat(SaveKeys.PlayerRotY());
            SetPos(new Vector3(_pos.x, _pos.y, _pos.z));
            SetRotY(rotY);
        }
        // Inventory
        {
            string saveKey = SaveKeys.PlayerInventory();
            // We HAVE save for this! Load it!
            if (SaveStorage.HasKey(saveKey)) {
                string jsonString = SaveStorage.GetString(saveKey);
                Inventory = JsonUtility.FromJson<PlayerInventory>(jsonString);
            }
            // We DON'T have a save for this. Make a new UserData.
            else {
                Inventory = new PlayerInventory();
            }
        }
        // ToolBelt
        {
            string saveKey = SaveKeys.PlayerToolBelt();
            // We HAVE save for this! Load it!
            if (SaveStorage.HasKey(saveKey)) {
                string jsonString = SaveStorage.GetString(saveKey);
                ToolBelt = JsonUtility.FromJson<PlayerToolBelt>(jsonString);
            }
            // We DON'T have a save for this. Make a new UserData.
            else {
                ToolBelt = new PlayerToolBelt();
            }
        }
        // Initialize Hand.
        Hand.Initialize();
        // Dispatch events so UIs update!
        EventBus.Instance.OnPlayerInventoryChanged();
        EventBus.Instance.OnPlayerToolBeltChanged();
    }

    public void SavePropertiesToStorage() {
        // Transform
        {
            SaveStorage.SetVector3(SaveKeys.PlayerPos(), GetPos());
            SaveStorage.SetFloat(SaveKeys.PlayerRotY(), GetRotY());
        }
        // ToolBelt and Inventory
        {
            SaveStorage.SetString(SaveKeys.PlayerToolBelt(), JsonUtility.ToJson(ToolBelt));
            SaveStorage.SetString(SaveKeys.PlayerInventory(), JsonUtility.ToJson(Inventory));
        }
    }




    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        //bool pIsInRain = IsInRain;

        // Update IsUnderShelter!
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 9999)) {
            GameObject go = hit.transform.gameObject;
            IsInRain = go.tag == Tags.RainingSky;
            IsUnderShelter = !IsInRain && hit.distance < 100; // HARDCODED max shelter distance.
        }
        else {
            IsInRain = false;
            IsUnderShelter = false;

        }

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
