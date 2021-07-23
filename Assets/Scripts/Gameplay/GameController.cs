﻿using System.Collections;
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
            // We DON'T have a save.
            else {
                int lm_terrain = 1 << LayerMask.NameToLayer("Terrain");
                RaycastHit hit;
                
                // Randomly add bushes.
                for (int i=0; i<100; i++) {
                    float x = Random.Range(20, 250);
                    float z = Random.Range(30, 200);
                    if (Physics.Raycast(new Vector3(x,1000,z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add a bush at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0,360), 0);
                        Vector3 scale = Vector3.one;
                        Bush obj = Instantiate(ResourcesHandler.Instance.Bush).GetComponent<Bush>();
                        obj.Initialize(pos, rot, scale, Bush.GetRandomType());
                    }
                }
                // Randomly add trees.
                for (int i=0; i<50; i++) {
                    float x = Random.Range(20, 250);
                    float z = Random.Range(30, 200);
                    if (Physics.Raycast(new Vector3(x,1000,z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add a bush at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0,360), 0);
                        Vector3 scale = Vector3.one * Random.Range(0.4f, 1.2f);
                        Tree obj = Instantiate(ResourcesHandler.Instance.Tree).GetComponent<Tree>();
                        obj.Initialize(pos, rot, scale, Tree.GetRandomType());
                    }
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
            //dm.PlayerInventory.CollectStick();
            player.SetRotY(90);//QQQ
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
