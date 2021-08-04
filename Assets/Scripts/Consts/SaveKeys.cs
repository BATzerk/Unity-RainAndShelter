using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveKeys {
    public static int CurrLevelIndex { get; private set; }
    public static void SetCurrLevelIndex(int val) {
        CurrLevelIndex = val;
    }
    private static string cliPrefix { get { return CurrLevelIndex + "_"; } }


    public static string PlayerInventory() { return cliPrefix + "PlayerInventory"; }
    public static string PlayerToolBelt() { return cliPrefix + "PlayerToolBelt"; }
    public static string PlayerPos() { return cliPrefix + "PlayerPos"; }
    public static string PlayerRotY() { return cliPrefix + "PlayerRotY"; }
    public static string FieldPropsData() { return cliPrefix + "FieldPropsData"; }

    public static string CurrHour() { return cliPrefix + "CurrHour"; }
    public static string WorldTime() { return cliPrefix + "CurrGameTime"; }
    public static string CurrWeatherState() { return cliPrefix + "CurrWeatherState"; }
    public static string TimeWhenNextWeather() { return cliPrefix + "TimeWhenNextWeather"; }



}
