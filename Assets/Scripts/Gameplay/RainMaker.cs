using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainMaker : MonoBehaviour {
    // Components
    [SerializeField] private ParticleSystem ps_mistFar;
    [SerializeField] private ParticleSystem ps_mistMed;
    [SerializeField] private ParticleSystem ps_mistNear;
    [SerializeField] private ParticleSystem ps_rainDrops;
    [SerializeField] private Image i_screenFog;
    // Properties
    public float RainVolume = 0.8f; // from 0 to 1!
    private float screenFogAlpha;
    private float screenFogAlphaTarget;
    RaycastHit hit;
    // References
    [SerializeField] private Player player;
    private Transform tf_mainCamera;




    // ----------------------------------------------------------------
    //  Start
    // ----------------------------------------------------------------
    private void Start() {
        tf_mainCamera = Camera.main.transform;
    }



    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void FixedUpdate() {
        // Screen Fog
        {
            // Update target
            float targetDry  = Mathf.Lerp(0f, 0.35f, RainVolume);
            float targetRain = Mathf.Lerp(0.3f, 0.7f, RainVolume);
            screenFogAlphaTarget = player.IsUnderShelter||RainVolume<0.1f ? targetDry : targetRain;
            // Apply target
            if (screenFogAlpha < screenFogAlphaTarget) {
                screenFogAlpha = Mathf.Min(1, screenFogAlpha + 0.01f);
            }
            if (screenFogAlpha > screenFogAlphaTarget) {
                screenFogAlpha = Mathf.Max(0, screenFogAlpha - 0.01f);
            }
            // Apply alpha
            GameUtils.SetUIGraphicAlpha(i_screenFog, screenFogAlpha);
        }
    }


    void Update() {
        // ps_rainDrops
        ps_rainDrops.gameObject.transform.position = tf_mainCamera.position + new Vector3(0, 30, 0);
        GameUtils.SetParticleSystemEmissionRate(ps_rainDrops, Mathf.Lerp(0, 2000, RainVolume));


        if (RainVolume > 0.5f) // TEMP BOOLEAN on/off with the fog.
        {
            // Mist Near
            {
                float x = tf_mainCamera.position.x + Random.Range(-20, 20);
                float z = tf_mainCamera.position.z + Random.Range(-20, 20);
                if (Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out hit, 200)) {
                    ps_mistNear.gameObject.transform.localPosition = hit.point;
                    ps_mistNear.Emit(1);
                }
            }

            // Mist Med
            if (Time.frameCount % 4 == 0) {
                float angle = Random.Range(0, Mathf.PI * 2);
                float dist = Random.Range(20, 80);
                float x = tf_mainCamera.position.x + Mathf.Cos(angle) * dist;
                float z = tf_mainCamera.position.z + Mathf.Sin(angle) * dist;
                if (Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out hit, 200)) {
                    ps_mistMed.gameObject.transform.localPosition = hit.point;
                    ps_mistMed.Emit(1);
                }
            }

            // Mist Far
            if (Time.frameCount % 14 == 0) {
                float angle = Random.Range(0, Mathf.PI * 2);
                float dist = Random.Range(80, 260);
                float x = tf_mainCamera.position.x + Mathf.Cos(angle) * dist;
                float z = tf_mainCamera.position.z + Mathf.Sin(angle) * dist;
                if (Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out hit, 200)) {
                    ps_mistFar.gameObject.transform.localPosition = hit.point;
                    ps_mistFar.Emit(1);
                }
            }
        }
    }


}
