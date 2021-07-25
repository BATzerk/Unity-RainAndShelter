using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FieldPropsData {
    // Properties
    public List<PlaceableData> placeableDatas = new List<PlaceableData>();
    public List<BushData> bushDatas = new List<BushData>();
    public List<TreeData> treeDatas = new List<TreeData>();
    public List<RockData> rockDatas = new List<RockData>();
    public List<StoneData> stoneDatas = new List<StoneData>();
    public List<StickData> stickDatas = new List<StickData>();

    // Creation
    public void PopulateFromWorld(Transform containerTF) {
        placeableDatas = new List<PlaceableData>();
        bushDatas = new List<BushData>();
        treeDatas = new List<TreeData>();
        stickDatas = new List<StickData>();

        foreach (Placeable obj in containerTF.GetComponentsInChildren<Placeable>()) {
            placeableDatas.Add(new PlaceableData(obj));
        }
        foreach (Bush obj in containerTF.GetComponentsInChildren<Bush>()) {
            bushDatas.Add(new BushData(obj));
        }
        foreach (Tree obj in containerTF.GetComponentsInChildren<Tree>()) {
            treeDatas.Add(new TreeData(obj));
        }
        foreach (Rock obj in containerTF.GetComponentsInChildren<Rock>()) {
            rockDatas.Add(new RockData(obj));
        }
        foreach (Stone obj in containerTF.GetComponentsInChildren<Stone>()) {
            stoneDatas.Add(new StoneData(obj));
        }
        foreach (Stick obj in containerTF.GetComponentsInChildren<Stick>()) {
            stickDatas.Add(new StickData(obj));
        }
    }
}


public class PropData {
    public Vector3 pos;
    public Vector3 rot; // euler angles
    public Vector3 scale;
}



[Serializable]
public class PlaceableData : PropData {
    public PlaceableType myType;
    public PlaceableData(Placeable myProp) {
        myType = myProp.MyType;
        pos = myProp.transform.position;
        rot = myProp.transform.eulerAngles;
        scale = myProp.transform.lossyScale;
    }
}
[Serializable]
public class BushData : PropData {
    public BushType bushType;
    public BushData(Bush myProp) {
        bushType = myProp.MyType;
        pos = myProp.transform.position;
        rot = myProp.transform.eulerAngles;
        scale = myProp.transform.lossyScale;
    }
}
[Serializable]
public class RockData : PropData {
    public RockType myType;
    public RockData(Rock myProp) {
        myType = myProp.MyType;
        pos = myProp.transform.position;
        rot = myProp.transform.eulerAngles;
        scale = myProp.transform.lossyScale;
    }
}
[Serializable]
public class StoneData : PropData {
    public StoneData(Stone myProp) {
        pos = myProp.transform.position;
        rot = myProp.transform.eulerAngles;
    }
}
[Serializable]
public class StickData : PropData {
    public StickData(Stick myStick) {
        pos = myStick.transform.position;
        rot = myStick.transform.eulerAngles;
    }
}
[Serializable]
public class TreeData : PropData {
    public TreeType treeType;
    public TreeData(Tree myProp) {
        treeType = myProp.MyType;
        pos = myProp.transform.position;
        rot = myProp.transform.eulerAngles;
        scale = myProp.transform.lossyScale;
    }
}




