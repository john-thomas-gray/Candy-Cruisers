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
    private bool onCoolDown = false;
    private bool deactivated = false;
    private float reactivationTime = 0;
    private double abilityCoolDown;
    private float[] reactivateCoolDownRange = { 25, 45 };
    public string shieldEnemyColor;

    static System.Random random = new System.Random();

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

        reactivate();

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

    public void deactivate()
    {
        deactivated = true;
        spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
        StartCoroutine(ShieldSize (0.01f, 0.001f));
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

    public void reactivate()
    {
        if (deactivated)
        {
            if (!onCoolDown)
            {
                abilityCoolDown = random.NextDouble() * (reactivateCoolDownRange[1] - reactivateCoolDownRange[0]) + reactivateCoolDownRange[0];
                // Decrease cooldown based on globalLevel
                enemy = transform.parent.gameObject;
                enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    abilityCoolDown -= (2.5f * (enemyScript.globalLevel) - 1);
                }
                if (abilityCoolDown < 0)
                {
                    abilityCoolDown = 1;
                }
                onCoolDown = true;
            }

            reactivationTime += Time.deltaTime;

            if (reactivationTime >= abilityCoolDown)
            {
                StartCoroutine(ShieldSize(1.25f, 0.25f)); // Grow over 2 seconds
                reactivationTime = 0.0f;
                onCoolDown = false;
                deactivated = false;
            }
        }

    }
}
