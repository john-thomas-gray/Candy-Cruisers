using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorManager : MonoBehaviour
{
    static System.Random random = new System.Random();
    private static ColorManager colorManager;

    // Multicolor

    private float multicolorTimer = 0.0f;
    private float multicolorCooldown = 0.1f;
    public static ColorManager Instance
    {
        get
        {
            // If the colorManager doesn't exist, create it
            if (colorManager == null)
            {
                GameObject colorManagerObject = new GameObject("ColorManager");
                colorManager = colorManagerObject.AddComponent<ColorManager>();
            }

            return colorManager;
        }
    }
    // Color dictionary
    public Dictionary<string, int> colorCounts = null;
    Color purple = new Color(1f, 0f, 1f, 1f);
    Color[] skins = null;
    string[] colors = null;
    string multicolorColor = "Green";

    void Awake()
    {
        skins = new Color[5] { Color.red, Color.yellow, Color.blue, Color.green, purple };
        colors = new string[5] { "Red", "Yellow", "Blue", "Green", "Purple" };
        // Set initial color dictionary
        colorCounts = new Dictionary<string, int>
            {
                { "Red", 0 },
                { "Yellow", 0 },
                { "Blue", 0 },
                { "Green", 0 },
                { "Purple", 0 }
            };
    }

    public string RandomOnscreenColor()
    {
        string[] colors = new string[5] {
            "Red",
            "Yellow",
            "Blue",
            "Green",
            "Purple"
            };

        string randomColor = null;
        int fallback = 0;
        int randomIndex = 0;
        while(fallback <= 20 && randomColor == null)
        {
            randomIndex = random.Next(colors.Length);
            if(colorCounts[colors[randomIndex]] > 0)
            {
                randomColor = colors[randomIndex];
                return randomColor;
            }
            fallback++;
        }

        randomIndex = random.Next(colors.Length);
        return colors[randomIndex];

    }

    public void SetColor(GameObject targetObject, string inputColor = null)
    {
        Color purple = new Color(1f, 0f, 1f, 1f);
        Color[] skins = new Color[5] {
                        Color.red,
                        Color.yellow,
                        Color.blue,
                        Color.green,
                        purple
                        };

        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        spriteRenderer.color = skins[Array.IndexOf(colors, inputColor)];

        if(targetObject.GetComponent<PlayerController>())
        {
            targetObject.GetComponent<PlayerController>().color = inputColor;
        }
        else if(targetObject.GetComponent<Tongue>())
        {
            targetObject.GetComponent<Tongue>().color = inputColor;
        }
    }

    public void SetEnemyColor(GameObject targetObject, string color = null, bool imitation = false, bool specialImitation = false)
    {
        Color purple = new Color(1f, 0f, 1f, 1f);
        Color[] skins = new Color[5] {
                        Color.red,
                        Color.yellow,
                        Color.blue,
                        Color.green,
                        purple
                        };
        string[] colors = new string[5] {
            "Red",
            "Yellow",
            "Blue",
            "Green",
            "Purple"
            };

        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (color == null)
        {
            // Set color string to a random "color"
            int randomIndex = random.Next(colors.Length);
            color = colors[randomIndex];
            // DEVTOOL
            // color = "Yellow";
        }


        // Set color property
        if(targetObject.GetComponent<Enemy>() && !specialImitation)
        {
            targetObject.tag = color;
            targetObject.GetComponent<Enemy>().color = color;
            colorCounts[color] += 1;
        }
        else
        {
            Debug.LogWarning("ColorManager: No enemy found.");
        }
        if (imitation)
        {
            colorCounts["Yellow"] -= 1;
        }
        // Get index of item in list
        int colorInx = Array.IndexOf(colors, color);
        // Set the target sprite's color
        Color spriteColor = skins[colorInx];
        spriteRenderer.color = spriteColor;

    }


    public void UpdateColorCounts(Dictionary<string, int> updateDictionary)
    {
        Debug.Log("UpdateColorCounts");
        colorCounts = updateDictionary;
    }

    public int totalEnemyCount()
    {
        int enemyTotal = 0;

        Debug.Log("ALL ENEMIES:");
        foreach (var kvp in colorCounts)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
        foreach (var kvp in colorCounts)
        {
            enemyTotal += kvp.Value;
            // Log color totals

        }
        Debug.Log("total enemies: " + enemyTotal);

        return enemyTotal;
    }


    public void Multicolor(GameObject targetObject)
    {
        // SOME OF THIS IS LIKELY UNNECESSARY. REVISIT.
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        multicolorTimer += Time.deltaTime;

        int colorInx = Array.IndexOf(colors,multicolorColor);

        Color spriteColor = skins[colorInx];
        spriteRenderer.color = spriteColor;

        if (targetObject.tag != "Player")
        {
            targetObject.GetComponent<Tongue>().color= "Multicolor";
        }
        multicolorTimer += Time.deltaTime;
        if (multicolorTimer >= multicolorCooldown)
        {
            // Set color string to a random "color"
            int randomIndex = random.Next(colors.Length);
            multicolorColor = colors[randomIndex];
            Color skin = skins[randomIndex];
            multicolorTimer = 0.0f;
            spriteRenderer.color= skin;
        }
    }

    public void turnWhite(GameObject targetObject)
    {
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        targetObject.GetComponent<PlayerController>().color = "White";
        spriteRenderer.color = Color.white;
    }


    // Ensure the colorManager is destroyed when the application quits
    private void OnApplicationQuit()
    {
        colorManager = null;
    }

}
