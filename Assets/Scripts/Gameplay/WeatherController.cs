using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherState : int {
    Undefined,
    Sunny, Raining
}

public enum TimeOfDay {
    Undefined,
    Dawn, Day, Dusk, Night
}

public class WeatherController : MonoBehaviour
{
    // Constants
    private const float RealSecondsToGameHours = 0.1f; // higher means faster day/night cycles.
    private const float NewSaveStartingHour = 8; // start at 8am.
    // References
    [SerializeField] private Collider c_rainingSky;
    [SerializeField] private GameController gameController;
    [SerializeField] private RainMaker rainMaker;
    [SerializeField] private Material m_skyClear;
    [SerializeField] private Material m_skyOvercast;
    // Properties
    static public float WorldTime; // starts at 0 when we spawn. Only ticks up. Saves and loads.
    public float CurrHour { get; private set; } // from 0 to 24.
    public float TimeWhenNextWeather { get; private set; }
    public float TimeUntilNextWeather { get; private set; }
    public TimeOfDay CurrTimeOfDay { get; private set; }
    public WeatherState CurrState { get; private set; }
    public WeatherState NextState { get; private set; }

    // Getters
    public bool IsRaining { get { return CurrState == WeatherState.Raining; } }


    // ----------------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------------
    public void LoadValuesFromStorage() {
        CurrHour = SaveStorage.GetFloat(SaveKeys.CurrHour(), 0);
        WorldTime = SaveStorage.GetFloat(SaveKeys.WorldTime(), NewSaveStartingHour / RealSecondsToGameHours);
        TimeWhenNextWeather = SaveStorage.GetFloat(SaveKeys.TimeWhenNextWeather(), 9999999);//HACK TEMP disabled storm. 3 * 60);
        SetCurrState((WeatherState) SaveStorage.GetInt(SaveKeys.CurrWeatherState(), 1));
    }
    public void SaveValuesToStorage() {
        SaveStorage.SetFloat(SaveKeys.CurrHour(), CurrHour);
        SaveStorage.SetFloat(SaveKeys.WorldTime(), WorldTime);
        SaveStorage.SetFloat(SaveKeys.TimeWhenNextWeather(), TimeWhenNextWeather);
        SaveStorage.SetInt(SaveKeys.CurrWeatherState(), (int)CurrState);
    }

    

    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        WorldTime += Time.deltaTime;
        CurrHour = (WorldTime * RealSecondsToGameHours) % 24;

        // Countdown TimeUntilNextWeather.
        TimeUntilNextWeather = TimeWhenNextWeather - WorldTime;
        if (TimeUntilNextWeather <= 0) { // time to switch?!
            CycleToNextWeatherState();
        }

        // Update timeOfDay!
        {
            if (CurrHour < 3) CurrTimeOfDay = TimeOfDay.Night;
            else if (CurrHour < 4) CurrTimeOfDay = TimeOfDay.Dawn;
            else if (CurrHour < 22) CurrTimeOfDay = TimeOfDay.Day;
            else if (CurrHour < 23) CurrTimeOfDay = TimeOfDay.Dusk;
            else CurrTimeOfDay = TimeOfDay.Night;
        }

        // Update skybox visuals.
        {
            switch (CurrState) {
                case WeatherState.Sunny:
                    RenderSettings.skybox = TimeUntilNextWeather > 40f ? m_skyClear : m_skyOvercast;
                    RenderSettings.ambientIntensity = 0.2f;// does nothing??
                    switch (CurrTimeOfDay) {
                        case TimeOfDay.Day:
                            RenderSettings.ambientSkyColor = new Color(1, 1, 1);
                            m_skyClear.SetFloat("_Exposure", 1.72f);//CurrHour * 0.1f);
                            m_skyClear.SetFloat("_AtmosphereThickness", 0.63f);
                            break;
                        case TimeOfDay.Night:
                            RenderSettings.ambientSkyColor = new Color255(72, 80, 164).ToColor();
                            m_skyClear.SetFloat("_Exposure", 0.67f);
                            m_skyClear.SetFloat("_AtmosphereThickness", 0.17f);
                            break;
                        case TimeOfDay.Dawn:
                        case TimeOfDay.Dusk:
                            RenderSettings.ambientSkyColor = new Color255(220, 135, 125).ToColor();
                            m_skyClear.SetFloat("_Exposure", 1f);
                            m_skyClear.SetFloat("_AtmosphereThickness", 1.2f);
                            break;
                    }
                    break;
                case WeatherState.Raining:
                    RenderSettings.skybox = m_skyOvercast;
                    RenderSettings.ambientIntensity = 1.0f;// does nothing??
                    switch (CurrTimeOfDay) {
                        case TimeOfDay.Day:
                            RenderSettings.ambientSkyColor = new Color(1,1,1);
                            m_skyOvercast.SetFloat("_Exposure", 1.6f);//CurrHour * 0.1f);
                            m_skyOvercast.SetColor("_Color", new Color(245, 245, 245));
                            break;
                        case TimeOfDay.Night:
                            RenderSettings.ambientSkyColor = new Color255(72, 80, 164).ToColor();
                            m_skyOvercast.SetFloat("_Exposure", 0.1f);
                            m_skyOvercast.SetColor("_Color", new Color(245, 245, 245));
                            break;
                        case TimeOfDay.Dawn:
                        case TimeOfDay.Dusk:
                            RenderSettings.ambientSkyColor = new Color255(220, 135, 125).ToColor();
                            m_skyOvercast.SetFloat("_Exposure", 1.08f);
                            m_skyOvercast.SetColor("_Color", new Color(255, 200, 140));
                            break;
                    }
                    break;
            }
        }
    }


    private void SetCurrState(WeatherState state) {
        CurrState = state;
        switch (CurrState) {
            case WeatherState.Sunny:
                NextState = WeatherState.Raining;
                rainMaker.RainVolume = 0;
                c_rainingSky.enabled = false;
                break;
            case WeatherState.Raining:
                NextState = WeatherState.Sunny;
                rainMaker.RainVolume = 0.8f;
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
                TimeWhenNextWeather = WorldTime + 4 * 60;
                break;

            case WeatherState.Raining:
                SetCurrState(WeatherState.Sunny);
                TimeWhenNextWeather = WorldTime + 3 * 60;
                break;

            default:
                Debug.LogError("Weather state not handled: " + CurrState);
                break;
        }
    }




}
