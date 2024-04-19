using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> {}
public class GameEventListener : MonoBehaviour
{
    // public GameEventSO gameEvent;

    // public CustomGameEvent response;

    // private void OnEnable() {
    //     gameEvent.RegisterListener(this);
    // }

    // private void OnDisable() {
    //     gameEvent.UnregisterListener(this);
    // }

    // public void OnEventRaised(LevelManagerSO sender, int data) {
    //     response.Invoke(sender, data);
    // }
}
