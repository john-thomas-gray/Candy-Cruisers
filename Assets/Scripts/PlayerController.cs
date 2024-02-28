using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool colorSet;
    public string shotColor;
    public string color;
    public bool alive;
    public float tankSpeed;
    private float screenBoundLeft = -2.45f;
    private float screenBoundRight = 2.45f;
    public GameObject laserPrefab;

    private GameObject fleet;
    private GridManager gridManagerInstance;
    private Dictionary<string, int> colorCounts;
    ColorManager colorManager;
    private bool wipedOut;


    void Awake()
    {
        alive = true;
        tankSpeed = 6f;
        // Get fleet GO and Grid script
        GameObject fleet = GameObject.Find("Fleet");
        gridManagerInstance = fleet.GetComponent<GridManager>();
        // Get ColorManager instance
        colorManager = ColorManager.Instance;
        // Create a reference to the colorCounts dictionary
        colorCounts = colorManager.colorCounts;
        // Create a reference to the wipedOut int
        wipedOut = gridManagerInstance.wipedOut;

    }

    void Start()
    {
        SetColor();
    }
    // Update is called once per frame
    void Update()
    {
        if(alive)
        {
            playerMovement();
            fireLaser();
        }
    }

    void playerMovement()
    {
        // Move Left
        if(Input.GetKey(KeyCode.LeftArrow) && transform.position.x > screenBoundLeft)
        {
            transform.position = transform.position + Vector3.left * tankSpeed * Time.deltaTime;
        }
        // Move Right
        else if(Input.GetKey(KeyCode.RightArrow) && transform.position.x < screenBoundRight)
        {
            transform.position = transform.position + Vector3.right * tankSpeed * Time.deltaTime;
        }
}
    void fireLaser()
    {
        if(Input.GetKey(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
        {
                // Debug.Log("shotColor: " + shotColor);
                // Debug.Log("color: " + color);
            colorCounts = colorManager.colorCounts;
            if (colorSet)
            {
                // Set independent shotColor variable, so missile can reference it once player changes colors
                shotColor = color;
                // Spawn laser in front of player
                Instantiate(laserPrefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation);
                // Make player a different color
                SetColor();
            }
        }
    }

    public void SetColor()
    {
        // Reference the player object's SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // Initialize color reference list and a list of color strings
        Color purple = new Color(1f, 0f, 1f, 1f);
        List<Color> skins = new List<Color> {Color.red, Color.yellow, Color.blue, Color.green, purple};
        List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };
        // Set colorCounts to current values
        colorCounts = colorManager.colorCounts;

        //loop to set color
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
                Debug.Log("Color set: " + color);
            }
            else
            {
                // Remove the used color
                skins.Remove(skin);
                colors.Remove(color);
                Debug.Log(color + " not found");
            }
            recursionLimit--;
            if(recursionLimit == 0)
            {
                Debug.Log("SetColor() timed out!");
            }
        }

    }
}
