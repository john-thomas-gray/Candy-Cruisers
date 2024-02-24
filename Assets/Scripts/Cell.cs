using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private string _name = "";
    public int number;
    public bool occupied = false;
    public string color = null;
    public GameObject enemy;

    // Property to get and set the name
    new public string name
    {
        get { return _name; }
        set
        {
            _name = value;
            UpdateGameObjectName();
        }
    }

    // Method to update the GameObject's name
    private void UpdateGameObjectName()
    {
        gameObject.name = _name;
    }

}
