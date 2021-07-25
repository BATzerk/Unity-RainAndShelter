using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [Header ("Common")]
    [SerializeField] public GameObject ImageLine;
    [SerializeField] public GameObject ImageLinesJoint;

    [Header("RainAndShelter")]
    [SerializeField] public GameObject Placeable;
    [SerializeField] public GameObject Bush;
    [SerializeField] public GameObject Tree;
    [SerializeField] public GameObject Rock;
    [SerializeField] public GameObject Stone;
    [SerializeField] public GameObject Stick;
    [SerializeField] private GameObject[] bushBodies; // the sub-prefabs.
    [SerializeField] private GameObject pb_simpleHut;
    [SerializeField] private GameObject pb_stickPillar;
    [SerializeField] private GameObject pb_stickRoof;



    public GameObject GetBushBody(BushType type) {
        //return bushBodies[type];
        switch (type) {
            case BushType.Type0: return bushBodies[0];
            case BushType.Type1: return bushBodies[1];
            case BushType.Type2: return bushBodies[2];
            case BushType.Type3: return bushBodies[3];
            case BushType.Type4: return bushBodies[4];
            default: Debug.LogError("Oops, no bush body in ResourcesHandler for this BushType: " + type); return bushBodies[0];
        }
    }
    public GameObject GetPlaceableBody(PlaceableType type) {
        switch (type) {
            case PlaceableType.SimpleHut: return pb_simpleHut;
            case PlaceableType.StickPillar: return pb_stickPillar;
            case PlaceableType.StickRoof: return pb_stickRoof;
            default: Debug.LogError("Oops, no PlaceableBody in ResourcesHandler for this PlaceableType: " + type); return pb_simpleHut;
        }
    }
    




    // Instance
    static public ResourcesHandler Instance { get; private set; }


    // ----------------------------------------------------------------
    //  Awake
    // ----------------------------------------------------------------
    private void Awake () {
        // There can only be one (instance)!
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy (this);
        }
	}
    
}
