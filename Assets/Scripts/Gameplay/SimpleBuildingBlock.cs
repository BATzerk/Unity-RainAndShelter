using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlaceableType {
    Undefined,
    Campfire,
    StickHut,
    StickPillar,
    StickRoof,
}


public struct PlaceableInfo {
    // Properties and Constructor
    public int SticksCost { get; private set; }
    public int StonesCost { get; private set; }
    public int StringsCost { get; private set; }
    public PlaceableInfo(int _sticks, int _stones, int _strings) {
        this.SticksCost = _sticks;
        this.StonesCost = _stones;
        this.StringsCost = _strings;
    }

    // Static Instances
    public static PlaceableInfo Undefined = new PlaceableInfo(0, 0, 0);
    public static PlaceableInfo Campfire = new PlaceableInfo(5, 0, 0);
    public static PlaceableInfo StickHut = new PlaceableInfo(10, 0, 0);
    public static PlaceableInfo StickPillar = new PlaceableInfo(3, 0, 0);
    public static PlaceableInfo StickRoof = new PlaceableInfo(8, 0, 0);
    public static PlaceableInfo GetInfoFromType(PlaceableType type) {
        switch (type) {
            case PlaceableType.Campfire: return Campfire;
            case PlaceableType.StickHut: return StickHut;
            case PlaceableType.StickPillar: return StickPillar;
            case PlaceableType.StickRoof: return StickRoof;
            default: Debug.LogError("Oops! PlaceableInfo not defined by type: " + type); return Undefined;
        }
    }

    // Getters
    public bool CanAfford(PlayerInventory pi) {
        return pi.NumSticks >= SticksCost
            && pi.NumStones >= StonesCost
            && pi.NumStrings >= StringsCost
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
    public static PlaceableType[] AvailableTypes = { PlaceableType.StickHut, PlaceableType.Campfire };//, PlaceableType.StickRoof
    // Components
    [SerializeField] private Rigidbody myRigidbody;
    private PlaceableBody myBody; // added/changed dynamically.
    // Properties
    public PlaceableType MyType { get; private set; }
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
    public void Initialize(GameController _gameController, Transform tf, PlaceableType _myType, float _timeInRain) {
        Initialize(_gameController, tf.position, tf.eulerAngles, tf.localScale, _myType, _timeInRain);
    }
    public void Initialize(GameController _gameController, Vector3 pos, Vector3 rot, Vector3 scale, PlaceableType _myType, float _timeInRain) {
        this.gameController = _gameController;
        this.transform.parent = gameController.FieldPropsTF;
        this.transform.position = pos;
        this.transform.eulerAngles = rot;
        this.transform.localScale = scale;
        prevWorldTime = WeatherController.WorldTime;
        this.MyType = _myType;
        myRigidbody.isKinematic = !(
            MyType == PlaceableType.StickRoof
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
        PlaceableInfo myInfo = PlaceableInfo.GetInfoFromType(MyType);
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
