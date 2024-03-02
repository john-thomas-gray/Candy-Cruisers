using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public bool special = false;
    SpriteRenderer spriteRenderer;
    public Color originalColor;
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        // Shrink gameObject to 1% instantly
        StartCoroutine(ShieldSize(0.01f, 0.001f));
        StartCoroutine(ShieldSize(1.25f, 0.25f)); // Grow over 2 seconds
        originalColor = spriteRenderer.color;
    }

    IEnumerator ShieldSize(float targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);

        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScaleVector, elapsedTime / duration);
            elapsedTime = Time.time - startTime;
            yield return null; // Wait for the next frame
        }

        transform.localScale = targetScaleVector; // Ensure the final scale is set
    }

    public void absorb()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
        Debug.Log("ABSORB");
        bool absorbing = true;
        if(absorbing)
        {
            StartCoroutine(ShieldSize (1.2f, .5f));
            spriteRenderer.color = Color.white;

        }
        // if(absorbing == false)
        // {
        //     StartCoroutine(ShieldSize (1.25f, .10f));
        //     spriteRenderer.color = originalColor;
        // }

    }
}
