using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action CheckRetreat;

    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(EventManager).Name;
                    instance = obj.AddComponent<EventManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy this instance if another instance already exists
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to the scene reload event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the scene reload event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the singleton pattern on scene reload
        instance = null;
    }

    public void StartCheckRetreat()
    {
        // Start a coroutine for the delay using an instance of EventManager
        StartCoroutine(DelayedInvoke());
    }

    private IEnumerator DelayedInvoke()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
        CheckRetreat?.Invoke(); // Invoke the event
    }
}
