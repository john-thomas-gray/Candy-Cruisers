using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public bool special = false;
    public Sprite specialSprite;
    SpriteRenderer spriteRenderer;
    public Color originalColor;
    private bool deflectorUp = false;

    public string shieldEnemyColor;


    // Current Enemy
    private GameObject enemy;
    private Enemy enemyScript;
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        // Shrink gameObject to 1% instantly
        StartCoroutine(ShieldSize(0.01f, 0.001f));
        StartCoroutine(ShieldSize(1.25f, 0.25f)); // Grow over 2 seconds
        originalColor = spriteRenderer.color;
        // Set the variable to the GameObject's parent
        if (transform.parent != null)
        {
            enemy = transform.parent.gameObject;
            enemyScript = enemy.GetComponent<Enemy>();
            shieldEnemyColor = enemyScript.color;
        }
        else
        {
            Debug.LogWarning("This shield has no enemy.");
        }

    }
    void Update()
    {
        if(enemyScript.special == true)
        {
            special = true;
            if (deflectorUp == false)
            {
                deflector();
            }
        }
        else
        {
            special = false;
        }

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

        StartCoroutine(ShieldSize (1.5f, .5f));
        // spriteRenderer.color = Color.white;
        if(this.gameObject.transform.localScale.x >= 1.5f)
        {
            StartCoroutine(ShieldSize (1.25f, .10f));
            // spriteRenderer.color = originalColor;
        }

    }

    public void deflector()
    {
        // Debug.Log("DEFLECTOR");
        // spriteRenderer.sprite = specialSprite;
        // StartCoroutine(ShieldSize (1.5f, .1f));

        if(deflectorUp == false)
        {   // Shrink current shield
            StartCoroutine(ShieldSize (0f, .25f));
            if (this.gameObject.transform.localScale.x <= 0f)
            {
                // Change shield shape and color
                spriteRenderer.sprite = specialSprite;
                // Grow shield
                StartCoroutine(ShieldSize (1.5f, .3f));
                deflectorUp = true;
            }
        }

    }
}
