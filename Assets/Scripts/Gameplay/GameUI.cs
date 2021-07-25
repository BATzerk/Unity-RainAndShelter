﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    // Components
    [SerializeField] TextMeshProUGUI t_countdown;
    [SerializeField] TextMeshProUGUI t_numSticks;
    [SerializeField] TextMeshProUGUI t_numStones;
    // References
    [SerializeField] private WeatherController weatherController;


    // Getters
    private DataManager dm { get { return GameManagers.Instance.DataManager; } }
    static string GetWeatherName(WeatherState state) {
        switch (state) {
            case WeatherState.Raining: return "storm";
            case WeatherState.Sunny: return "sun";
            default: return "undefined";
        }
    }


    // ----------------------------------------------------------------
    //  Start / Destroy
    // ----------------------------------------------------------------
    private void Start() {
        UpdateInventoryTexts();

        // Add event listeners!
        EventBus.Instance.PlayerInventoryChangedEvent += OnPlayerInventoryChanged;
    }
    private void OnDestroy() {
        // Remove event listeners!
        EventBus.Instance.PlayerInventoryChangedEvent -= OnPlayerInventoryChanged;
    }


    private void Update() {
        float secsUntilNextWeatherState = weatherController.TimeWhenNextWeather - weatherController.CurrTime;
        string str = "time until ";
        str += GetWeatherName(weatherController.NextState) + ": ";
        str += TextUtils.GetSecondsToTimeString(secsUntilNextWeatherState);
        t_countdown.text = str;
    }


    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    private void OnPlayerInventoryChanged() {
        UpdateInventoryTexts();
    }


    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    private void UpdateInventoryTexts() {
        t_numSticks.text = "sticks: " + dm.PlayerInventory.NumSticks;
        t_numStones.text = "stones: " + dm.PlayerInventory.NumStones;
    }


}