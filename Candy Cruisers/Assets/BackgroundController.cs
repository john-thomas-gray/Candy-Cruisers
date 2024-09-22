using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [Header("Background Images")]
    public List<Sprite> backgroundImages;
    private SpriteRenderer spriteRenderer;
    public IntEventChannelSO updateGlobalLevelChannel;
    public int count = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetActiveImage(count);
            count++;
        }
    }
    private void OnEnable()
    {
        updateGlobalLevelChannel.OnEventRaised += SetActiveImage;
    }

    private void OnDisable()
    {
        updateGlobalLevelChannel.OnEventRaised -= SetActiveImage;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (backgroundImages == null || backgroundImages.Count == 0)
        {
            Debug.LogError("No images assigned to the backgroundImages list.");
        }
    }

    public void SetActiveImage(int index)
    {
        if (index >= 0 && index < backgroundImages.Count)
        {
            StartCoroutine(FadeImageIn(backgroundImages[index], 1f)); // Fade in over 1 second
        }
    }

    private IEnumerator FadeImageIn(Sprite newImage, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = spriteRenderer.color;

        // Set the new image, but make it transparent first
        spriteRenderer.sprite = newImage;
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0); // Set alpha to 0

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha); // Gradually increase alpha
            yield return null;
        }

        // Ensure it's fully visible at the end
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
    }

}
