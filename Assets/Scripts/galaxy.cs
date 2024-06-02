using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float rotationSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the game object around the z-axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
