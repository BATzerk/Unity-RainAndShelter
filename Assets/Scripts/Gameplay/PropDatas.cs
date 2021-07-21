using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FieldPropsData {
    // Properties
    public List<BushData> bushDatas = new List<BushData>();
    public List<TreeData> treeDatas = new List<TreeData>();
    public List<StickData> stickDatas=new List<StickData>();

    // Creation
    public void PopulateFromWorld() {
        bushDatas = new List<BushData>();
        treeDatas = new List<TreeData>();
        stickDatas = new List<StickData>();

        foreach (Bush obj in GameObject.FindObjectsOfType<Bush>()) {
            bushDatas.Add(new BushData(obj));
        }
        foreach (Tree obj in GameObject.FindObjectsOfType<Tree>()) {
            treeDatas.Add(new TreeData(obj));
        }
        foreach (Stick obj in GameObject.FindObjectsOfType<Stick>()) {
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
public class BushData : PropData {
    public BushType bushType;
    public BushData(Bush myProp) {
        bushType = myProp.MyType;
        pos = myProp.gameObject.transform.position;
        rot = myProp.gameObject.transform.eulerAngles;
        scale = myProp.gameObject.transform.lossyScale;
    }
}
[Serializable]
public class StickData : PropData {
    public StickData(Stick myStick) {
        pos = myStick.gameObject.transform.position;
        rot = myStick.gameObject.transform.eulerAngles;
    }
}
[Serializable]
public class TreeData : PropData {
    public TreeData(Tree myProp) {
        pos = myProp.gameObject.transform.position;
        rot = myProp.gameObject.transform.eulerAngles;
        scale = myProp.gameObject.transform.lossyScale;
    }
}




