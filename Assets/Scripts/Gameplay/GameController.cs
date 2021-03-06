using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Properties
    [SerializeField] private int debug_startingLevelIndex;
    // References
    [SerializeField] Player player;
    [SerializeField] Transform fieldPropsTF;
    [SerializeField] WeatherController weatherController;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }
    public Player Player { get { return player; } }
    public Transform FieldPropsTF { get { return fieldPropsTF; } }



    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    void Start() {
        ReloadFromSave(debug_startingLevelIndex);
    }
    private void DestroyFieldProps() {
        for (int i=fieldPropsTF.childCount-1; i>=0; i--) {
            Destroy(fieldPropsTF.GetChild(i));
        }
    }



    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    private void ReloadFromSave(int _currLevelIndex) {
        SaveKeys.SetCurrLevelIndex(_currLevelIndex);

        // Player
        player.InitializeFromSave();
        // WeatherController
        weatherController.LoadValuesFromStorage();
        // FieldPropsData
        {
            string saveKey = SaveKeys.FieldPropsData();
            // We DO have the save!
            if (SaveStorage.HasKey(saveKey)) {
                DestroyFieldProps(); // Destroy the default stuff out there.
                string saveStr = SaveStorage.GetString(saveKey);
                FieldPropsData fieldPropsData = JsonUtility.FromJson<FieldPropsData>(saveStr);

                foreach (SimpleBuildingBlockData data in fieldPropsData.simpleBuildingBlockDatas) {
                    SimpleBuildingBlock newObj = Instantiate(ResourcesHandler.Instance.SimpleBuildingBlock).GetComponent<SimpleBuildingBlock>();
                    newObj.Initialize(this, data);
                }
                foreach (BushData data in fieldPropsData.bushDatas) {
                    Bush newObj = Instantiate(ResourcesHandler.Instance.Bush).GetComponent<Bush>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (CampfireData data in fieldPropsData.campfireDatas) {
                    Campfire newObj = Instantiate(ResourcesHandler.Instance.Campfire).GetComponent<Campfire>();
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
                foreach (TreeData data in fieldPropsData.treeDatas) {
                    Tree newObj = Instantiate(ResourcesHandler.Instance.Tree).GetComponent<Tree>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (TreeStumpData data in fieldPropsData.treeStumpDatas) {
                    TreeStump newObj = Instantiate(ResourcesHandler.Instance.TreeStump).GetComponent<TreeStump>();
                    newObj.Initialize(fieldPropsTF, data);
                }
                foreach (TrashPieceData data in fieldPropsData.trashPieceDatas) {
                    TrashPiece newObj = Instantiate(ResourcesHandler.Instance.TrashPiece).GetComponent<TrashPiece>();
                    newObj.Initialize(fieldPropsTF, data);
                }
            }
            // We DON'T have a save.
            else {
                int lm_terrain = 1 << LayerMask.NameToLayer("Terrain");
                RaycastHit hit;
                Rect tb = new Rect(20,30, 330,330); // terrain bounds.

                // Randomly add BUSHES.
                for (int i = 0; i < 1000; i++) {
                    float x = Random.Range(tb.xMin, tb.xMax);
                    float z = Random.Range(tb.yMin, tb.yMax);
                    if (Physics.Raycast(new Vector3(x, 1000, z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add one at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0, 360), 0);
                        Vector3 scale = Vector3.one;
                        Bush obj = Instantiate(ResourcesHandler.Instance.Bush).GetComponent<Bush>();
                        obj.Initialize(fieldPropsTF, pos, rot, scale, Bush.GetRandomType(), 3);
                    }
                }
                // Randomly add ROCKS.
                for (int i = 0; i < 500; i++) {
                    float x = Random.Range(tb.xMin, tb.xMax);
                    float z = Random.Range(tb.yMin, tb.yMax);
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
                for (int i=0; i<12; i++) {
                    float x = Random.Range(tb.xMin, tb.xMax);
                    float z = Random.Range(tb.yMin, tb.yMax);
                    if (Physics.Raycast(new Vector3(x,1000,z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add one at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0,360), 0);
                        Vector3 scale = Vector3.one * Random.Range(0.8f, 1.5f);
                        Tree obj = Instantiate(ResourcesHandler.Instance.Tree).GetComponent<Tree>();
                        obj.Initialize(fieldPropsTF, pos, rot, scale, Tree.GetRandomType(), 0);
                    }
                }
                // Randomly add TRASH PIECES.
                for (int i = 0; i < 1000; i++) {
                    float x = Random.Range(tb.xMin, tb.xMax);
                    float z = Random.Range(tb.yMin, tb.yMax);
                    if (Physics.Raycast(new Vector3(x, 1000, z), Vector3.down, out hit, 9999, lm_terrain)) {
                        // Add one at the hit pos!
                        Vector3 pos = hit.point;
                        Vector3 rot = new Vector3(0, Random.Range(0, 360), 0);
                        Vector3 scale = Vector3.one;
                        TrashPiece obj = Instantiate(ResourcesHandler.Instance.TrashPiece).GetComponent<TrashPiece>();
                        obj.Initialize(fieldPropsTF, pos, rot, scale, TrashPiece.GetRandomType());
                    }
                }
            }
        }
    }
    private void SaveGameState() {
        // Player
        player.SavePropertiesToStorage();
        // WeatherController
        weatherController.SaveValuesToStorage();
        // FieldPropsData
        FieldPropsData fieldPropsData = new FieldPropsData();
        fieldPropsData.PopulateFromWorld(fieldPropsTF);
        SaveStorage.SetString(SaveKeys.FieldPropsData(), JsonUtility.ToJson(fieldPropsData));

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
        if (Input.GetKeyDown(KeyCode.Tab)) { player.IsDebugWarpSpeed = !player.IsDebugWarpSpeed; }
        if (Input.GetKeyDown(KeyCode.T)) { WeatherController.WorldTime -= 10; }
        if (Input.GetKeyDown(KeyCode.Y)) { WeatherController.WorldTime += 10; }
        if (Input.GetKeyDown(KeyCode.U)) { player.Inventory.ChangeSticks(5); }
        if (Input.GetKeyDown(KeyCode.I)) { player.Inventory.ChangeStones(5); }
        if (Input.GetKeyDown(KeyCode.X)) { player.ToolBelt.TryToCraftTool(ToolType.Axe); }//.tools[0] = new Tool(ToolType.Axe, 1); }
        if (Input.GetKeyDown(KeyCode.V)) { player.ToolBelt.TryToCraftTool(ToolType.Shovel); }//tools[1] = new Tool(ToolType.Shovel, 1); }


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
