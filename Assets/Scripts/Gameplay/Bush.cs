using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BushType : int {
    Undefined,
    Stick, String, Leaf
}

public class Bush : MonoBehaviour, IClickable {
    // Statics
    public static BushType GetRandomType() { return (BushType) Random.Range(1, 3); }

    // Components
    [SerializeField] MeshRenderer mr_bodyHighlight;
    [SerializeField] GameObject myBodyGO; // this is created dynamically!
    [SerializeField] GameObject[] resourceBodyGOs; // e.g. the stick bodies stickin' out of me.
    // Properties
    [SerializeField] private BushType myType;
    [SerializeField] private int numResourcesLeft;
    //private int numTimesClicked = 0;

    // Getters
    public BushType MyType { get { return myType; } }
    public int NumResourcesLeft { get { return numResourcesLeft; } }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, BushData data) {
        Initialize(parent, data.pos,data.rot,data.scale, data.bushType, data.numResourcesLeft);
    }
    public void Initialize(Transform parent, Vector3 pos,Vector3 rot,Vector3 scale, BushType bushType, int numResourcesLeft) {
        // Transform
        this.myType = bushType;
        this.numResourcesLeft = numResourcesLeft;
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;
        // Set my visuals based on my type!
        // First, delete the placeholder visual.
        //Destroy(myBodyGO);
        //// Now, add the correct one!
        //myBodyGO = Instantiate(ResourcesHandler.Instance.GetBushBody(myType));
        //GameUtils.ParentAndReset(myBodyGO, transform);

        // Setup basics.
        //numTimesClicked = 0;
        mr_bodyHighlight.enabled = false;

        CullResourceBodyGOs();
    }


    void CullResourceBodyGOs() {
        // Remove any resource body GOs that shouldn't be here, based on how many I have left.
        for (int i=numResourcesLeft; i<resourceBodyGOs.Length; i++) {
            if (resourceBodyGOs[i] != null) {
                Destroy(resourceBodyGOs[i]);
                resourceBodyGOs[i] = null;
            }
        }
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public bool IsClickable(Tool tool) {
        if (numResourcesLeft > 0) return true;
        if (tool!=null && tool.MyType==ToolType.Shovel) return true;
        return false;
    }
    public CursorType CurrCursorForMe(Tool tool) {
        if (numResourcesLeft > 0) return CursorType.Punch;
        if (tool.MyType==ToolType.Shovel) return CursorType.Dig;
        return CursorType.Undefined;
    }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        // We've got resources??
        if (numResourcesLeft > 0) {
            // Spawn a stick!
            Vector3 _pos = transform.position + new Vector3(0, 1, 0);// Random.Range(0.5f, 5f), 0);
            Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Stick newStick = GameObject.Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
            Rigidbody rigidbody = newStick.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));
            rigidbody.velocity = new Vector3(Random.Range(-1.2f, 1.2f), 4, Random.Range(-1.2f, 1.2f));
            newStick.Initialize(transform.parent, _pos, _rot);

            // Decrement how many resources I have left!
            numResourcesLeft--;

            // Update how many resources I got on my body!
            CullResourceBodyGOs();

            // Shake it, baby!
            iTween.Stop();
            myBodyGO.transform.localEulerAngles = Vector3.zero;
            Invoke("ShakeBodyTween", 0.01f); // hacky?
        }

        // We DON'T have resources, AND it's a shovel!..
        else if (player.Hand.currTool.MyType == ToolType.Shovel) {
            // Damage the tool used.
            player.Hand.DamageCurrTool(0.1f);
            // Simply destroy me for now ;)
            Destroy(this.gameObject);
        }
    }
    private void ShakeBodyTween() {
        iTween.PunchRotation(myBodyGO, new Vector3(0, 60, 0), 1.5f);// gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
    }

    public void OnHoverOver() {
        mr_bodyHighlight.enabled = true;
    }
    public void OnHoverOut() {
        if (mr_bodyHighlight == null) { return; } // Safety check.
        mr_bodyHighlight.enabled = false;
    }


}
