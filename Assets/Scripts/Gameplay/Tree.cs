using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeType : int {
    Undefined,
    Type0, Type1, Type2
}

public class Tree : MonoBehaviour, IClickable {
    // Statics
    public static TreeType GetRandomType() { return (TreeType)Random.Range(1, 3); }

    // Components
    [SerializeField] GameObject bodyGO;
    [SerializeField] MeshRenderer mr_bodyHighlight;
    // Properties
    [SerializeField] private TreeType myType; // NOTE: UNUSED CURRENTLY.
    private int numTimesChopped=0;

    // Getters
    public int NumTimesChopped { get { return numTimesChopped; } }
    public TreeType MyType { get { return myType; } }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform parent, TreeData data) {
        Initialize(parent, data.pos, data.rot, data.scale, data.treeType, data.numTimesChopped);
    }
    public void Initialize(Transform parent, Vector3 pos, Vector3 rot, Vector3 scale, TreeType treeType, int numTimesChopped) {
        // Transform
        this.myType = treeType;
        this.numTimesChopped = numTimesChopped;
        this.transform.parent = parent;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;

        mr_bodyHighlight.enabled = false;
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public bool IsClickable(Tool tool) { return tool!=null && tool.MyType==ToolType.Axe; }
    public CursorType CurrCursorForMe(Tool tool) { return CursorType.Chop; }
    public void OnRClickMe(Player player) { }
    public void OnLClickMe(Player player) {
        numTimesChopped ++;

        // Damage the tool used.
        player.Hand.DamageCurrTool(0.1f);

        // Shake it, baby!
        iTween.Stop();
        bodyGO.transform.localEulerAngles = Vector3.zero;
        Invoke("ShakeBodyTween", 0.01f); // hacky?

        if (numTimesChopped >= 4) {
            // Spawn some woooood!
            int numToSpawn = Random.Range(4, 9);
            for (int i=0; i<numToSpawn; i++) {
                Vector3 _pos = transform.position + new Vector3(0, Random.Range(0.5f, 5f), 0);
                Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                // TODO: Spawn wood instead!
                Stick newStick = GameObject.Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
                newStick.Initialize(transform.parent, _pos, _rot);
            }
            // Finally, destroy me. I'm toast.
            Destroy(gameObject);
        }
    }
    private void ShakeBodyTween() {
        iTween.PunchRotation(bodyGO, new Vector3(0, 20, 0), 1.1f);// gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
    }

    public void OnHoverOver() {
        mr_bodyHighlight.enabled = true;
    }
    public void OnHoverOut() {
        if (mr_bodyHighlight == null) { return; } // Safety check.
        mr_bodyHighlight.enabled = false;
    }


}
