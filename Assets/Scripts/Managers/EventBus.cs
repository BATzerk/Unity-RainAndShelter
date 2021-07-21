using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventBus {
	// Instance
	public static bool IsInitializing { get; private set; }
	private EventBus() { }
	static private EventBus instance;
	static public EventBus Instance {
		get {
			if (instance == null) {
				if (IsInitializing) { // Uh-oh safety check.
					Debug.LogError("EventManager access loop infinite recursion error! It's trying to access itself before it's done being initialized.");
					return null; // So the program doesn't freeze.
				}
				else {
					IsInitializing = true;
					instance = new EventBus();
				}
			}
			else {
				IsInitializing = false; // Don't HAVE to update this value at all, but it's nice to for accuracy.
			}
			return instance;
		}
	}



	// Actions and Event Variables
	public delegate void NoParamAction ();
	public delegate void BoolAction (bool _bool);
	public delegate void StringAction (string _str);
    public delegate void IntAction (int _int);
    public delegate void StringBoolAction (string _str, bool _bool);

    // Common
    public event NoParamAction ScreenSizeChangedEvent;
    public void OnScreenSizeChanged () { ScreenSizeChangedEvent?.Invoke(); }


    // Gameplay
    public event NoParamAction PlayerInventoryChangedEvent;

    public void OnPlayerInventoryChanged() { PlayerInventoryChangedEvent?.Invoke(); }







}




