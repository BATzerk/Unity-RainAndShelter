using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    // Constants
    private float ReachRange = 4; // how far from the camera before it's too far to touch.
    // Components
    [SerializeField] Camera myCamera;
    [SerializeField] Transform cameraTF;
    // Properties
    private RaycastHit hitInfo;
    // References
    private IClickable clickableOver; // the object we're directly looking at.


    // Update
    void Update() {
        // Update clickableOver!
        IClickable newClickableOver = null;
        if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hitInfo, ReachRange)) {
            GameObject go = hitInfo.transform.gameObject;
            if (go != null) {
                newClickableOver = go.GetComponent<IClickable>();
            }
        }

        // Has it changed??
        if (clickableOver != newClickableOver) {
            if (clickableOver!=null) clickableOver.OnHoverOut();
            if (newClickableOver!=null && newClickableOver.IsClickable()) newClickableOver.OnHoverOver();
            clickableOver = newClickableOver; // update the ref.
        }

        // Click?!?
        if (clickableOver != null && Input.GetMouseButtonDown(0) && clickableOver.IsClickable()) {
            clickableOver.OnClickMe();
        }
    }
}
