using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    static System.Random random = new System.Random();
    public string color;
    public bool alive;
    public bool dead;
    public bool special;
    public bool super;
    public bool isChecked;

    private Transform cellTransform;
    private Transform fleetTransform;
    private GameObject fleet;
    private GridManager gridManagerInstance;
    private Dictionary<string, int> colorCounts;

    public bool allDead;

    void Awake()
    {
        alive = true;
        dead = false;
        SetColor();
        special = false;
        super = false;
        isChecked = false;

        cellTransform = transform.parent;
        fleetTransform = cellTransform.parent;
        fleet = fleetTransform.gameObject;
        gridManagerInstance = fleet.GetComponent<GridManager>();
        colorCounts = gridManagerInstance.colorCounts;

    }


    private void SetColor()
    {
        Color purple = new Color(1f, 0f, 1f, 1f);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // !! MIGHT NEED TO CHANGE THE SCOPE OF THESE !!
        List<Color> skins = new List<Color> {Color.red, Color.yellow, Color.blue, Color.green, purple};
        List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };
        // Set color string to a random "color"
        int randomIndex = random.Next(colors.Count);
        color = colors[randomIndex];
        // Set the enemy's tag to the appropriate color
        this.gameObject.tag = color;
        // Set the sprite's color
        Color spriteColor = skins[randomIndex];
        spriteRenderer.color = spriteColor;
    }

    void Abilities()
    {
        // Check color
        if(color == "Red")
        {
            // Base ability
            if (!special)
            {

            }
            // Special ability
            else if (special)
            {

            }
            else if (super)
            {

            }
        }
        // for each
        // Base abilities

        // Special abilities

        // Super abilities
    }


    public void CheckNeighbors()
    {
        if(isChecked == false)
        {
            // Get instance of current grid
            List<GameObject> grid = gridManagerInstance.grid;
            // Get instance of cell holding this enemy
            Transform cellTransform = transform.parent;

            // Get the cell number
            int cellNumber = cellTransform.gameObject.GetComponent<Cell>().number;
            // Debug.Log("Checking Cell " + cellNumber + "'s neighbors");
            // Check the neighboring cells
            List<GameObject> neighbors = new List<GameObject>();

            // Check up if not bot row
            if (cellNumber > 5)
            {
                GameObject up = grid[cellNumber - 6];
                neighbors.Add(up);
            }
            // Check left if not left column
            if (cellNumber % 6 != 0)
            {
                GameObject left = grid[cellNumber - 1];
                neighbors.Add(left);
            }
            // Check right if not right column
            if ((cellNumber + 1) % 6 != 0)
            {
                GameObject right = grid[cellNumber + 1];
                neighbors.Add(right);
            }
            // Check down if not bot row
            if (cellNumber < 66)
            {
                GameObject down = grid[cellNumber + 6];
                neighbors.Add(down);
            }
            // List enemies with matching colors
                List<GameObject> matches = new List<GameObject>();
                for (int j = 0; j < neighbors.Count; j++)
                {
                    GameObject neighbor = neighbors[j];
                    if (neighbor.GetComponent<Cell>().enemy)
                    {
                        GameObject neighborEnemy = neighbor.GetComponent<Cell>().enemy;
                        string neighborColor = neighborEnemy.GetComponent<Enemy>().color;
                        // If neighbor is a like color add it to list of matches
                        if (neighborColor == color)
                        {
                            matches.Add(neighborEnemy);
                        }
                    }
                }
                // Set matches to !alive
                for (int k = 0; k < matches.Count; k++)
                {
                    GameObject match = matches[k];
                    match.GetComponent<Enemy>().alive = false;
                    isChecked = true;
                    match.GetComponent<Enemy>().CheckNeighbors();
                }
        }
        // Call death to kill current enemy
        Death();
    }

    public void Death()
    {
        if(alive == false && dead == false)
        {

            // Set dead to true to prevent multiple runs in same frame
            dead = true;
            // Subtract color from colorCounts dictionary instance
            colorCounts = gridManagerInstance.colorCounts;
            colorCounts[color] -= 1;

            // Check if current color count is 0
            if (colorCounts[color] == 0)
            {
                gridManagerInstance.FleetWipeCheck();
                // gridManagerInstance.allDead = true;
                if (!gridManagerInstance.wipedOut)
                {
                    Debug.Log("magic bullet");
                }
            }

            // Check if all color counts are 0
            // Give magic bullet
            // Update colorCounts dictionary
            fleet.GetComponent<GridManager>().colorCounts = colorCounts;
            // Destroy gameObject
            Destroy(this.gameObject);

        }
    }

}
