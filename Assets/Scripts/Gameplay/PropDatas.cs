using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FieldPropsData {
    // Properties
    public List<SimpleBuildingBlockData> simpleBuildingBlockDatas = new List<SimpleBuildingBlockData>();
    public List<BushData> bushDatas = new List<BushData>();
    public List<CampfireData> campfireDatas = new List<CampfireData>();
    public List<RockData> rockDatas = new List<RockData>();
    public List<StoneData> stoneDatas = new List<StoneData>();
    public List<StickData> stickDatas = new List<StickData>();
    public List<TrashPieceData> trashPieceDatas = new List<TrashPieceData>();
    public List<TreeData> treeDatas = new List<TreeData>();

    // Creation
    public void PopulateFromWorld(Transform containerTF) {
        simpleBuildingBlockDatas = new List<SimpleBuildingBlockData>();
        bushDatas = new List<BushData>();
        campfireDatas = new List<CampfireData>();
        treeDatas = new List<TreeData>();
        stickDatas = new List<StickData>();

        foreach (SimpleBuildingBlock obj in containerTF.GetComponentsInChildren<SimpleBuildingBlock>()) {
            simpleBuildingBlockDatas.Add(new SimpleBuildingBlockData(obj));
        }
        foreach (Bush obj in containerTF.GetComponentsInChildren<Bush>()) {
            bushDatas.Add(new BushData(obj));
        }
        foreach (Campfire obj in containerTF.GetComponentsInChildren<Campfire>()) {
            campfireDatas.Add(new CampfireData(obj));
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
        foreach (TrashPiece obj in containerTF.GetComponentsInChildren<TrashPiece>()) {
            trashPieceDatas.Add(new TrashPieceData(obj));
        }
        foreach (Tree obj in containerTF.GetComponentsInChildren<Tree>()) {
            treeDatas.Add(new TreeData(obj));
        }
    }
}


public class PropData {
    public Vector3 pos;
    public Vector3 rot; // euler angles
    public Vector3 scale;
    protected void SetTFValues(GameObject go) {
        pos = go.transform.position;
        rot = go.transform.eulerAngles;
        scale = go.transform.lossyScale;
    }
}



[Serializable]
public class SimpleBuildingBlockData : PropData {
    public float TimeInRain;
    public PlaceableType myType;
    public SimpleBuildingBlockData(SimpleBuildingBlock myProp) {
        SetTFValues(myProp.gameObject);
        myType = myProp.MyType;
        TimeInRain = myProp.TimeInRain;
    }
}
[Serializable]
public class CampfireData : PropData {
    public bool isLit;
    public CampfireData(Campfire myProp) {
        SetTFValues(myProp.gameObject);
        isLit = myProp.IsLit;
    }
}


[Serializable]
public class BushData : PropData {
    public BushType bushType;
    public BushData(Bush myProp) {
        SetTFValues(myProp.gameObject);
        bushType = myProp.MyType;
    }
}
[Serializable]
public class RockData : PropData {
    public RockType myType;
    public RockData(Rock myProp) {
        SetTFValues(myProp.gameObject);
        myType = myProp.MyType;
    }
}
[Serializable]
public class StoneData : PropData {
    public StoneData(Stone myProp) {
        SetTFValues(myProp.gameObject);
    }
}
[Serializable]
public class StickData : PropData {
    public StickData(Stick myProp) {
        SetTFValues(myProp.gameObject);
    }
}
[Serializable]
public class TrashPieceData : PropData {
    public TrashPieceType myType;
    public TrashPieceData(TrashPiece myProp) {
        SetTFValues(myProp.gameObject);
        myType = myProp.MyType;
    }
}
[Serializable]
public class TreeData : PropData {
    public TreeType treeType;
    public TreeData(Tree myProp) {
        SetTFValues(myProp.gameObject);
        treeType = myProp.MyType;
    }
}




