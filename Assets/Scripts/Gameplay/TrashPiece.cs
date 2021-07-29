using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TrashPieceType : int {
    Undefined,
    SodaCan, SodaCanCrushed, TinCan, TinCanCrushedSlightly
}

public class TrashPiece : MonoBehaviour, IClickable {
    // Statics
    public static TrashPieceType GetRandomType() { return (TrashPieceType) Random.Range(1, 4); }

    // Components
    //[SerializeField] GameObject myBodyGO; // this is created dynamically!
    // Properties
    public TrashPieceType MyType { get; private set; }

    // Getters
    public bool IsClickable() { return true; }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, TrashPieceData data) {
        Initialize(parent, data.pos,data.rot,data.scale, data.myType);
    }
    public void Initialize(Transform parent, Vector3 pos,Vector3 rot,Vector3 scale, TrashPieceType _type) {
        // Transform
        this.MyType = _type;
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;
        // Add my body based on my type!
        GameObject myBodyGO = Instantiate(ResourcesHandler.Instance.GetTrashPieceBody(MyType));
        GameUtils.ParentAndReset(myBodyGO, this.transform);
        myBodyGO.transform.localScale = new Vector3(10, 10, 10);
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public CursorType CurrCursorForMe() { return CursorType.Hand; }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        // hacky access to PlayerHand.
        player.GetComponentInChildren<PlayerHand>().PickUpRigidbody(GetComponent<Rigidbody>());
    }



    public void OnHoverOver() {
    }
    public void OnHoverOut() {
    }



}
