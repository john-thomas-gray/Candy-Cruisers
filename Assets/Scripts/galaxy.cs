using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Galaxy : MonoBehaviour
{
    public IntEventChannelSO updateGlobalLevelChannel;
    // Movement variables
    public float rotationSpeed = 5f;

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

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            float newY = originalY + Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Would be cool to switch rotation direction
    void RaiseRotationSpeed(int level)
    {
        float newRotationSpeed;

        if (level % 2 == 0)
        {
            newRotationSpeed = -(Mathf.Abs(rotationSpeed) + 2f * level);
        }
        else
        {
            newRotationSpeed = Mathf.Abs(rotationSpeed) + 2f * level;
        }

        StartCoroutine(ChangeRotationSpeedGradually(newRotationSpeed, 1f));
    }

    private IEnumerator ChangeRotationSpeedGradually(float targetSpeed, float duration)
    {
        float startSpeed = rotationSpeed;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            rotationSpeed = Mathf.Lerp(startSpeed, targetSpeed, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        rotationSpeed = targetSpeed;
    }
}
