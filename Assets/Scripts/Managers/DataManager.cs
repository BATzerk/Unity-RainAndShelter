using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/*
[System.Serializable]
public class SerializedTransform {
    public float[] pos = new float[3]; // GLOBAL
    public float[] rot = new float[4]; // GLOBAL
    public float[] scale = new float[3]; // GLOBAL

    public SerializedTransform(Transform tf) {
        pos[0] = tf.position.x;
        pos[1] = tf.position.y;
        pos[2] = tf.position.z;

        rot[0] = tf.rotation.w;
        rot[1] = tf.rotation.x;
        rot[2] = tf.rotation.y;
        rot[3] = tf.rotation.z;

        scale[0] = tf.lossyScale.x;
        scale[1] = tf.lossyScale.y;
        scale[2] = tf.lossyScale.z;
    }

    static public void Deserialize(Transform tf, SerializedTransform stf) {
        tf.position.Set(stf.pos[0], stf.pos[1], stf.pos[2]);
        tf.rotation.Set(stf.rot[0], stf.rot[1], stf.rot[2], stf.rot[3]);
        tf.lossyScale.Set(stf.scale[0], stf.scale[1], stf.scale[2]);
    }
}
*/


public class DataManager {
    // Properties
    public PlayerInventory PlayerInventory { get; private set; } // loaded from saveData!
    

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public DataManager() {
        // Load save data!
        LoadPlayerInventory();
    }
    

    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    public void LoadPlayerInventory() {
        string saveKey = SaveKeys.PlayerInventory;
        // We HAVE save for this! Load it!
        if (SaveStorage.HasKey(saveKey)) {
            string jsonString = SaveStorage.GetString(saveKey);
            PlayerInventory = JsonUtility.FromJson<PlayerInventory>(jsonString);
        }
        // We DON'T have a save for this. Make a new UserData.
        else {
            PlayerInventory = new PlayerInventory();
        }
    }
    public void SavePlayerInventory() {
        string saveKey = SaveKeys.PlayerInventory;
        string jsonString = JsonUtility.ToJson(PlayerInventory);
        SaveStorage.SetString(saveKey, jsonString);
    }
    
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void DeleteAllSaveData() {
        // NOOK IT
        SaveStorage.DeleteAll();
        Debug.Log("All SaveStorage CLEARED!");
        LoadPlayerInventory(); // Reload PlayerInventory!
    }


}


