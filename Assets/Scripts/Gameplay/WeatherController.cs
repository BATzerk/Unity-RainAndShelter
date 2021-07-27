using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherState : int {
    Undefined,
    Sunny, Raining
}

public class WeatherController : MonoBehaviour
{
    // References
    [SerializeField] private Collider c_rainingSky;
    [SerializeField] private GameController gameController;
    [SerializeField] private RainMaker rainMaker;
    [SerializeField] private Material m_skyClear;
    [SerializeField] private Material m_skyOvercast;
    // Properties
    public float CurrTime;// { get; private set; }
    public float TimeWhenNextWeather { get; private set; }
    public float TimeUntilNextWeather { get; private set; }
    public WeatherState CurrState { get; private set; }
    public WeatherState NextState { get; private set; }

    // Getters
    public bool IsRaining { get { return CurrState == WeatherState.Raining; } }


    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    public void LoadValuesFromStorage() {
        CurrTime = SaveStorage.GetFloat(SaveKeys.CurrGameTime(), 0);
        TimeWhenNextWeather = SaveStorage.GetFloat(SaveKeys.TimeWhenNextWeather(), 3 * 60);
        SetCurrState((WeatherState) SaveStorage.GetInt(SaveKeys.CurrWeatherState(), 1));
    }
    public void SaveValuesToStorage() {
        SaveStorage.SetFloat(SaveKeys.CurrGameTime(), CurrTime);
        SaveStorage.SetFloat(SaveKeys.TimeWhenNextWeather(), TimeWhenNextWeather);
        SaveStorage.SetInt(SaveKeys.CurrWeatherState(), (int)CurrState);
    }


    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        UpdateTimers();

        // Brute-force every-frame updating the skybox.
        switch (CurrState) {
            case WeatherState.Sunny:
                if (TimeUntilNextWeather > 40f)
                    RenderSettings.skybox = m_skyClear;
                else
                    RenderSettings.skybox = m_skyOvercast;
                break;
            case WeatherState.Raining:
                RenderSettings.skybox = m_skyOvercast;
                break;
        }
    }

    private void UpdateTimers() {
        CurrTime += Time.deltaTime;
        TimeUntilNextWeather = TimeWhenNextWeather - CurrTime;
        if (TimeUntilNextWeather <= 0) { // time to switch?!
            CycleToNextWeatherState();
        }
    }


    private void SetCurrState(WeatherState state) {
        CurrState = state;
        switch (CurrState) {
            case WeatherState.Sunny:
                NextState = WeatherState.Raining;
                rainMaker.RainVolume = 0;
                RenderSettings.skybox = m_skyClear;
                c_rainingSky.enabled = false;
                break;
            case WeatherState.Raining:
                NextState = WeatherState.Sunny;
                rainMaker.RainVolume = 0.8f;
                RenderSettings.skybox = m_skyOvercast;
                c_rainingSky.enabled = true;
                break;
            default:
                Debug.LogError("Weather state not handled: " + state);
                break;
        }
    }
    private void CycleToNextWeatherState() {
        switch (CurrState) {
            case WeatherState.Sunny:
                SetCurrState(WeatherState.Raining);
                TimeWhenNextWeather = CurrTime + 3 * 60;
                break;

            case WeatherState.Raining:
                SetCurrState(WeatherState.Sunny);
                TimeWhenNextWeather = CurrTime + 3 * 60;
                break;

            default:
                Debug.LogError("Weather state not handled: " + CurrState);
                break;
        }
    }



}
