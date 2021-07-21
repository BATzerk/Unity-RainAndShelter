using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IClickable {
    // Components
    [SerializeField] MeshRenderer mr_bodyHighlight;
    // Properties
    private int numTimesClicked = 0;

    public bool IsClickable() { return false; }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(TreeData data) {
        gameObject.transform.position = data.pos;
        gameObject.transform.eulerAngles = data.rot;
        gameObject.transform.localScale = data.scale;
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
