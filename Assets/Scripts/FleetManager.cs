// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// // This is used to create a Singleton Pattern
// public class FleetManager : MonoBehaviour
// {
//     private static FleetManager instance;
//     public GameObject fleet;

//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     public static FleetManager Instance
//     {
//         get { return instance; }
//     }
// }

// // Use this on other scripts

// void Awake()
// {

//     FleetManager fleetManager = FleetManager.Instance;

//     if (fleetManager != null)
//     {
//         fleet = fleetManager.fleet;
//         gridManagerInstance = fleet.GetComponent<GridManager>();
//     }
//     else
//     {
//         Debug.LogError("FleetManager not found in Awake.");
//     }
// }
