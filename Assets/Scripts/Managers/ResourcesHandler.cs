using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [Header ("Common")]
    [SerializeField] public GameObject ImageLine;
    [SerializeField] public GameObject ImageLinesJoint;
    
    [Header ("PrioritiesGame")]
    [SerializeField] public Material m_Additive;
    [SerializeField] public GameObject UFOShootDown_Missile;
    
    [SerializeField] public GameObject JugglingGame_Ball;
    [SerializeField] public GameObject PremadePrioToggle;
    //[SerializeField] public GameObject PriosFinalRankRowView;
    [SerializeField] public GameObject PriosManualRankRowBacking;
    [SerializeField] public GameObject PriosManualRankRowViewAuto;
    [SerializeField] public GameObject PriosManualRankRowViewManual;
    
    
    
    
    // Instance
    static public ResourcesHandler Instance { get; private set; }


    // ----------------------------------------------------------------
    //  Awake
    // ----------------------------------------------------------------
    private void Awake () {
        // There can only be one (instance)!
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy (this);
        }
	}
    
}
