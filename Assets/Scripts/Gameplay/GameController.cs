using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // References
    [SerializeField] Player player;
    [SerializeField] GameObject defaultFieldPropsGO;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }



    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    void Start() {
        ReloadFromSave();
    }
    private void DestroyDefaultFieldProps() {
        Destroy(defaultFieldPropsGO);
    }



    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    private void ReloadFromSave() {
        // Player pos/rot
        Vector3 _pos = SaveStorage.GetVector3(SaveKeys.PlayerPos, player.GetPos()); // default to the player's current (aka default, fresh-save) pos
        player.SetPos(new Vector3(_pos.x, _pos.y, _pos.z));
        float rotY = SaveStorage.GetFloat(SaveKeys.PlayerRotY);
        player.SetRotY(rotY);
        // Player inventory
        dm.LoadPlayerInventory();
        // FieldPropsData
        {
            string saveKey = SaveKeys.FieldPropsData;
            // We DO have the save!
            if (SaveStorage.HasKey(saveKey)) {
                DestroyDefaultFieldProps(); // Destroy the default stuff out there.
                string saveStr = SaveStorage.GetString(saveKey);
                FieldPropsData fieldPropsData = JsonUtility.FromJson<FieldPropsData>(saveStr);
                foreach (BushData data in fieldPropsData.bushDatas) {
                    Bush newObj = Instantiate(ResourcesHandler.Instance.Bush).GetComponent<Bush>();
                    newObj.Initialize(data);
                }
                foreach (TreeData data in fieldPropsData.treeDatas) {
                    Tree newObj = Instantiate(ResourcesHandler.Instance.Tree).GetComponent<Tree>();
                    newObj.Initialize(data);
                }
                foreach (StickData data in fieldPropsData.stickDatas) {
                    Stick newObj = Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
                    newObj.Initialize(data);
                }
            }
        }
    }
    private void SaveGameState() {
        // Player and Inventory
        SaveStorage.SetVector3(SaveKeys.PlayerPos, player.GetPos());
        SaveStorage.SetFloat(SaveKeys.PlayerRotY, player.GetRotY());
        dm.SavePlayerInventory();
        // FieldPropsData
        FieldPropsData fieldPropsData = new FieldPropsData();
        fieldPropsData.PopulateFromWorld();
        SaveStorage.SetString(SaveKeys.FieldPropsData, JsonUtility.ToJson(fieldPropsData));

        Debug.Log("Saved game state.");
    }




    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        RegisterButtonInput();
    }

    private void RegisterButtonInput() {
        // SHIFT + ____
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetKeyDown(KeyCode.S)) {
                SaveGameState();
            }
        }


        // DEBUG
        if (Input.GetKeyDown(KeyCode.T)) {
            dm.PlayerInventory.CollectStick();
        }


        // Reload scene
        if (Input.GetKey(KeyCode.Return)) {
            SceneHelper.ReloadScene();
        }

        // DELETE ALL SAVE DATA
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Delete)) {
            dm.DeleteAllSaveData();
            SceneHelper.ReloadScene();
        }
    }

}
