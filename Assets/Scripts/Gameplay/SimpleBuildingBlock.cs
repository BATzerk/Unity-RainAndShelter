using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlaceableType {
    Undefined,
    Campfire,
    SimpleHut,
    StickPillar,
    StickRoof,
}


public struct PlaceableInfo {
    // Properties and Constructor
    public int SticksCost { get; private set; }
    public int StonesCost { get; private set; }
    public PlaceableInfo(int _sticks, int _stones) {
        this.SticksCost = _sticks;
        this.StonesCost = _stones;
    }

    // Static Instances
    public static PlaceableInfo Undefined = new PlaceableInfo(0, 0);
    public static PlaceableInfo Campfire = new PlaceableInfo(5, 0);
    public static PlaceableInfo SimpleHut = new PlaceableInfo(10, 0);
    public static PlaceableInfo StickPillar = new PlaceableInfo(3, 0);
    public static PlaceableInfo StickRoof = new PlaceableInfo(8, 0);
    public static PlaceableInfo GetInfoFromType(PlaceableType type) {
        switch (type) {
            case PlaceableType.Campfire: return Campfire;
            case PlaceableType.SimpleHut: return SimpleHut;
            case PlaceableType.StickPillar: return StickPillar;
            case PlaceableType.StickRoof: return StickRoof;
            default: Debug.LogError("Oops! PlaceableInfo not defined by type: " + type); return Undefined;
        }
    }

    // Getters
    public bool CanAfford(PlayerInventory pi) {
        return pi.NumSticks >= SticksCost
            && pi.NumStones >= StonesCost;
    }

}


public class SimpleBuildingBlock : MonoBehaviour, IClickable
{
    // Statics
    const float NudgeDist = 0.2f;
    public static PlaceableType[] AvailableTypes = { PlaceableType.SimpleHut, PlaceableType.Campfire };//, PlaceableType.StickRoof
    // Components
    [SerializeField] private Rigidbody myRigidbody;
    GameObject bodyGO; // added/changed dynamically.
    // Properties
    public PlaceableType MyType { get; private set; }
    private int numTimesClicked;
    private Coroutine c_resetNumTimesClicked;
    // References
    [SerializeField] private Material m_grayDark;
    [SerializeField] private Material m_simpleHut;
    private GameController gameController;


    // ----------------------------------------------------------------
    //  Setters
    // ----------------------------------------------------------------
    public void SetMaterial(Material mat) {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.material = mat;
        }
    }


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(GameController _gameController, SimpleBuildingBlockData data) {
        Initialize(_gameController, data.pos, data.rot, data.scale, data.myType);
    }
    public void Initialize(GameController _gameController, Transform tf, PlaceableType _myType) {
        Initialize(_gameController, tf.position, tf.eulerAngles, tf.localScale, _myType);
    }
    public void Initialize(GameController _gameController, Vector3 pos, Vector3 rot, Vector3 scale, PlaceableType _myType) {
        this.gameController = _gameController;
        this.transform.parent = gameController.FieldPropsTF;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;

        SetMyType(_myType);
    }


    // ----------------------------------------------------------------
    //  Setters
    // ----------------------------------------------------------------
    public void SetMyType(PlaceableType _type) {
        this.MyType = _type;
        myRigidbody.isKinematic = !(
            MyType == PlaceableType.StickRoof
        );

        // Destroy body.
        if (bodyGO != null) { Destroy(bodyGO); }
        bodyGO = Instantiate(ResourcesHandler.Instance.GetPlaceableBody(this.MyType));
        GameUtils.ParentAndReset(bodyGO, this.transform);

        //SetMaterial(m_simpleHut);
    }



    // ----------------------------------------------------------------
    //  Clicking
    // ----------------------------------------------------------------
    public bool IsClickable() { return true; }
    public CursorType CurrCursorForMe() { return CursorType.Punch; }
    public void OnHoverOver() {
        // todo: Somethin'.
    }
    public void OnHoverOut() {
        // todo: Somethin'.
    }
    public void OnRClickMe(Player player) {
        NudgeFromPlayer(player);
    }
    public void OnLClickMe(Player player) {
        // Increment.
        numTimesClicked++;
        // Stop any coroutine, if exists.
        if (c_resetNumTimesClicked != null) {
            StopCoroutine(c_resetNumTimesClicked);
        }
        // Clicked enough times??
        if (numTimesClicked >= 3) {
            DemolishMeAndSpawnMyContainingResources();
        }
        // Didn't click enough times. Plan on resetting in a few seconds.
        else {
            c_resetNumTimesClicked = StartCoroutine(Coroutine_ResetNumTimesClicked());
        }

    }



    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    private void NudgeFromPlayer(Player player) {
        //Vector3 nudgeNormalized = new Vector3(0, player.GetRotY(), 0);
        this.transform.position += Camera.main.transform.forward * NudgeDist;
    }


    private void DemolishMeAndSpawnMyContainingResources() {
        // Spew out some resources, mate!
        PlaceableInfo myInfo = PlaceableInfo.GetInfoFromType(MyType);
        //gameController.SpawnResources(myInfo);
        for (int i=0; i<myInfo.SticksCost; i++) {
            Vector3 _pos = transform.position + new Vector3(0, 1, 0);// Random.Range(0.5f, 5f), 0);
            Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Stick newObj = Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
            Rigidbody rigidbody = newObj.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));
            rigidbody.velocity = new Vector3(Random.Range(-1.2f, 1.2f), 4, Random.Range(-1.2f, 1.2f));
            newObj.Initialize(transform.parent, _pos, _rot);
        }
        for (int i=0; i<myInfo.StonesCost; i++) {
            Vector3 _pos = transform.position + new Vector3(0, 1, 0);// Random.Range(0.5f, 5f), 0);
            Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Stone newObj = Instantiate(ResourcesHandler.Instance.Stone).GetComponent<Stone>();
            Rigidbody rigidbody = newObj.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));
            rigidbody.velocity = new Vector3(Random.Range(-1.2f, 1.2f), 4, Random.Range(-1.2f, 1.2f));
            newObj.Initialize(transform.parent, _pos, _rot);
        }

        // Destroy me.
        Destroy(this.gameObject);
    }

    private IEnumerator Coroutine_ResetNumTimesClicked() {
        yield return new WaitForSeconds(4); // Reset NumTimesClicked in 4 seconds.
        numTimesClicked = 0;
    }




}
