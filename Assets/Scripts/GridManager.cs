using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{

    // Enemy prefab
    public GameObject enemyPrefab;

    // List of all cell objects in the grid & cellPrefab gameobject
    public List<GameObject> grid = new List<GameObject>();
    public GameObject cellPrefab;

    // Variable for when fleet changes directions
    int turnInterval = 0;
    private float timer;
    private bool hasDescended = false;

    // List of enemies shifted one row down with the top row cleared
    private List<GameObject> shift = new List<GameObject>();
    // Color dictionary
    public Dictionary<string, int> colorCounts = new Dictionary<string, int>();
    // Color Manager
    ColorManager colorManager;

    // Set gameOver
    public bool gameOver;

    // Fleet empty
    public bool wipedOut;

    void Awake()
    {
        gameOver = false;
        colorManager = ColorManager.Instance;
        InitializeFleetGrid();
        PopulateFleet(24);
    }
    void Start()
    {
        
    }

    void Update()
    {
        FleetMovement(.5f);
        // Debug
        if(Input.GetKeyDown(KeyCode.C))
        {
            TotalEnemyCount();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            FleetStatus();
        }
        // FleetWipe
        if(wipedOut)
        {
            PopulateFleet(24);
            wipedOut = false;
        }
    }

    void InitializeFleetGrid()
    {
        int cellInx = 0;
        int rows = 12;
        int columns = 6;
        for(int row = 0; row < rows; row++)
        {
            for(int column = 0; column < columns; column++)
            {
                // 1. Set position of cell
                float xPosition = (float)column;
                float yPosition = (float)row;
                // Set the spacing
                xPosition = xPosition * 0.75f;
                yPosition = yPosition * 0.75f;
                // Offset position so fleet GameObject is at the center of all cells
                xPosition = xPosition - 1.875f;
                yPosition = yPosition - 3.75f;
                // Set Vector3
                Vector3 cellLocation = new Vector3(xPosition, yPosition, 0);
                // 2. Add cells
                // Instantiate
                GameObject cell = Instantiate(cellPrefab, transform);
                // Name
                cell.GetComponent<Cell>().name = "Cell " + cellInx.ToString();
                // Number
                cell.GetComponent<Cell>().number = cellInx;
                // Location
                cell.transform.localPosition = cellLocation;

                // 3. Create list containing every cell
                grid.Insert(cellInx, cell);
                cellInx++;

            }
        }
    }

    public void FleetStatus()
    {
        colorCounts = colorManager.colorCounts;

        // Iterate through the cells in the grid
        for (int i = 0; i < grid.Count; i++)
        {
            // Get the cell
            GameObject currentCell = grid[i];
            // Check if current cell contains an enemy
            if (currentCell.GetComponent<Cell>().enemy)
            {
                // Set currentEnemy to the enemy in the cell
                GameObject currentEnemy = currentCell.GetComponent<Cell>().enemy;
                // Get the currentEnemyColor
                string currentEnemyColor = currentEnemy.GetComponent<Enemy>().color;
                colorCounts[currentEnemyColor] += 1;
                // Get currentEnemySpecial and currentEnemySuper
                bool currentEnemySpecial = currentEnemy.GetComponent<Enemy>().special;
                bool currentEnemySuper = currentEnemy.GetComponent<Enemy>().super;

                // Check if currentEnemySpecial is false
                if (!currentEnemySpecial)
                {
                    // Check the neighboring cells
                    List<GameObject> neighbors = new List<GameObject>();

                    // Check up if not bot row
                    if (i > 6)
                    {
                    GameObject up = grid[i - 6];
                    neighbors.Add(up);
                    }
                    // Check left if not left column
                    if (i % 6 != 0)
                    {
                    GameObject left = grid[i - 1];
                    neighbors.Add(left);
                    }
                    // Check right if not right column
                    if ((i + 1) % 6 != 0)
                    {
                    GameObject right = grid[i + 1];
                    neighbors.Add(right);
                    }
                    // Check down if not bot row
                    if (i < 66)
                    {
                    GameObject down = grid[i + 6];
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
                            if (neighborColor == currentEnemyColor)
                            {
                                matches.Add(neighborEnemy);
                            }
                        }
                    }
                    // If matches holds 2 or more enemies
                    if (matches.Count >= 2)
                    {
                        // Turn on currentEnemy's special
                        currentEnemy.GetComponent<Enemy>().special = true;
                        // Turn on the neighbor's specials
                        for (int k = 0; k < matches.Count; k++)
                        {
                            GameObject match = matches[k];
                            match.GetComponent<Enemy>().special = true;
                        }
                    }
                }
                else if (!currentEnemySuper)
                {

                }

            }
        }
    }

    void TotalEnemyCount()
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
    }

    public void FleetWipeCheck()
    {
        int emptyKeyCount = 0;
        foreach (var kvp in colorCounts)
        {
            if (kvp.Value <= 0)
            {
                emptyKeyCount++;
            }
        }
        if (emptyKeyCount == colorCounts.Count)
        {
            wipedOut = true;
        }
    }

    void PopulateFleet(int end, int start = 0)
    {

        // Iterate through the selected number of rows
        for (int i = start; i < end; i++)
        {
            // Instantiate enemy at the cell's transform
            GameObject cell = grid[i];
            GameObject enemy = Instantiate(enemyPrefab, cell.transform);
            // // Set the cellNum property on the enemy NECESSARY?
            // enemy.GetComponent<Enemy>().cellNum = i;
            // Assign enemy object to the cell
            cell.GetComponent<Cell>().enemy = enemy;
            // Assign cell's color
            cell.GetComponent<Cell>().color = enemy.GetComponent<Enemy>().color;

        }

        FleetStatus();
    }

    public void FleetShift()
    {
            shift = new List<GameObject>(new GameObject[72]);
            // Create list to hold enemies at indexes offset one row
            for (int i = 0; i < 72; i++)
            {
                if(i < 6)
                    {
                        // Set the values in the first row to null
                        shift[i] = null;
                    }
                else
                    {
                        // Populate the remaining indexes with the enemy in the cell currently above them
                        // Get the cell in the above row
                        GameObject aboveCell = grid[i - 6];
                        // Reference the enemy from the above row
                        GameObject aboveEnemy = aboveCell.GetComponent<Cell>().enemy;
                        // Set the current row to that enemy
                        shift[i] = aboveEnemy;

                    }
            }
    }

    public void Advance()
    {
            // Debug.Log("Advance");
            for(int i = 0; i < 72; i++)
            {
                // Get the cell to update
                GameObject currentCell = grid[i];
                Transform currentCellTransform = currentCell.transform;
                // Detach the children
                Transform cellTransform = currentCell.transform;

                // Get the enemy/null to be placed
                GameObject newOccupant = shift[i];

                // If the new occupant is not not null
                if(newOccupant != null)
                {
                    // Detach the children from the current cell
                    currentCellTransform.DetachChildren();

                    // Set the new occupant as the child of the current cell
                    newOccupant.transform.SetParent(currentCellTransform);

                    // Update the new occupant's position
                    newOccupant.transform.localPosition = Vector3.zero;

                    // Update cell's enemy info
                    currentCell.GetComponent<Cell>().enemy = newOccupant;
                    currentCell.GetComponent<Cell>().color = newOccupant.GetComponent<Enemy>().color;

                }
                else
                {
                    currentCell.GetComponent<Cell>().enemy = null;
                }
            }
            PopulateFleet(6);

    }

    public void FleetMovement(float moveSpeed)
{
    float moveTime = 5.0f;
    float moveIncrement = 0.10f;

    if (!gameOver)
    {
        if (turnInterval < 10)
        {
            if (timer < moveTime / colorCounts["Green"])
            {
                timer += Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.right * moveIncrement;
                timer = 0;
                turnInterval++;
                // Check if the descent should occur
                if (turnInterval == 10 && !hasDescended)
                {
                    Descend();
                    hasDescended = true;
                }
            }
        }
        else
        {
            if (timer < moveTime / colorCounts["Green"])
            {
                timer += Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.left * moveIncrement;
                timer = 0;
                turnInterval++;

                // Check if the descent should occur
                if (turnInterval == 20 && !hasDescended)
                {
                    Descend();
                    hasDescended = true;
                }
            }

            if (turnInterval == 20)
            {
                turnInterval = 0;
            }
        }

        // Reset the descent flag if the turnInterval is not 10 or 20
        if (turnInterval != 10 && turnInterval != 20)
        {
            hasDescended = false;
        }
    }
}

    public void Descend()
    {
        // Game Over
            for (int i = 66; i < 72; i++)
                {
                    GameObject currentCell = grid[i];
                    if (currentCell.GetComponent<Cell>().enemy)
                    {
                        GameOver();
                    }
                }
            if(!gameOver)
            {
                FleetShift();
                Advance();
            }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        gameOver = true;
    }

}
