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
    [SerializeField] private GameController gameController;
    [SerializeField] private RainMaker rainMaker;
    // Properties
    public float CurrTime;// { get; private set; }
    public float TimeWhenNextWeather { get; private set; }
    public WeatherState CurrState { get; private set; }
    public WeatherState NextState { get; private set; }

    // Getters
    public bool IsRaining { get { return CurrState == WeatherState.Raining; } }


    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    public void LoadWeatherValues() {
        CurrTime = SaveStorage.GetFloat(SaveKeys.CurrGameTime, 0);
        TimeWhenNextWeather = SaveStorage.GetFloat(SaveKeys.TimeWhenNextWeather, 3 * 60);
        SetCurrState((WeatherState) SaveStorage.GetInt(SaveKeys.CurrWeatherState, 1));
    }
    public void SaveWeatherValues() {
        SaveStorage.SetFloat(SaveKeys.CurrGameTime, CurrTime);
        SaveStorage.SetFloat(SaveKeys.TimeWhenNextWeather, TimeWhenNextWeather);
        SaveStorage.SetInt(SaveKeys.CurrWeatherState, (int)CurrState);
    }


    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        UpdateTimers();
    }

    private void UpdateTimers() {
        CurrTime += Time.deltaTime;
        if (CurrTime >= TimeWhenNextWeather) {
            CycleToNextWeatherState();
        }
    }


    private void SetCurrState(WeatherState state) {
        CurrState = state;
        switch (CurrState) {
            case WeatherState.Sunny:
                NextState = WeatherState.Raining;
                rainMaker.RainVolume = 0;
                break;
            case WeatherState.Raining:
                NextState = WeatherState.Sunny;
                rainMaker.RainVolume = 0.8f;
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
