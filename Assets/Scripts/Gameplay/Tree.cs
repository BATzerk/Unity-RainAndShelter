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
    [SerializeField] MeshRenderer mr_bodyHighlight;
    // Properties
    [SerializeField] private TreeType myType; // NOTE: UNUSED CURRENTLY.
    private int numTimesClicked = 0;

    // Getters
    public bool IsClickable() { return false; }
    public TreeType MyType { get { return myType; } }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(TreeData data) {
        Initialize(data.pos, data.rot, data.scale, data.treeType);
    }
    public void Initialize(Vector3 pos, Vector3 rot, Vector3 scale, TreeType treeType) {
        // Transform
        myType = treeType;
        gameObject.transform.position = pos;
        gameObject.transform.eulerAngles = rot;
        gameObject.transform.localScale = scale;

        numTimesClicked = 0;
        mr_bodyHighlight.enabled = false;
    }




    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickMe() {
        numTimesClicked++;
        if (numTimesClicked >= 3) {
            // Spawn some sticksss!
            int numSticksToSpawn = Random.Range(4, 9);
            for (int i=0; i<numSticksToSpawn; i++) {
                Vector3 _pos = transform.position + new Vector3(0, Random.Range(0.5f, 5f), 0);
                Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                Stick newStick = GameObject.Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
                newStick.Initialize(_pos, _rot);
            }
            // Finally, destroy me. I'm toast.
            Destroy(gameObject);
        }
    }

    public void OnHoverOver() {
        mr_bodyHighlight.enabled = true;
    }
    public void OnHoverOut() {
        if (mr_bodyHighlight == null) { return; } // Safety check.
        mr_bodyHighlight.enabled = false;
    }


}
