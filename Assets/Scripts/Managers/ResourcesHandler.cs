using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [Header ("Common")]
    [SerializeField] public GameObject ImageLine;
    [SerializeField] public GameObject ImageLinesJoint;

    [Header("RainAndShelter")]
    [SerializeField] public GameObject Bush;
    [SerializeField] public GameObject Campfire;
    [SerializeField] public GameObject Rock;
    [SerializeField] public GameObject Stone;
    [SerializeField] public GameObject Stick;
    [SerializeField] public GameObject SimpleBuildingBlock;
    [SerializeField] public GameObject TrashPiece;
    [SerializeField] public GameObject Tree;
    [SerializeField] private GameObject[] bushBodies; // the sub-prefabs.
    [SerializeField] private GameObject[] trashPieceBodies; // the sub-prefabs.
    // Placeables
    [SerializeField] private GameObject campfireBody;
    [SerializeField] private GameObject[] stickHutBodies;
    [SerializeField] private GameObject stickPillarBody;
    [SerializeField] private GameObject stickRoofBody;



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
    public GameObject GetTrashPieceBody(TrashPieceType type) {
        //return bushBodies[type];
        switch (type) {
            case TrashPieceType.SodaCan: return trashPieceBodies[0];
            case TrashPieceType.SodaCanCrushed: return trashPieceBodies[1];
            case TrashPieceType.TinCan: return trashPieceBodies[2];
            case TrashPieceType.TinCanCrushedSlightly: return trashPieceBodies[3];
            default: Debug.LogError("Oops, no body in ResourcesHandler for this TrashPieceType: " + type); return trashPieceBodies[0];
        }
    }
    public GameObject GetPlaceableBody(PlaceableType type, WeatheredState state) {
        switch (type) {
            case PlaceableType.Campfire: return campfireBody;
            case PlaceableType.StickHut:
                switch (state) {
                    case WeatheredState.Good: return stickHutBodies[0];
                    case WeatheredState.Leaky: return stickHutBodies[1];
                    default: return stickHutBodies[2];
                }
            case PlaceableType.StickPillar: return stickPillarBody;
            case PlaceableType.StickRoof: return stickRoofBody;
            default: Debug.LogError("Oops, no PlaceableBody in ResourcesHandler for this PlaceableType: " + type); return stickPillarBody;
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
