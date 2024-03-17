using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    static System.Random random = new System.Random();
    public string color;
    public int cellNumber;
    public bool alive;
    public bool dead;
    public bool special;
    public bool super;
    public bool kill;
    public bool specialGreenCounted;

    // GridManager
    private Transform cellTransform;
    private Transform fleetTransform;
    private GameObject fleet;
    private GridManager gridManagerScript;

    // Color
    ColorManager colorManager;
    private Dictionary<string, int> colorCounts;

    public Sprite specialSkin;

    // Neighbors
    private GameObject up = null;
    private GameObject left = null;
    private GameObject right = null;
    private GameObject down = null; 

    // Retreat
    public bool retreat = false;
    public int group = 1;
    List<GameObject> retreating = new List<GameObject> {};

    // RED ABILITIES
    private float timeSinceLastShot = 0;
    private double shotCoolDown;
    private float[] shotCoolDownRange = {4, 7};
    private bool onCoolDown = false;
    public GameObject missilePrefab;

    // BLUE ABILITIES
    public GameObject shieldPrefab;

    // GREEN ABILITIES
    public float specialMultiplier;

    void Awake()
    {
        alive = true;
        dead = false;

        // Color
        color = "";
        colorManager = ColorManager.Instance;
        colorCounts = colorManager.colorCounts;
        colorManager.SetColor(this.gameObject);

        special = false;
        super = false;
        kill = false;

        cellTransform = transform.parent;
        fleetTransform = cellTransform.parent;
        fleet = fleetTransform.gameObject;
        gridManagerScript = fleet.GetComponent<GridManager>();

        checkNeighbors();

    }

    void Start()
    {
        if (color == "Blue")
        {
            activateShield();
        }
    }

    void Update()
    {
        Abilities();
        if(Input.GetKeyDown(KeyCode.A))
        {
            // checkNeighbors();
            gridManagerScript.retreat(13);
        }
    }

    void Abilities()
    {
        // Check color
        if(color == "Red")
        {
            // Base ability
            if (!special)
            {
                fireMissile();
            }
            // Special ability
            else if (special)
            {

            }

        }
        else if(color == "Green")
        {
            // Special ability
            if (special)
            {
                if(specialGreenCounted == false)
                {
                    gridManagerScript.specialGreenCount++;
                    specialGreenCounted = true;
                }
            }
        }

        // for each
        // Base abilities

        // Special abilities

        // Super abilities
    }

     public void checkRetreat()
    {
        Debug.Log("checkretreat");
        // Check if there is no enemy in the above cell and the current enemy could retreat
        if (up.GetComponent<Cell>().enemy != null && up.GetComponent<Cell>().enemy.GetComponent<Enemy>().dead)
            {
                retreat = true;
            }
        // If enemy is not set to be killed and could retreat
        if(kill == false && retreat == true)
        {
            // Get instance of current grid
            List<GameObject> grid = gridManagerScript.grid;
            // Get instance of cell holding this enemy
            Transform cellTransform = transform.parent;
            // Get the cell number
            cellNumber = cellTransform.gameObject.GetComponent<Cell>().number;

            // Check left if not left column
            if (cellNumber % 6 != 0)
            {
                left = grid[cellNumber - 1].GetComponent<Cell>().enemy;
                if (left != null && left.GetComponent<Enemy>().retreat == false)
                {
                    left.GetComponent<Enemy>().checkRetreat();
                }
            }
            // Check right if not right column
            if ((cellNumber + 1) % 6 != 0)
            {
                right = grid[cellNumber + 1].GetComponent<Cell>().enemy;
                if (right != null && right.GetComponent<Enemy>().retreat == false)
                {
                    right.GetComponent<Enemy>().checkRetreat();
                }
            }
            // Check down if not in bottom row
            if (cellNumber < 66)
            {
                down = grid[cellNumber + 6];
            }
            gridManagerScript.retreat(cellNumber);
            retreat = false;
            if (cellNumber > 5)
            {
                checkRetreat();
            }

        }
    }
    public void checkNeighbors()
    {
        if(kill == false)
        {
            // Get instance of current grid
            List<GameObject> grid = gridManagerScript.grid;
            // Get instance of cell holding this enemy
            Transform cellTransform = transform.parent;
            // Get the cell number
            cellNumber = cellTransform.gameObject.GetComponent<Cell>().number;
            // Debug.Log("Checking Cell " + cellNumber + "'s neighbors");

            // Check the neighboring cells
            List<GameObject> neighbors = new List<GameObject>();

            // Debug.Log("Cell: " + cellNumber + ", " + "Color: " + color);

            // Check up if not bot row
            if (cellNumber > 5)
            {
                up = grid[cellNumber - 6];
                neighbors.Add(up);

            }
            // Check left if not left column
            if (cellNumber % 6 != 0)
            {
                left = grid[cellNumber - 1];
                neighbors.Add(left);
            }
            // Check right if not right column
            if ((cellNumber + 1) % 6 != 0)
            {
                right = grid[cellNumber + 1];
                neighbors.Add(right);
            }
            // Check down if not bot row
            if (cellNumber < 66)
            {
                down = grid[cellNumber + 6];
                neighbors.Add(down);
            }

            // List for matching colored neighbors
            List<GameObject> matches = new List<GameObject>();
            // Iterate through the neighboring cell occupants
            for (int j = 0; j < neighbors.Count; j++)
            {
                GameObject neighbor = neighbors[j];
                // If the neighbor is an enemy
                if (neighbor.GetComponent<Cell>().enemy)
                {
                    GameObject neighborEnemy = neighbor.GetComponent<Cell>().enemy;
                    string neighborColor = neighborEnemy.GetComponent<Enemy>().color;
                    // If neighbor is a like color add it to list of matches
                    if (neighborColor == color)
                    {
                        matches.Add(neighborEnemy);
                    }
                    if(!alive)
                    {
                        neighborEnemy.GetComponent<Enemy>().checkNeighbors();
                    }
                }
            }
            // Set Special
            if (matches.Count >= 2)
            {
                // Turn on currentEnemy's special
                if (!special)
                {
                    gameObject.GetComponent<Enemy>().special = true;
                    gameObject.GetComponent<SpriteRenderer>().sprite = specialSkin;
                }
                // Turn on the neighbor's specials
                for (int k = 0; k < matches.Count; k++)
                {
                    GameObject match = matches[k];
                    match.GetComponent<Enemy>().special = true;
                    match.GetComponent<SpriteRenderer>().sprite = specialSkin;
                }
            }

            // Kill protocol -- If current enemy !alive set matches to !alive
            if(!alive)
            {
                for (int k = 0; k < matches.Count; k++)
                {
                    GameObject match = matches[k];
                    match.GetComponent<Enemy>().alive = false;
                    kill = true;
                    match.GetComponent<Enemy>().checkNeighbors();
                }
            }
        }
        // Call death to kill current enemy
        death();
    }


    // ||| ABILITIES |||

    // RED

    // BASIC
    void fireMissile()
    {
        if(!onCoolDown)
        {
            shotCoolDown = random.NextDouble() * (shotCoolDownRange[1] - shotCoolDownRange[0]) + shotCoolDownRange[0];
            onCoolDown = true;
        }

        timeSinceLastShot += Time.deltaTime;

        if(timeSinceLastShot >= shotCoolDown)
        {
                // Debug.Log("shotColor: " + shotColor);
                // Debug.Log("color: " + color);
            colorCounts = colorManager.colorCounts;

            if (colorManager.colorSet)
            {
                // Spawn missile in front of enemy
                Instantiate(missilePrefab, new Vector3(transform.position.x, transform.position.y - .75f, transform.position.z), transform.rotation);
                // Reset cooldown
                timeSinceLastShot = 0.0f;
                onCoolDown = false;
            }
        }
    }

    // BLUE

    // BASIC
    void activateShield()
    {
        // Spawn a shield object around enemy
        GameObject shield = Instantiate(shieldPrefab, gameObject.transform);

    }


    public void death()
    {
        if(alive == false && dead == false)
        {

            // Set dead to true to prevent multiple runs in same frame
            dead = true;
            // Subtract color from colorCounts dictionary instance
            colorCounts = colorManager.colorCounts;
            colorCounts[color] -= 1;
            this.transform.parent.GetComponent<Cell>().color = null;

            // Check if current color count is 0
            if (colorCounts[color] == 0)
            {
                gridManagerScript.FleetWipeCheck();
                if (!gridManagerScript.wipedOut)
                {
                    colorManager.magicLaser = true;
                }
            }

            // Subract special green count
            if (specialGreenCounted)
            {
                gridManagerScript.specialGreenCount--;
                specialGreenCounted = false;
            }
            
            Destroy(this.gameObject);

        }
    }

}
