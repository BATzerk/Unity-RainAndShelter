using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType {
    Undefined,
    Neutral, Circle, Hand, Punch,
    Chop, Dig,
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
    public int currToolIndex { get; private set; }
    public Tool currTool { get; private set; } // depends on currToolIndex. Can be Hand, Axe, or Shovel.
    private CraftableInfo currPlaceableInfo;
    private RaycastHit hit;
    private RaycastHit[] hits;
    // References
    [SerializeField] GameController gameController;
    [SerializeField] GameUI gameUI;
    private IClickable clickableOver; // the object we're directly looking at.
    private Rigidbody rbHolding; // the object we're holding in front of us!

    // Getters
    private PlayerInventory pi { get { return player.Inventory; } }


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Awake() {
        lm_terrain = 1 << LayerMask.NameToLayer("Terrain");
        lm_ignorePlayer = ~LayerMask.GetMask("Player");
        // Add event listeners.
        EventBus.Instance.PlayerInventoryChangedEvent += OnPlayerInventoryChanged;
    }
    private void OnDestroy() {
        // Remove event listeners.
        EventBus.Instance.PlayerInventoryChangedEvent -= OnPlayerInventoryChanged;
    }

    public void Initialize() {
        SetIsPlacing(false);
        SetCurrToolIndex(0);
        SetCurrPlaceableTypeIndex(0);
    }



    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void DamageCurrTool(float dmg) {
        // If I've got a current tool, inflict the damage!
        if (currTool != null) {
            currTool.Health -= dmg;
            // Out of health?? Destroy the tool!
            if (currTool.Health <= 0) {
                player.ToolBelt.tools[currToolIndex] = null; // We actually only have to null it out!
                SetCurrToolIndex(currToolIndex); // Refresh this value.
            }
        }
        // Dispatch event.
        EventBus.Instance.OnPlayerToolBeltChanged();
    }

    private void SetCurrToolIndex(int val) {
        currToolIndex = val;
        currTool = player.ToolBelt.tools[currToolIndex];
        EventBus.Instance.OnPlayerToolBeltChanged();
        // Just in case, stop placing or holding anything.
        SetIsPlacing(false);
        DropRigidbodyHolding();
    }
    private void ChangeCurrToolIndex(int delta) {
        int index = currToolIndex + delta;
        if (index < 0) index += player.ToolBelt.tools.Length;
        if (index >= player.ToolBelt.tools.Length) index -= player.ToolBelt.tools.Length;
        SetCurrToolIndex(index);
    }


    private void SetCurrPlaceableTypeIndex(int val) {
        currPlaceableTypeIndex = val;
        CraftableType type = SimpleBuildingBlock.AvailableTypes[currPlaceableTypeIndex];
        currPlaceableInfo = CraftableInfo.GetInfoFromType(type);
        placeableGhost.SetMyType(type);
        UpdatePlaceableCanAffordVisuals();
        isPlaceableGroundlocked =
            type == CraftableType.Campfire
         || type == CraftableType.StickPillar
         || type == CraftableType.StickHut
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
        CraftableType type = placeableGhost.MyType;
        switch (type) {
            case CraftableType.Campfire:
                Campfire campfire = Instantiate(ResourcesHandler.Instance.Campfire).GetComponent<Campfire>();
                campfire.Initialize(gameController.FieldPropsTF, placeableGhost.transform, true);
                break;
            case CraftableType.StickHut:
            case CraftableType.StickPillar:
            case CraftableType.StickRoof:
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
    private void DropRigidbodyHolding() {
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

        // ToolBelt
        {
            if (Input.mouseScrollDelta.y > 0.1f) {
                ChangeCurrToolIndex(1);
            }
            else if (Input.mouseScrollDelta.y < -0.1f) {
                ChangeCurrToolIndex(-1);
            }
        }


        // If I'm NOT holding anything...
        if (rbHolding == null) {
            // Update clickableOver!
            {
                IClickable newClickableOver = null;

                // Do a RaycastAll, so we can look THROUGH any clickables that say they CAN'T be clicked. So, e.g., a Bush that can no longer be clicked doesn't block the sticks next to it.
                hits = Physics.RaycastAll(cameraTF.position, cameraTF.forward, ReachRange, lm_ignorePlayer);
                System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance)); // Sort them by distance! They don't come in in guaranteed order.
                for (int i=0; i<hits.Length; i++) {
                    GameObject go = hits[i].transform.gameObject;
                    if (go == null) { break; } // No GO? Break.
                    IClickable clickable = go.GetComponent<IClickable>();
                    if (clickable == null) { break; } // Hit a non-clickable? Also break.
                    if (clickable.IsClickable(currTool)) { // If this one CAN be clicked, use it!
                        newClickableOver = clickable;
                        break;
                    }
                }

                // Has it changed??
                if (clickableOver != newClickableOver) {
                    if (clickableOver != null) clickableOver.OnHoverOut();
                    if (newClickableOver != null && newClickableOver.IsClickable(currTool)) newClickableOver.OnHoverOver();
                    clickableOver = newClickableOver; // update the ref.
                }

                // A clickable clickableOver!
                if (clickableOver != null && clickableOver.IsClickable(currTool)) {
                    // Cursor.
                    ct = clickableOver.CurrCursorForMe(currTool);
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
                    if (Input.GetKeyDown(KeyCode.F)) {
                        ChangeTypePlacing(1);
                    }
                    else if (Input.GetKeyDown(KeyCode.G)) {
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
            if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hit, ObjectHoldDistance, lm_terrain)) {
            //if (rbHolding.SweepTest(cameraTF.forward, out hitInfo, ObjectHoldDistance)) {
                holdDistance = hit.distance;
            }
            holdDistance -= 0.2f; // hardcoded hack.

            rbt.position = cameraTF.position + cameraTF.forward * holdDistance;
            rbHolding.velocity = (rbt.position - prevPos) * 10f;
            if (Input.GetMouseButtonDown(0)) {
                DropRigidbodyHolding();
            }
        }

        // Set cursor image!
        gameUI.SetCursorType(ct);
    }


    private Vector3 GetGroundlockedPos() {
        // It's close enough?
        if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hit, PlaceForwardDist, lm_terrain)) {
            return hit.point;
        }
        else {
            Vector3 pos = cameraTF.position + cameraTF.forward * PlaceForwardDist;
            if (Physics.Raycast(new Vector3(pos.x, 1000, pos.z), Vector3.down, out hit, 2000, lm_terrain)) {
                return hit.point;
            }
            return pos; // No hit? Return the original pos, I guess.
        }
    }







}







//if (Physics.Raycast(cameraTF.position, cameraTF.forward, out hit, ReachRange, lm_ignorePlayer)) {
//    GameObject go = hit.transform.gameObject;
//    if (go != null) {
//        newClickableOver = go.GetComponent<IClickable>();
//    }
//}