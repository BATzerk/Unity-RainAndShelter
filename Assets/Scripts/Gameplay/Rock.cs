using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RockType : int {
    Undefined,
    Type0, Type1, Type2//, Type3, Type4
}

public class Rock : MonoBehaviour, IClickable {
    // Statics
    public static RockType GetRandomType() { return (RockType) Random.Range(1, 3); }

    // Components
    [SerializeField] MeshRenderer mr_bodyHighlight;
    [SerializeField] GameObject myBodyGO; // this is created dynamically!
    // Properties
    [SerializeField] private RockType myType; // NOTE: UNUSED!
    private int numTimesClicked = 0;

    // Getters
    public bool IsClickable() { return true; }
    public RockType MyType { get { return myType; } }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, RockData data) {
        Initialize(parent, data.pos,data.rot,data.scale, data.myType);
    }
    public void Initialize(Transform parent, Vector3 pos,Vector3 rot,Vector3 scale, RockType _type) {
        // Transform
        this.myType = _type;
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;
        // Set my visuals based on my type!
        // First, delete the placeholder visual.
        //Destroy(myBodyGO);
        // Now, add the correct one!
        //myBodyGO = Instantiate(ResourcesHandler.Instance.GetBushBody(myType));TODO: This. Prob do a different mesh instead.

        // Setup basics.
        numTimesClicked = 0;
        mr_bodyHighlight.enabled = false;
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public CursorType CurrCursorForMe() { return CursorType.Punch; }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        // Shake it, baby!
        iTween.Stop();
        Invoke("ShakeBodyTween", 0.01f); // hacky?

        numTimesClicked++;
        if (numTimesClicked >= 3) {
            ReleaseStonesAndDestroySelf();
        }
    }


    private void ReleaseStonesAndDestroySelf() {
        int numToRelease = Random.Range(3, 4);
        for (int i=0; i<numToRelease; i++) {
            Vector3 _pos = transform.position + new Vector3(0, 1, 0);// Random.Range(0.5f, 5f), 0);
            Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Stone newObj = Instantiate(ResourcesHandler.Instance.Stone).GetComponent<Stone>();
            Rigidbody rigidbody = newObj.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));
            rigidbody.velocity = new Vector3(Random.Range(-1, 1), 2, Random.Range(-1, 1));
            newObj.Initialize(transform.parent, _pos, _rot);
        }
        // I'm toast.
        Destroy(gameObject);
    }
    private void ShakeBodyTween() {
        iTween.PunchRotation(myBodyGO, new Vector3(0, 20, 0), 0.7f);// gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
    }

    public void OnHoverOver() {
        mr_bodyHighlight.enabled = true;
    }
    public void OnHoverOut() {
        if (mr_bodyHighlight == null) { return; } // Safety check.
        mr_bodyHighlight.enabled = false;
    }



}
