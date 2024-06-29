using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorManager : MonoBehaviour
{
    static System.Random random = new System.Random();
    private static ColorManager colorManager;
    public bool colorSet;

    // Multicolor
    public bool magicTongue = false;
    private float multicolorTimer = 0.0f;
    private float multicolorCooldown = 0.05f;
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

    public void SetColor(GameObject targetObject, string color = null)
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

        // Check if the object is not the player
        if (targetObject.tag != "Player")
        {
            if (color == null)
            {
                // Set color string to a random "color"
                int randomIndex = random.Next(colors.Length);
                color = colors[randomIndex];
            }

            targetObject.tag = color;
            // Get index of item in list
            int colorInx = Array.IndexOf(colors, color);

            // Set the target sprite's color
            Color spriteColor = skins[colorInx];
            spriteRenderer.color = spriteColor;

            // Set color property
            if(targetObject.GetComponent<Enemy>())
            {
                targetObject.GetComponent<Enemy>().color = color;
                colorCounts[color] += 1;
            }
            else
            {
                targetObject.GetComponent<Tongue>().color = color;
            }
        }
        else // Set player color
        {
            if (color == null)
            {
                List<string> onScreenColors = new List<string> {};
                foreach (var kvp in colorCounts)
                    {
                        if (kvp.Value > 0)
                            {
                                onScreenColors.Add(kvp.Key);
                            }
                    }
                if (onScreenColors.Count > 0)
                {
                    int randomIndex = random.Next(onScreenColors.Count);
                    spriteRenderer.color = skins[Array.IndexOf(colors, onScreenColors[randomIndex])];
                    color = onScreenColors[randomIndex];
                }
                else // if there are no enemies on screen, use fallback
                {
                    color = "Red";
                }
            }
            colorSet = true;

            targetObject.GetComponent<PlayerController>().color = color;
        }
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
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        // Set initial color value so IndexOf doesn't throw error
        string color = "Green";

        // Check if the object is not the player
        if (targetObject.tag != "Player")
        {
            multicolorTimer += Time.deltaTime;
            if (multicolorTimer >= multicolorCooldown)
            {
                // Set color string to a random "color"
                int randomIndex = random.Next(colors.Length);
                color = colors[randomIndex];
                multicolorTimer = 0.0f;
            }

            // targetObject.tag = 'Multicolor';

            // Get index of item in list
            int colorInx = Array.IndexOf(colors,color);

            // Set the target sprite's color
            Color spriteColor = skins[colorInx];
            spriteRenderer.color = spriteColor;

            // Set color property
            targetObject.GetComponent<Tongue>().color = "Multicolor";

        }
        else
        {

            multicolorTimer += Time.deltaTime;
            if (multicolorTimer >= multicolorCooldown)
            {
                // Set color string to a random "color"
                int randomIndex = random.Next(colors.Length);
                color = colors[randomIndex];
                Color skin = skins[randomIndex];
                multicolorTimer = 0.0f;
                spriteRenderer.color = skin;
            }

        }

    }

    public void turnWhite(GameObject targetObject)
    {
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        targetObject.GetComponent<Tongue>().color = "White";
        spriteRenderer.color = Color.white;
    }


    // Ensure the colorManager is destroyed when the application quits
    private void OnApplicationQuit()
    {
        colorManager = null;
    }

}
