using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    static System.Random random = new System.Random();
    private static ColorManager colorManager;
    public bool colorSet;

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
        Color purple = new Color(1f, 0f, 1f, 1f);
        List<Color> skins = new List<Color> { Color.red, Color.yellow, Color.blue, Color.green, purple };
        List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };

        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        // Check if the object is not the player
        if (targetObject.tag != "Player")
        {

            if (color == null)
            {
                // Set color string to a random "color"
                int randomIndex = random.Next(colors.Count);
                color = colors[randomIndex];
            }

            targetObject.tag = color;


            // Get index of item in list
            int colorInx = colors.IndexOf(color);

            // Set the target sprite's color
            Color spriteColor = skins[colorInx];
            spriteRenderer.color = spriteColor;
        }
        else
        {
            colorSet = false;
            int recursionLimit = 5;
            // Randomly iterate through all possible colors
            while(colorSet == false && recursionLimit > 0)
                {
                    // Get a random num
                    int randomIndex = random.Next(colors.Count);
                    // Index skins for a random color value
                    Color skin = skins[randomIndex];
                    // Index colors list for a random color
                    color = colors[randomIndex];

                    // If color shares an onscreen enemy's color
                    if (colorCounts[color] > 0)
                    {
                        // Set sprite color to skin
                        spriteRenderer.color = skin;
                        colorSet = true;
                        // Debug.Log("Color set: " + color);
                    }
                    else
                    {
                        // Remove the used color
                        skins.Remove(skin);
                        colors.Remove(color);
                        // Debug.Log(color + " not found");
                    }
                    recursionLimit--;
                    if(recursionLimit == 0)
                    {
                        Debug.Log("SetColor() timed out!");
                    }
                }
        }
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
