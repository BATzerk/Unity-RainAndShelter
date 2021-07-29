using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType {
    Undefined,
    Neutral, Circle, Hand, Punch
    //Nudge, Grab, Grabbing
}

public class PlayerHand : MonoBehaviour {
    // Constants
    private float ReachRange = 4; // how far from the camera before it's too far to touch.
    private float ObjectHoldDistance = 2; // how far from our face we hold something.
    private float PlaceForwardDist = 6; // how far from the camera we will place something.
    // Components
    [SerializeField] Player player;
    [SerializeField] Camera myCamera;
    [SerializeField] Transform cameraTF;
    [SerializeField] PlaceableGhost placeableGhost;
    // Properties
    private int lm_terrain;
    private int lm_ignorePlayer;
    private bool isPlacing;
    private bool isPlaceableGroundlocked; // if TRUE, this placeable will snap to the terrain.
    private int currPlaceableTypeIndex; // index in Placeable.AvailableTypes!
    private PlaceableInfo currPlaceableInfo;
    private RaycastHit hitInfo;
    // References
    [SerializeField] GameController gameController;
    [SerializeField] GameUI gameUI;
    private IClickable clickableOver; // the object we're directly looking at.
    private Rigidbody rbHolding; // the object we're holding in front of us!

    // Getters
    private PlayerInventory pi { get { return player.Inventory; } }


    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Awake() {
        lm_terrain = 1 << LayerMask.NameToLayer("Terrain");
        lm_ignorePlayer = ~LayerMask.GetMask("Player");
    }
    private void Start() {
        SetIsPlacing(false);
        SetCurrPlaceableTypeIndex(0);
        // Add event listeners.
        EventBus.Instance.PlayerInventoryChangedEvent += OnPlayerInventoryChanged;
    }
    private void OnDestroy() {
        // Remove event listeners.
        EventBus.Instance.PlayerInventoryChangedEvent -= OnPlayerInventoryChanged;
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    private void SetCurrPlaceableTypeIndex(int val) {
        currPlaceableTypeIndex = val;
        PlaceableType type = SimpleBuildingBlock.AvailableTypes[currPlaceableTypeIndex];
        currPlaceableInfo = PlaceableInfo.GetInfoFromType(type);
        placeableGhost.SetMyType(type);
        UpdatePlaceableCanAffordVisuals();
        isPlaceableGroundlocked =
            type == PlaceableType.Campfire
         || type == PlaceableType.StickPillar
         || type == PlaceableType.StickHut
        ;
    }
    private void ChangeTypePlacing(int delta) {
        int newIndex = currPlaceableTypeIndex + delta;
        if (newIndex < 0) newIndex += SimpleBuildingBlock.AvailableTypes.Length;
        if (newIndex >= SimpleBuildingBlock.AvailableTypes.Length) newIndex -= SimpleBuildingBlock.AvailableTypes.Length;
        SetCurrPlaceableTypeIndex(newIndex);
    }
    private void SetIsPlacing(bool _isPlacing) {
        isPlacing = _isPlacing;
        placeableGhost.gameObject.SetActive(isPlacing);
    }

    private void PlaceNewObjectAtGhost() {
        // Place it!
        PlaceableType type = placeableGhost.MyType;
        switch (type) {
            case PlaceableType.Campfire:
                Campfire campfire = Instantiate(ResourcesHandler.Instance.Campfire).GetComponent<Campfire>();
                campfire.Initialize(gameController.FieldPropsTF, placeableGhost.transform, true);
                break;
            case PlaceableType.StickHut:
            case PlaceableType.StickPillar:
            case PlaceableType.StickRoof:
                SimpleBuildingBlock buildingBlock = Instantiate(ResourcesHandler.Instance.SimpleBuildingBlock).GetComponent<SimpleBuildingBlock>();
                buildingBlock.Initialize(gameController, placeableGhost.transform, placeableGhost.MyType, 0);
                break;
        }
        // Consume its cost.
        pi.PayForCost(currPlaceableInfo);

        // Stop placing.
        SetIsPlacing(false);
    }
    private void UpdatePlaceableCanAffordVisuals() {
        if (pi == null) { return; } // Safety check for first frame.
        bool canAfford = currPlaceableInfo.CanAfford(pi);
        placeableGhost.SetCanAfford(canAfford);
    }

    public void PickUpRigidbody(Rigidbody rb) {
        rbHolding = rb;
        // Nullify clickableOver.
        if (clickableOver != null) clickableOver.OnHoverOut();
        clickableOver = null;
        // Stop placing anything.
        SetIsPlacing(false);
    }
    private void DropObjectHolding() {
        rbHolding = null;
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnPlayerInventoryChanged() {
        UpdatePlaceableCanAffordVisuals();
    }


    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    void Update() {
        CursorType ct = CursorType.Neutral; // ...Until I say otherwise.

        // If I'm NOT holding anything...
        if (rbHolding == null) {
            // Update clickableOver!
            {
                IClickable newClickableOver = null;
                if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hitInfo, ReachRange, lm_ignorePlayer)) {
                    GameObject go = hitInfo.transform.gameObject;
                    if (go != null) {
                        newClickableOver = go.GetComponent<IClickable>();
                    }
                }

                // Has it changed??
                if (clickableOver != newClickableOver) {
                    if (clickableOver != null) clickableOver.OnHoverOut();
                    if (newClickableOver != null && newClickableOver.IsClickable()) newClickableOver.OnHoverOver();
                    clickableOver = newClickableOver; // update the ref.
                }

                // A clickable clickableOver!
                if (clickableOver != null && clickableOver.IsClickable()) {
                    // Cursor.
                    ct = clickableOver.CurrCursorForMe();
                    // Click?!?
                    if (Input.GetMouseButtonDown(0)) {
                        clickableOver.OnLClickMe(player);
                    }
                    else if (Input.GetMouseButtonDown(1)) {
                        clickableOver.OnRClickMe(player);
                    }
                }
            }


            // PLACING things
            {
                // Toggle IsPlacing
                if (Input.GetKeyDown(KeyCode.E) && !player.IsInRain) {
                    SetIsPlacing(!isPlacing);
                }
                // Stop placing if we're wet.
                if (isPlacing && player.IsInRain) {
                    SetIsPlacing(false); // wha! We can't get wet and place at the same time! It just doesn't make sense. You can't do things when you're all wet.
                }

                if (isPlacing) {
                    // Maybe change its type!
                    if (Input.mouseScrollDelta.y > 0.1f) {
                        ChangeTypePlacing(1);
                    }
                    else if (Input.mouseScrollDelta.y < -0.1f) {
                        ChangeTypePlacing(-1);
                    }


                    // Update its position!
                    Vector3 pos;
                    if (isPlaceableGroundlocked) { pos = GetGroundlockedPos(); } // Groundlocked? Lock pos to ground.
                    else { pos = cameraTF.position + cameraTF.forward * PlaceForwardDist; }
                    placeableGhost.transform.position = pos;
                    placeableGhost.transform.rotation = Quaternion.Euler(0, myCamera.transform.eulerAngles.y, 0);

                    if (Input.GetMouseButtonDown(0)) {
                        if (currPlaceableInfo.CanAfford(pi)) {
                            PlaceNewObjectAtGhost();
                        }
                    }
                }
            }
        }

        // If I AM holding something...!
        else {
            Transform rbt = rbHolding.transform;
            Vector3 prevPos = rbt.position;

            float holdDistance = ObjectHoldDistance;
            rbt.position = cameraTF.position;
            if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hitInfo, ObjectHoldDistance, lm_terrain)) {
            //if (rbHolding.SweepTest(cameraTF.forward, out hitInfo, ObjectHoldDistance)) {
                holdDistance = hitInfo.distance;
            }
            holdDistance -= 0.2f; // hardcoded hack.

            rbt.position = cameraTF.position + cameraTF.forward * holdDistance;
            rbHolding.velocity = (rbt.position - prevPos) * 10f;
            if (Input.GetMouseButtonDown(0)) {
                DropObjectHolding();
            }
        }

        // Set cursor image!
        gameUI.SetCursorType(ct);
    }


    private Vector3 GetGroundlockedPos() {
        // It's close enough?
        if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hitInfo, PlaceForwardDist, lm_terrain)) {
            return hitInfo.point;
        }
        else {
            Vector3 pos = cameraTF.position + cameraTF.forward * PlaceForwardDist;
            if (Physics.Raycast(new Vector3(pos.x, 1000, pos.z), Vector3.down, out hitInfo, 2000, lm_terrain)) {
                return hitInfo.point;
            }
            return pos; // No hit? Return the original pos, I guess.
        }
    }







}
