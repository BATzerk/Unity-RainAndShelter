using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CraftableType {
    Undefined,

    //// Tools
    //Axe,
    //Shovel,

    // Placeables
    Campfire,
    StickHut,
    StickPillar,
    StickRoof,
}


public struct CraftableInfo {
    // Properties and Constructor
    public int SticksCost { get; private set; }
    public int StonesCost { get; private set; }
    public int TwinesCost { get; private set; }
    public CraftableInfo(int _sticks, int _stones, int _twines) {
        this.SticksCost = _sticks;
        this.StonesCost = _stones;
        this.TwinesCost = _twines;
    }

    // Static Instances
    public static CraftableInfo Undefined = new CraftableInfo(0, 0, 0);

    public static CraftableInfo Axe = new CraftableInfo(2, 3, 3);
    public static CraftableInfo Shovel = new CraftableInfo(3, 2, 3);

    public static CraftableInfo Campfire = new CraftableInfo(5, 0, 0);
    public static CraftableInfo StickHut = new CraftableInfo(10, 0, 0);
    public static CraftableInfo StickPillar = new CraftableInfo(3, 0, 0);
    public static CraftableInfo StickRoof = new CraftableInfo(8, 0, 0);
    public static CraftableInfo GetInfoFromType(CraftableType type) {
        switch (type) {
            case CraftableType.Campfire: return Campfire;
            case CraftableType.StickHut: return StickHut;
            case CraftableType.StickPillar: return StickPillar;
            case CraftableType.StickRoof: return StickRoof;
            default: Debug.LogError("Oops! CraftableInfo not defined by type: " + type); return Undefined;
        }
    }
    public static CraftableInfo GetInfoFromType(ToolType type) {
        switch (type) {
            case ToolType.Axe: return Axe;
            case ToolType.Shovel: return Shovel;
            default: Debug.LogError("Oops! CraftableInfo not defined by type: " + type); return Undefined;
        }
    }

    // Getters
    public bool CanAfford(PlayerInventory pi) {
        return pi.NumSticks >= SticksCost
            && pi.NumStones >= StonesCost
            && pi.NumTwines >= TwinesCost
        ;
    }
}


public enum WeatheredState {
    Undefined,
    Good, Leaky, Ruined
}

public class SimpleBuildingBlock : MonoBehaviour, IClickable
{
    // Statics
    const float NudgeDist = 0.2f;
    public static CraftableType[] AvailableTypes = { CraftableType.StickHut, CraftableType.Campfire };//, PlaceableType.StickRoof
    // Components
    [SerializeField] private Rigidbody myRigidbody;
    private PlaceableBody myBody; // added/changed dynamically.
    // Properties
    public CraftableType MyType { get; private set; }
    public WeatheredState WeatheredState { get; private set; }
    public float TimeInRain { get; private set; }
    private float prevWorldTime; // to calculate DeltaTime for low-frame update.
    private bool isInRain;
    private int numTimesClicked;
    private Coroutine c_resetNumTimesClicked;
    private RaycastHit hit;
    // References
    [SerializeField] private Material m_grayDark;
    [SerializeField] private Material m_simpleHut;
    private GameController gameController;


