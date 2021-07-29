using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableBody : MonoBehaviour {
    // Components
    [SerializeField] private Transform headTopTF; // the tippy-top of my head, JUST above all my colliders. I raycast up for rain from here!
    [SerializeField] public ParticleSystem[] ps_leakyLeaks;

    // Getters
    public Transform HeadTopTF { get { return headTopTF; } }


    // Events
    public void UpdateParticlesFromIsInRain(bool isInRain) {
        foreach (ParticleSystem ps in ps_leakyLeaks) {
            GameUtils.SetParticleSystemEmissionEnabled(ps, isInRain);
        }
    }

}
