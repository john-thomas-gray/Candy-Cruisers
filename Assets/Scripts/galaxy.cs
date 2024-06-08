using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float rotationSpeed = 5f;
    public IntEventChannelSO updateGlobalLevelChannel;

    // Movement variables
    public float moveSpeed = 2f; // Speed of the vertical movement
    public float moveAmplitude = .5f; // Amplitude of the vertical movement

    private float originalY;

    private void OnEnable()
    {
        updateGlobalLevelChannel.OnEventRaised += RaiseRotationSpeed;
    }

    private void OnDisable()
    {
        updateGlobalLevelChannel.OnEventRaised -= RaiseRotationSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the game object around the z-axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Move the game object up and down
        float newY = originalY + Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void RaiseRotationSpeed(int level)
    {
        rotationSpeed = rotationSpeed + 2f * level;
        moveSpeed = moveSpeed + 0.1f;
    }
}
