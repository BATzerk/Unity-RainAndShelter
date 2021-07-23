using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    // Constants
    private float ReachRange = 4; // how far from the camera before it's too far to touch.
    private float PlaceForwardDist = 7; // how far from the camera we will place something.
    // Components
    [SerializeField] Camera myCamera;
    [SerializeField] Transform cameraTF;
    [SerializeField] Placeable placeableGhost;
    // Properties
    private bool isPlacing;
    private RaycastHit hitInfo;
    // References
    [SerializeField] GameController gameController;
    private IClickable clickableOver; // the object we're directly looking at.


    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Start() {
        SetIsPlacing(false);
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    void SetIsPlacing(bool _isPlacing) {
        isPlacing = _isPlacing;
        placeableGhost.gameObject.SetActive(isPlacing);
    }

    private void PlaceNewObjectAtGhost() {
        Placeable newObj = Instantiate(ResourcesHandler.Instance.Placeable).GetComponent<Placeable>();
        newObj.Initialize(gameController.FieldPropsTF, placeableGhost.transform, placeableGhost.MyType);


        SetIsPlacing(false);
    }



    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    void Update() {
        // Update clickableOver!
        {
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


        // PLACING things
        {
            if (Input.GetKeyDown(KeyCode.E)) { SetIsPlacing(!isPlacing); }

            if (isPlacing) {
                Transform camTF = Camera.main.transform;
                Vector3 pos = camTF.position + cameraTF.forward * PlaceForwardDist;
                placeableGhost.transform.position = pos;
                placeableGhost.transform.rotation = Quaternion.Euler(0, 0, 0);

                if (Input.GetMouseButtonDown(0)) {
                    PlaceNewObjectAtGhost();
                }
            }
        }



    }






}
