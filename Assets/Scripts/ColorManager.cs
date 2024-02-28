using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    static System.Random random = new System.Random();
    private static ColorManager colorManager;

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
    List<Color> skins = null;
    List<string> colors = null;

    void Awake()
    {
        skins = new List<Color> { Color.red, Color.yellow, Color.blue, Color.green, purple };
        colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };
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
        // Color purple = new Color(1f, 0f, 1f, 1f);
        // List<Color> skins = new List<Color> { Color.red, Color.yellow, Color.blue, Color.green, purple };
        // List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };

        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (color == null)
        {
            // Set color string to a random "color"
            int randomIndex = random.Next(colors.Count);
            color = colors[randomIndex];
        }


        // Set the object's tag to the appropriate color
        // THIS CURRENTLY DOESN'T WORK 2.28.24
        if (this.gameObject.tag != "Player")
        {
            this.gameObject.tag = color;
        }

        // Get index of item in list
        int colorInx = colors.IndexOf(color);

        // Set the target sprite's color
        Color spriteColor = skins[colorInx];
        spriteRenderer.color = spriteColor;
    }

    public void UpdateColorCounts(Dictionary<string, int> updateDictionary)
    {
        Debug.Log("UpdateColorCounts");
        colorCounts = updateDictionary;
    }


    // Ensure the colorManager is destroyed when the application quits
    private void OnApplicationQuit()
    {
        colorManager = null;
    }
}
