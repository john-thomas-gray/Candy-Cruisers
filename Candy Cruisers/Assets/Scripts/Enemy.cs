using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    static System.Random random = new System.Random();
    public string color = "";
    public int cellNumber;
    public bool alive;
    public bool dead;
    public bool special;
    public bool super;
    public bool kill;
    public bool specialGreenCounted;

    [Header ("Event Channels")]
    public IntEventChannelSO updateGlobalLevelChannel;
    // Broadcasting
    public VoidEventChannelSO checkRetreatEventChannel;
    public BoolEventChannelSO SetMagicTongueChannel;
    public StringEventChannelSO SetColorWheelWedge;

    // Animation
    private SquashAndStretch squashAndStretch;

    // GridManager
    private Transform cellTransform;
    private Transform fleetTransform;
    private GameObject fleet;
    private GridManager gridManagerScript;

    [Header("Scoring")]
    [SerializeField]
    private ScoreManagerSO scoreManager;
    public int globalLevel = 1;

    // Color
    ColorManager colorManager;
    private Dictionary<string, int> colorCounts;

    public Sprite specialSkin;
    public Sprite yellowSkin;
    public Sprite greenSkin;
    public Sprite blueSkin;
    public Sprite purpleSkin;

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
    private float timeSinceLastActivation = 0;
    private double abilityCoolDown;
    private float[] shotCoolDownRange = {4, 8};
    private bool onCoolDown = false;
    public GameObject missilePrefab;
    public GameObject homingMissilePrefab;

    // BLUE ABILITIES
    public GameObject shieldPrefab;
    public bool shieldInitiated = false;

    // GREEN ABILITIES
    public float specialMultiplier;

    // PURPLE ABILITIES
    private float[] warpCoolDownRange = {10, 30};
    public bool warpedIn = false;
    private bool colorReset = false;

    // YELLOW ABILITIES
    private float[] imitateCoolDownRange = {10, 18};
    // Cooldown becomes shorter for special yellow
    public bool isImitation = false;
    bool inMiddle = false;
    public string imitationColor;


    private void OnEnable()
    {
        updateGlobalLevelChannel.OnEventRaised += updateGlobalLevel;

    }
    private void OnDisable()
    {
        updateGlobalLevelChannel.OnEventRaised -= updateGlobalLevel;
    }

    void updateGlobalLevel(int level)
    {
        globalLevel = level;
    }
    void Awake()
    {
        alive = true;
        dead = false;
        // Color
        colorManager = ColorManager.Instance;
        colorCounts = colorManager.colorCounts;
        colorManager.SetEnemyColor(this.gameObject);
        if(color == "Yellow")
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = yellowSkin;
        }
        if(color == "Green")
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = greenSkin;
        }
        if(color == "Purple")
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = purpleSkin;
        }
        if(color == "Blue")
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = blueSkin;
        }

        squashAndStretch = GetComponent<SquashAndStretch>();

        special = false;
        super = false;
        kill = false;

        cellTransform = transform.parent;
        fleetTransform = cellTransform.parent;
        fleet = fleetTransform.gameObject;
        gridManagerScript = fleet.GetComponent<GridManager>();
        updateGlobalLevel(fleet.GetComponent<GridManager>().globalLevel);
        checkNeighbors();

    }

    void FixedUpdate()
    {
        Abilities();
        // BANDAID - the problem is that this adds to the color dictionary without subtracting the changed color from the dictionary
        if(warpedIn && !colorReset)
        {
            colorManager.SetEnemyColor(this.gameObject, color);
            colorReset = true;
        }
        // DEVTOOL FIRE MISSILES
        if(Input.GetKeyDown(KeyCode.Q) &&  color == "Red")
        {
            Instantiate(missilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        }
    }

    void Abilities()
    {
        if(color == "Red")
        {
            ability(fireMissile, shotCoolDownRange);
        }
        else if(color == "Green")
        {
            if (special)
            {
                if(specialGreenCounted == false)
                {
                    gridManagerScript.specialGreenCount++;
                    specialGreenCounted = true;
                }
            }
        }
        else if(color == "Purple")
        {
            ability(beamIn, warpCoolDownRange);
        }
        else if(color == "Yellow")
        {
            ability(imitate, imitateCoolDownRange);
        }
        else if(shieldInitiated == false && color == "Blue")
        {
            shieldInitiated = true;
            activateShield();
        }
    }
    public void checkNeighbors()
    {
        if(kill == false)
        {
            gridManagerScript.neighborsChecked++;
            // Get instance of current grid
            GameObject[] fleetGrid = gridManagerScript.fleetGrid;
            // Get instance of cell holding this enemy
            Transform cellTransform = transform.parent;
            // Get the cell number
            cellNumber = cellTransform.gameObject.GetComponent<Cell>().number;
            // Debug.Log("Checking Cell " + cellNumber + "'s neighbors");

            // Check the neighboring cells
            List<GameObject> neighbors = new List<GameObject>();

            // Check up if not bot row
            if (cellNumber > 5)
            {
            up = fleetGrid[cellNumber - 6];
                neighbors.Add(up);

            }
            // Check left if not left column
            if (cellNumber % 6 != 0)
            {
                left = fleetGrid[cellNumber - 1];
                neighbors.Add(left);
            }
            // Check right if not right column
            if ((cellNumber + 1) % 6 != 0)
            {
                right = fleetGrid[cellNumber + 1];
                neighbors.Add(right);
            }
            // Check down if not bot row
            if (cellNumber < fleetGrid.Length - 6)
            {
                down = fleetGrid[cellNumber + 6];
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
        death();
    }


    // ||| ABILITIES |||
    void ability(Action function, float[] cooldownRange)
    {
        if(!onCoolDown)
        {
            abilityCoolDown = random.NextDouble() * (cooldownRange[1] - cooldownRange[0]) + cooldownRange[0];
            abilityCoolDown -= (0.5f * (globalLevel) - 1);
            if (abilityCoolDown < 0)
            {
                abilityCoolDown = 1;
            }
            onCoolDown = true;
            squashAndStretch.CheckForAndStartCoroutine();
        }

        timeSinceLastActivation += Time.deltaTime;

        if(timeSinceLastActivation >= abilityCoolDown)
        {
            function();
            timeSinceLastActivation = 0.0f;
            onCoolDown = false;
        }
    }

    // RED
    void fireMissile()
    {
        if (special == false)
        {
            Instantiate(missilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        }
        else
        {
            Instantiate(homingMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        }
    }

    // BLUE
    void activateShield()
    {
        // Spawn a shield object around enemy
        GameObject shield = Instantiate(shieldPrefab, gameObject.transform);

    }

    // PURPLE
    void beamIn()
    {
        gridManagerScript.beamIn(gameObject);
    }

    // YELLOW
    public void imitate()
    {
        if(!inMiddle && !isImitation)
        {
            GameObject[] fleetGrid = gridManagerScript.fleetGrid;
            Transform cellTransform = transform.parent;
            cellNumber = cellTransform.gameObject.GetComponent<Cell>().number;
            GameObject[] neighboringCells = new GameObject[4];

            void addNeighbor(int index, int neighborIndex)
            {
                if (neighborIndex >= 0 && neighborIndex < fleetGrid.Length)
                {
                    neighboringCells[index] = fleetGrid[neighborIndex];
                }
                else
                {
                    neighboringCells[index] = null;
                }
            }

            addNeighbor(0, cellNumber > 5 ? cellNumber - 6 : -1); // up
            addNeighbor(1, cellNumber % 6 != 0 ? cellNumber - 1 : -1); // left
            addNeighbor(2, (cellNumber + 1) % 6 != 0 ? cellNumber + 1 : -1); // right
            addNeighbor(3, cellNumber < fleetGrid.Length - 6 ? cellNumber + 6 : -1); // down

            up = neighboringCells[0];
            left = neighboringCells[1];
            right = neighboringCells[2];
            down = neighboringCells[3];

            for (int i = neighboringCells.Length - 1; i >= 0; i--)
            {
                int j = random.Next(i + 1);
                GameObject temp = neighboringCells[i];
                neighboringCells[i] = neighboringCells[j];
                neighboringCells[j] = temp;
            }

            int yellowCounter = 0;

            foreach (var cell in neighboringCells)
            {
                if (cell != null && cell.GetComponent<Cell>().enemy)
                {
                    GameObject neighbor = cell.GetComponent<Cell>().enemy;
                    string neighborColor = neighbor.GetComponent<Enemy>().color;
                    if (neighborColor != "Yellow")
                    {
                        if (!special)
                        {
                            color = neighborColor;
                            colorManager.SetEnemyColor(this.gameObject, color, true);
                            break;
                        }
                        else // If special...
                        {
                            // Look like the neighbor and use its abilities,
                            // but remain yellow (with tag, maybe)
                            imitationColor = neighborColor;
                            colorManager.SetEnemyColor(this.gameObject, imitationColor, false, true);
                            isImitation = true;

                            // If one of the true neighbors dies,
                            // return to yellow color, restart imitate ability
                            // timer, set inMiddle to false, set isImitation to false
                        }
                    }
                    else
                    {
                        yellowCounter++;

                        if(yellowCounter == 4)
                        {
                            inMiddle = true;
                        }
                    }
                }
            }
        }
    }

    public void hit(string tongueColor, int magicValue)
    {
        if(magicValue > 0 || tongueColor == color)
        {
            alive = false;
            checkNeighbors();
            gridManagerScript.TallyScore(gridManagerScript.fleetGrid, this.gameObject);

        }
    }

    public IEnumerator reveal()
    {
        int count = 0;
        bool revealed = false;
        Color falseColor = ColorExtension(imitationColor);
        while (count < 11)
        {
            yield return new WaitForSeconds(0.05f);

            if (this == null || gameObject == null || gameObject.GetComponent<SpriteRenderer>() == null)
            {
                yield break;
            }
            if (revealed == false)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                revealed = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = falseColor;
                revealed = false;
            }
            count ++;
            if (count == 10)
            {
                isImitation = false;
                timeSinceLastActivation = 0;
            }
        }

        Color ColorExtension(string colorName)
        {
            switch(colorName)
            {
                case "Green":
                    return Color.green;
                case "Purple":
                    return new Color(1f, 0f, 1f, 1f);
                case "Red":
                    return Color.red;
                case "Blue":
                return Color.blue;
                default:
                    return Color.white;
            }
        }
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
                if (gridManagerScript.enemyCount != 0)
                {
                    SetMagicTongueChannel.RaiseEvent(true);
                    SetColorWheelWedge.RaiseEvent(color);
                }
            }

            // Subract special green count
            if (specialGreenCounted)
            {
                gridManagerScript.specialGreenCount--;
                specialGreenCounted = false;
            }
            checkRetreatEventChannel.RaiseEvent();
            gridManagerScript.DecrementEnemyCount();
            Destroy(this.gameObject);
        }
    }
}
