using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // References
    [SerializeField] Player player;
    [SerializeField] Transform fieldPropsTF;
    [SerializeField] WeatherController weatherController;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }
    public Transform FieldPropsTF { get { return fieldPropsTF; } }



    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    void Start() {
        ReloadFromSave();
    }
    private void DestroyFieldProps() {
        for (int i=fieldPropsTF.childCount-1; i>=0; i--) {
            Destroy(fieldPropsTF.GetChild(i));
        }
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
        // WeatherController
        weatherController.LoadWeatherValues();
        // FieldPropsData
        {
            string saveKey = SaveKeys.FieldPropsData;
            // We DO have the save!
            if (SaveStorage.HasKey(saveKey)) {
                DestroyFieldProps(); // Destroy the default stuff out there.
                string saveStr = SaveStorage.GetString(saveKey);
                FieldPropsData fieldPropsData = JsonUtility.FromJson<FieldPropsData>(saveStr);

                foreach (PlaceableData data in fieldPropsData.placeableDatas) {
                    Placeable newObj = Instantiate(ResourcesHandler.Instance.Placeable).GetComponent<Placeable>();
                    newObj.Initialize(this, data);
                }
                foreach (BushData data in fieldPropsData.bushDatas) {
                    Bush newObj = Instantiate(ResourcesHandler.Instance.Bush).GetComponent<Bush>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (TreeData data in fieldPropsData.treeDatas) {
                    Tree newObj = Instantiate(ResourcesHandler.Instance.Tree).GetComponent<Tree>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (RockData data in fieldPropsData.rockDatas) {
                    Rock newObj = Instantiate(ResourcesHandler.Instance.Rock).GetComponent<Rock>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (StoneData data in fieldPropsData.stoneDatas) {
                    Stone newObj = Instantiate(ResourcesHandler.Instance.Stone).GetComponent<Stone>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (StickData data in fieldPropsData.stickDatas) {
                    Stick newObj = Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
                    newObj.Initialize(fieldPropsTF, data);
                }
            }
            // We DON'T have a save.
            else {
                int lm_terrain = 1 << LayerMask.NameToLayer("Terrain");
                RaycastHit hit;

                // Randomly add BUSHES.
                for (int i = 0; i < 100; i++) {
                    float x = Random.Range(20, 250);
                    float z = Random.Range(30, 200);
                    if (Physics.Raycast(new Vector3(x, 1000, z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add one at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0, 360), 0);
                        Vector3 scale = Vector3.one;
                        Bush obj = Instantiate(ResourcesHandler.Instance.Bush).GetComponent<Bush>();
                        obj.Initialize(fieldPropsTF, pos, rot, scale, Bush.GetRandomType());
                    }
                }
                // Randomly add ROCKS.
                for (int i = 0; i < 100; i++) {
                    float x = Random.Range(20, 250);
                    float z = Random.Range(30, 200);
                    if (Physics.Raycast(new Vector3(x, 1000, z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add one at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = hit.normal * 90;// + new Vector3(0, Random.Range(0, 360), 0);
                        Vector3 scale = Vector3.one;
                        Rock obj = Instantiate(ResourcesHandler.Instance.Rock).GetComponent<Rock>();
                        obj.Initialize(fieldPropsTF, pos, rot, scale, Rock.GetRandomType());
                    }
                }
                // Randomly add TREES.
                for (int i=0; i<50; i++) {
                    float x = Random.Range(20, 250);
                    float z = Random.Range(30, 200);
                    if (Physics.Raycast(new Vector3(x,1000,z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add one at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0,360), 0);
                        Vector3 scale = Vector3.one * Random.Range(0.4f, 1.2f);
                        Tree obj = Instantiate(ResourcesHandler.Instance.Tree).GetComponent<Tree>();
                        obj.Initialize(fieldPropsTF, pos, rot, scale, Tree.GetRandomType());
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
        // WeatherController
        weatherController.SaveWeatherValues();
        // FieldPropsData
        FieldPropsData fieldPropsData = new FieldPropsData();
        fieldPropsData.PopulateFromWorld(fieldPropsTF);
        SaveStorage.SetString(SaveKeys.FieldPropsData, JsonUtility.ToJson(fieldPropsData));

        Debug.Log("Saved game state.");
    }



    //public void SpawnResources(




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
        if (Input.GetKeyDown(KeyCode.T)) { weatherController.CurrTime -= 10; }
        if (Input.GetKeyDown(KeyCode.Y)) { weatherController.CurrTime += 10; }
        if (Input.GetKeyDown(KeyCode.U)) { dm.PlayerInventory.ChangeSticks(5); }
        if (Input.GetKeyDown(KeyCode.I)) { dm.PlayerInventory.ChangeStones(5); }


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
