using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // References
    [SerializeField] Player player;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }



    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    void Start() {
        ReloadFromSave();
    }



    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    private void ReloadFromSave() {
        Vector3 _pos = SaveStorage.GetVector3(SaveKeys.PlayerPos, Vector3.zero);
        player.SetPos(new Vector3(_pos.x, _pos.y, _pos.z));
        float rotY = SaveStorage.GetFloat(SaveKeys.PlayerRotY);
        player.SetRotY(rotY);
        dm.LoadPlayerInventory();
        Debug.Log("numSticks: " + dm.PlayerInventory.numSticks);
    }
    private void SaveGameState() {
        SaveStorage.SetVector3   (SaveKeys.PlayerPos, player.gameObject.transform.position);
        SaveStorage.SetFloat(SaveKeys.PlayerRotY, player.GetRotY());
        dm.SavePlayerInventory();
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
            player.gameObject.transform.position = new Vector3(20, 100, 50);//QQQ
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
