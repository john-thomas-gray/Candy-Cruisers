using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorWheel : MonoBehaviour
{
    [Header("Color Wedges")]
    public GameObject redWedge;
    public GameObject yellowWedge;
    public GameObject blueWedge;
    public GameObject greenWedge;
    public GameObject purpleWedge;

    public StringEventChannelSO SetColorWheelWedgeChannel;

    private void OnEnable()
    {
        SetColorWheelWedgeChannel.OnEventRaised += SetColorWheelWedge;
    }
    private void OnDisable()
    {
        SetColorWheelWedgeChannel.OnEventRaised -= SetColorWheelWedge;
    }

    private void SetColorWheelWedge(string color)
    {
        switch (color)
        {
            case "Red":
                redWedge.SetActive(true);
                break;
            case "Yellow":
                yellowWedge.SetActive(true);
                break;
            case "Blue":
                blueWedge.SetActive(true);
                break;
            case "Green":
                greenWedge.SetActive(true);
                break;
            case "Purple":
                purpleWedge.SetActive(true);
                break;
        }
    }

    public void Deactivate()
    {
        redWedge.SetActive(false);
        yellowWedge.SetActive(false);
        blueWedge.SetActive(false);
        greenWedge.SetActive(false);
        purpleWedge.SetActive(false);
    }

    public void Bonus()
    {
        Spin();
    }

    public void Spin()
    {
        Debug.Log("Spin");
        StartCoroutine(RotateOverTime(1f));
    }

    private IEnumerator RotateOverTime(float duration)
    {
        Debug.Log("Start RotateOverTime");
        float initialZRotation = transform.eulerAngles.z; // Start from the current Z rotation
        float targetZRotation = initialZRotation + 1080;  // Target an additional 1080 degrees
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            float currentZRotation = Mathf.Lerp(initialZRotation, targetZRotation, normalizedTime);

            // Apply the new rotation incrementally on the Z axis
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, currentZRotation);
            Debug.Log($"Rotating to {currentZRotation} degrees at t={elapsedTime}");
            yield return null;
        }

        // Ensure the final rotation is exactly the target value
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetZRotation);
        Debug.Log("End Rotation: " + transform.rotation.eulerAngles.z);
        Deactivate();
    }



}
