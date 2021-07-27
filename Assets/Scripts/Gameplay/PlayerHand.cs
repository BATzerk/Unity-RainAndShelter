using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    // Constants
    private float ReachRange = 4; // how far from the camera before it's too far to touch.
    private float PlaceForwardDist = 6; // how far from the camera we will place something.
    // Components
    [SerializeField] Player player;
    [SerializeField] Camera myCamera;
    [SerializeField] Transform cameraTF;
    [SerializeField] Placeable placeableGhost;
    // Properties
    private int lm_terrain;
    private bool isPlacing;
    private bool isPlaceableGroundlocked; // if TRUE, this placeable will snap to the terrain.
    private int currPlaceableTypeIndex; // index in Placeable.AvailableTypes!
    private PlaceableInfo currPlaceableInfo;
    private RaycastHit hitInfo;
    // References
    [SerializeField] GameController gameController;
    private IClickable clickableOver; // the object we're directly looking at.

    // Getters
    private PlayerInventory pi { get { return player.Inventory; } }

    //private bool CanAffordPlaceable(int typeIndex) { return CanAffordPlaceable(Placeable.AvailableTypes[typeIndex]); }
    //private bool CanAffordPlaceable(PlaceableType type) {
    //    return .
    //    PlayerInventory pi = GameManagers.Instance.DataManager.PlayerInventory;
    //    switch (type) {
    //        case PlaceableType.SimpleHut: return pi.NumSticks >= 20;
    //        case PlaceableType.StickPillar: return pi.NumSticks >= 3;
    //        case PlaceableType.StickRoof: return pi.NumSticks >= 8;
    //        default: Debug.LogError("Whoa! PlaceableType not defined: " + type); return true;
    //    }
    //}


    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Awake() {
        lm_terrain = 1 << LayerMask.NameToLayer("Terrain");
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
        PlaceableType type = Placeable.AvailableTypes[currPlaceableTypeIndex];
        currPlaceableInfo = PlaceableInfo.GetInfoFromType(type);
        placeableGhost.SetMyType(type);
        UpdatePlaceableCanAffordVisuals();
        isPlaceableGroundlocked =
            type == PlaceableType.StickPillar
         || type == PlaceableType.SimpleHut
        ;
    }
    private void ChangeTypePlacing(int delta) {
        int newIndex = currPlaceableTypeIndex + delta;
        if (newIndex < 0) newIndex += Placeable.AvailableTypes.Length;
        if (newIndex >= Placeable.AvailableTypes.Length) newIndex -= Placeable.AvailableTypes.Length;
        SetCurrPlaceableTypeIndex(newIndex);
    }
    private void SetIsPlacing(bool _isPlacing) {
        isPlacing = _isPlacing;
        placeableGhost.gameObject.SetActive(isPlacing);
    }

    private void PlaceNewObjectAtGhost() {
        // Place it.
        Placeable newObj = Instantiate(ResourcesHandler.Instance.Placeable).GetComponent<Placeable>();
        newObj.Initialize(gameController, placeableGhost.transform, placeableGhost.MyType);
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
            if (clickableOver != null && clickableOver.IsClickable()) {
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
                Transform camTF = Camera.main.transform;
                Vector3 pos = camTF.position + cameraTF.forward * PlaceForwardDist;
                if (isPlaceableGroundlocked) { pos = GetGroundlockedPos(pos); } // Groundlocked? Lock pos to ground.
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


    private Vector3 GetGroundlockedPos(Vector3 originalPos) {
        if (Physics.Raycast(new Vector3(originalPos.x,1000,originalPos.z), Vector3.down, out hitInfo, 2000, lm_terrain)) {
            return hitInfo.point;
        }
        return originalPos; // No hit? Return the original pos, I guess.
    }







}