    // ----------------------------------------------------------------
    //  Setters
    // ----------------------------------------------------------------
    private bool CheckIsInRain() {
        if (Physics.Raycast(myBody.HeadTopTF.position, Vector3.up, out hit, 2000)) { // HARDCODED starting the ray ABOVE the campfire. So we don't collide with it. :P
            GameObject go = hit.transform.gameObject;
            if (go.tag == Tags.RainingSky) {
                return true;
            }
        }
        return false;
    }
    private void SetTimeInRain(float _timeInRain) {
        float threshLeaky = 100;
        float threshRuined = 170;

        WeatheredState prevState = WeatheredState;
        TimeInRain = _timeInRain;
        // Update WeatheredState from TimeInRain.
        if (TimeInRain >= threshRuined) { WeatheredState = WeatheredState.Ruined; }
        else if (TimeInRain >= threshLeaky) { WeatheredState = WeatheredState.Leaky; }
        else { WeatheredState = WeatheredState.Good; }

        // Has our WeatheredState changed?!
        if (WeatheredState != prevState) {
            RemakeBody();
        }
    }



    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Start() {
        // Start low-frame update.
        InvokeRepeating("LowFrameUpdate", 0, 0.5f);
    }

    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(GameController _gameController, SimpleBuildingBlockData data) {
        Initialize(_gameController, data.pos, data.rot, data.scale, data.myType, data.TimeInRain);
    }
    public void Initialize(GameController _gameController, Transform tf, CraftableType _myType, float _timeInRain) {
        Initialize(_gameController, tf.position, tf.eulerAngles, tf.localScale, _myType, _timeInRain);
    }
    public void Initialize(GameController _gameController, Vector3 pos, Vector3 rot, Vector3 scale, CraftableType _myType, float _timeInRain) {
        this.gameController = _gameController;
        this.transform.parent = gameController.FieldPropsTF;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;
        prevWorldTime = WeatherController.WorldTime;
        this.MyType = _myType;
        myRigidbody.isKinematic = !(
            MyType == CraftableType.StickRoof
        );

        SetTimeInRain(_timeInRain);
    }

    private void RemakeBody() {
        // Destroy body.
        if (myBody != null) { Destroy(myBody.gameObject); }
        myBody = Instantiate(ResourcesHandler.Instance.GetPlaceableBody(this.MyType, this.WeatheredState)).GetComponent<PlaceableBody>();
        GameUtils.ParentAndReset(myBody.gameObject, this.transform);
        myBody.UpdateParticlesFromIsInRain(isInRain);
    }





    // ----------------------------------------------------------------
    //  Low-Frame Update!
    // ----------------------------------------------------------------
    private void LowFrameUpdate() {
        float deltaTime = WeatherController.WorldTime - prevWorldTime;
        bool prevIsInRain = isInRain;

        isInRain = CheckIsInRain();

        // Am I in the rain?
        if (isInRain) {
            SetTimeInRain(TimeInRain + deltaTime);
        }
        // Has my rain-status changed?
        if (isInRain != prevIsInRain) {
            myBody.UpdateParticlesFromIsInRain(isInRain);
        }

        prevWorldTime = WeatherController.WorldTime;
    }






    // ----------------------------------------------------------------
    //  Clicking
    // ----------------------------------------------------------------
    public bool IsClickable(Tool tool) { return true; }
    public CursorType CurrCursorForMe(Tool tool) { return CursorType.Punch; }
    public void OnHoverOver() {
    }
    public void OnHoverOut() {
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
        // Determine how much we SHOULDN'T get back, due to being weathered.
        float sellbackRatio;
        switch (WeatheredState) {
            case WeatheredState.Good: sellbackRatio = 1; break;
            case WeatheredState.Leaky: sellbackRatio = 0.6f; break;
            case WeatheredState.Ruined: sellbackRatio = 0.2f; break;
            default: sellbackRatio = 1; break;
        }

        // Spew out some resources, mate!
        CraftableInfo myInfo = CraftableInfo.GetInfoFromType(MyType);
        int numSticks = Mathf.FloorToInt(myInfo.SticksCost * sellbackRatio);
        int numStones = Mathf.FloorToInt(myInfo.StonesCost * sellbackRatio);

        for (int i=0; i< numSticks; i++) {
            Vector3 _pos = transform.position + new Vector3(0, 1, 0);// Random.Range(0.5f, 5f), 0);
            Vector3 _rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Stick newObj = Instantiate(ResourcesHandler.Instance.Stick).GetComponent<Stick>();
            Rigidbody rigidbody = newObj.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));
            rigidbody.velocity = new Vector3(Random.Range(-1.2f, 1.2f), 4, Random.Range(-1.2f, 1.2f));
            newObj.Initialize(transform.parent, _pos, _rot);
        }
        for (int i=0; i<numStones; i++) {
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
