using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{

    // Enemy prefab
    public GameObject enemyPrefab;

    // Enemy specials
    public int specialGreenCount = 0;

    // List of all cell objects in the grid & cellPrefab gameobject
    public List<GameObject> grid = new List<GameObject>();
    public GameObject cellPrefab;

    // Variable for when fleet changes directions
    int turnInterval = 0;
    private float timer;
    private bool hasDescended = false;

    // List of enemies shifted one row down with the top row cleared
    public List<GameObject> shift = new List<GameObject>();
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
        initializeFleetGrid();
        populateFleet(24, 12);
        fleetShift();
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
            // colorManager.TotalEnemyCount();
            retreat(12);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            descend();
        }
        // FleetWipe
        if(wipedOut)
        {
            populateFleet(24);
            wipedOut = false;
        }
    }

    void initializeFleetGrid()
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

    public void fleetStatus()
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

                currentEnemy.GetComponent<Enemy>().checkNeighbors();
            }
        }
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

    void populateFleet(int end, int start = 0)
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

        fleetStatus();
    }

    public void fleetShift()
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

    public void advance()
    {
            for(int i = 0; i < 72; i++)
            {
                // Get the cell to update
                GameObject currentCell = grid[i];
                Transform currentCellTransform = currentCell.transform;

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
            populateFleet(6);

    }

    public void FleetMovement(float moveSpeed)
{
    float moveTime = 5.0f;
    float moveIncrement = 0.10f;

    if (!gameOver)
    {
        if (turnInterval < 10)
        {
            if (timer < moveTime / (colorManager.colorCounts["Green"] + specialGreenCount))
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
                    descend();
                    hasDescended = true;
                }
            }
        }
        else
        {
            if (timer < moveTime / (colorManager.colorCounts["Green"] + specialGreenCount))
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
                    descend();
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

    public void descend()
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
                fleetShift();
                advance();
            }
    }

    public void retreat(int cellNumber)
    {
        // Get the cell to update
        GameObject currentCell = grid[cellNumber];
        Transform currentCellTransform = currentCell.transform;
        // Get the enemy/null to be placed
        GameObject currentOccupant = currentCell.GetComponent<Cell>().enemy;
        // Debug.Log(currentOccupant);
        // Debug.Log("currentCell: " + currentCell);
        // Debug.Log("currentTransform: " + currentCellTransform);

        // Get above cell info
        GameObject aboveCell = grid[cellNumber - 6];
        Transform aboveCellTransform = aboveCell.transform;
        GameObject aboveOccupant = aboveCell.GetComponent<Cell>().enemy;

        // Debug.Log("aboveCell: " + aboveCell);
        // Debug.Log("aboveTransform: " + aboveCellTransform);

        // I'd like to add in some kind of delay so I can run this if check
        // if (aboveCell.GetComponent<Cell>().enemy == null)
        if (true)
        {
            // Debug.Log("reassign");
            // Detach the children from the current cell
            currentCellTransform.DetachChildren();
            // Set current occupant as child of above cell
            currentOccupant.transform.SetParent(aboveCellTransform);
            // Set current occupant position to above cell
            currentOccupant.transform.localPosition = Vector3.zero;
            // Update current cell's enemy info
            currentCell.GetComponent<Cell>().enemy = null;
            currentCell.GetComponent<Cell>().color = null;
            // Update above cell's enemy info
            aboveCell.GetComponent<Cell>().enemy = currentOccupant;
            aboveCell.GetComponent<Cell>().color = currentOccupant.GetComponent<Enemy>().color;

            Debug.Log(currentOccupant.GetComponent<Enemy>().color + " enemy in cell " + cellNumber + " retreating");
        }

        if(cellNumber < 66)
        {
            GameObject belowOccupant = grid[cellNumber + 6].GetComponent<Cell>().enemy;
            if (belowOccupant != null)
            {
                belowOccupant.GetComponent<Enemy>().checkRetreat();
            }
        }
    }
    public void GameOver()
    {
        Debug.Log("GAME OVER");
        gameOver = true;
    }

}
