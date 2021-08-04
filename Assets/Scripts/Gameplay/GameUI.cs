using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    // Components
    [SerializeField] Image i_cursor;
    [SerializeField] TextMeshProUGUI t_countdown;
    [SerializeField] TextMeshProUGUI t_debugInfo;
    // References
    [SerializeField] private WeatherController weatherController;
    [SerializeField] private Sprite s_cursorNeutral;
    [SerializeField] private Sprite s_cursorCircle;
    [SerializeField] private Sprite s_cursorHand;
    [SerializeField] private Sprite s_cursorPunch;
    [SerializeField] private Sprite s_cursorChop;
    [SerializeField] private Sprite s_cursorDig;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }
    static string GetWeatherName(WeatherState state) {
        switch (state) {
            case WeatherState.Raining: return "storm";
            case WeatherState.Sunny: return "sun";
            default: return "undefined";
        }
    }
    private Sprite GetCursorSpriteFromType(CursorType ct) {
        switch (ct) {
            case CursorType.Neutral: return s_cursorNeutral;
            case CursorType.Circle: return s_cursorCircle;
            case CursorType.Hand: return s_cursorHand;
            case CursorType.Punch: return s_cursorPunch;
            case CursorType.Chop: return s_cursorChop;
            case CursorType.Dig: return s_cursorDig;
            default: Debug.LogError("No cursor sprite for type: " + ct); return null;
        }
    }


    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------
    private void Update() {
        // Weather Countdown
        float secsUntilNextWeatherState = weatherController.TimeWhenNextWeather - WeatherController.WorldTime;
        string str = "time until ";
        str += GetWeatherName(weatherController.NextState) + ": ";
        str += TextUtils.GetSecondsToTimeString(secsUntilNextWeatherState);
        t_countdown.text = str;

        // Debug text
        t_debugInfo.text = "Time: " + TextUtils.GetSecondsToTimeString(weatherController.CurrHour*60);
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    public void SetCursorType(CursorType ct) {
        i_cursor.sprite = GetCursorSpriteFromType(ct);
    }


}
