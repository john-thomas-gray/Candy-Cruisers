using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvent")]
public class GameEventSO : ScriptableObject
{
    public List<GameEventListener> listeners = new List<GameEventListener>();

    // Raise event through different method signatures

    public void Raise() {
        for (int i = 0; i < listeners.Count; i++) {
            listeners[i].OnEventRaised();
        }
    }

    // Manage Listeners

    public void RegisterListener(GameEventListener listener) {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener) {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}
