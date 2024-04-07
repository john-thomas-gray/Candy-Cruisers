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

    private Vector3 initialGridPos;

    // Retreat
    private List<GameObject> queue = new List<GameObject>();

    void Awake()
    {
        gameOver = false;
        colorManager = ColorManager.Instance;
        initializeFleetGrid();
        populateFleet(24);
        fleetShift();
        initialGridPos = transform.position;
    }
    void Start()
    {

        EventManager.CheckRetreat += checkRetreat;
    }

    void Update()
    {
        // FleetMovement(.5f);
        // Debug
        if(Input.GetKeyDown(KeyCode.C))
        {
            colorManager.totalEnemyCount();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            descend();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            // checkRetreat();
            beamIn();
        }
        // FleetWipe
        if(wipedOut)
        {
            transform.position = initialGridPos;
            turnInterval = 0;
            populateFleet(24);
            wipedOut = false;
        }
        // checkRetreat();

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

    public void beamIn()
    {
        // Set FarthestEnemy
        int farthestEnemyInx = 0;
        // Create dict
        Dictionary<int, List<int>> warpDict = new Dictionary<int, List<int>>();
        // Iterate through grid list
        for (int i = 0; i < grid.Count; i++)
        {
            // Create warpZones list
            List<int> warpZones = new List<int>();
            // Get enemy
            GameObject enemy = grid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                // Check up if not bot row
                if (i > 5 && grid[i - 6].GetComponent<Cell>().enemy == null)
                {
                    warpZones.Add(i - 6);
                }
                // Check left if not left column
                if (i % 6 != 0 && grid[i - 1].GetComponent<Cell>().enemy == null)
                {
                    warpZones.Add(i - 1);
                }
                // Check right if not right column
                if (i % 6 != 5 && grid[i + 1].GetComponent<Cell>().enemy == null)
                {
                    warpZones.Add(i + 1);
                }
                // Check down if not bot row
                if (i < 66 && grid[i + 6].GetComponent<Cell>().enemy == null)
                {
                    warpZones.Add(i + 6);
                }
                if (warpZones.Count > 0)
                {
                    warpDict[i] = warpZones;
                    farthestEnemyInx = i;
                }
                // Debug.Log("Cell#: " + i + " WarpZones: " + warpZones.Count + " WarpDict: " + warpDict.Count);
            }
        }
        if (warpDict.Count != 0)
        {
            // Pick a random enemy
            int cellInx = GetRandomCellIndex();
            int warpInx = 0;
            if (cellInx == farthestEnemyInx)
            {

            }
            else if (InFarthestRow(cellInx))
            {

            }
            else
            {
                warpInx = UnityEngine.Random.Range(0, warpDict[cellInx].Count);
            }
            // int warpInx = UnityEngine.Random.Range(0, warpDict[cellInx].Count - 1);
            // Debug.Log("CellInx: " + cellInx + " WarpInx: " + warpInx);
            int warpZone = warpDict[cellInx][warpInx];
            GameObject warpCell = grid[warpZone];
            GameObject newEnemy = Instantiate(enemyPrefab, warpCell.transform);
            // Assign enemy object to the cell
            warpCell.GetComponent<Cell>().enemy = newEnemy;
            // Assign cell's color
            warpCell.GetComponent<Cell>().color = newEnemy.GetComponent<Enemy>().color;
        }

        bool InFarthestRow(int cellInx)
        {
            // Find the row index of the highest value
            int highestValueRowIndex = farthestEnemyInx / 6;

            // Calculate the row index range (inclusive) for the highest value
            int startIndex = highestValueRowIndex * 6;
            int endIndex = startIndex + 5;

            // Check if the cell index falls within the row range of the highest value
            if (startIndex <= cellInx && cellInx <= endIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        int GetRandomCellIndex()
        {
            // Get the keys of the dictionary
            List<int> keys = new List<int>(warpDict.Keys);

            // Check if the dictionary is empty
            if (keys.Count == 0)
            {
                return -1;
            }

            // Generate a random index within the range of keys
            int randomIndex = UnityEngine.Random.Range(0, keys.Count);

            // Retrieve and return the key at the randomly generated index
            return keys[randomIndex];
        }
    }
    public void checkRetreat()
    {
        // Empty out the queue
        queue.Clear();
        // Reset enemy group numbers
        for (int i = 0; i < grid.Count; i++)
        {
            GameObject enemy = grid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.group = 0;
            }
        }
        // Set fresh group variable
        int group = 0;
        // Iterate through the grid list
        for (int i = 0; i < grid.Count; i++)
        {
            // Get enemy
            GameObject enemy = grid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                // If an enemy has not been grouped
                if (enemyScript.group == 0 && enemyScript.alive)
                {
                    // Iterate group number
                    group++;
                    // Label him group number
                    enemyScript.group = group;
                    // Have enemy run neighbor checks until all connected enemies are labelled group number (coroutine?)
                    checkNeighborRetreat(grid[i].GetComponent<Cell>().number, group);
                    // Add to check queue
                    queue.Add(enemy);
                }
                // Debug.Log("Enemy number " + grid[i].GetComponent<Cell>().number + " is in group " + group);
            }
        }
        for (int j = 1; j <= group; j++)
        {
            // Set initial retreat to true
            bool shouldRetreat = true;
            // Iterate through the queue
            for (int i = 0; i < queue.Count; i++)
            {
                GameObject enemy = queue[i];

                // If queued enemy is in the current group
                if (enemy != null && enemy.GetComponent<Enemy>().group == j)
                {
                    if (enemy.GetComponent<Enemy>().cellNumber < 6)
                    {
                        shouldRetreat = false;
                        break;
                    }
                }
            }

            if (shouldRetreat)
            {
                foreach (GameObject enemy in queue)
                {
                    if (enemy != null && enemy.GetComponent<Enemy>().group == j && enemy.GetComponent<Enemy>().cellNumber >= 6)
                    {
                        StartCoroutine(RetreatWithDelay(enemy.GetComponent<Enemy>().cellNumber));
                    }
                }
            }
        }

        IEnumerator RetreatWithDelay(int cellNumber)
        {
            // Get the enemy to retreat
            GameObject enemy = GetEnemyByCellNumber(cellNumber);
            // Retreat after a delay
            yield return new WaitForSeconds(.1f);
            retreat(cellNumber);
        }

        GameObject GetEnemyByCellNumber(int cellNumber)
        {
            // Find and return the enemy with the specified cell number
            foreach (GameObject enemy in queue)
            {
                if (enemy != null && enemy.GetComponent<Enemy>().cellNumber == cellNumber)
                {
                    return enemy;
                }
            }
            return null;
        }

    }
    public void checkNeighborRetreat(int cellNumber, int group)
    {

        if(cellNumber > 5)
        {
            if(grid[cellNumber - 6].GetComponent<Cell>().enemy != null && grid[cellNumber - 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                grid[cellNumber - 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(grid[cellNumber - 6].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber - 6, group);
            }
        }
        if(cellNumber % 6 != 0)
        {
            if(grid[cellNumber - 1].GetComponent<Cell>().enemy != null && grid[cellNumber - 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                grid[cellNumber - 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(grid[cellNumber - 1].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber - 1, group);
            }
        }
        if((cellNumber + 1) % 6 != 0)
        {
            if(grid[cellNumber + 1].GetComponent<Cell>().enemy != null && grid[cellNumber + 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                grid[cellNumber + 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(grid[cellNumber + 1].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber + 1, group);
            }
        }
        if(cellNumber < 66)
        {
            if(grid[cellNumber + 6].GetComponent<Cell>().enemy != null && grid[cellNumber + 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                grid[cellNumber + 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(grid[cellNumber + 6].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber + 6, group);
            }
        }
    }
    public void retreat(int cellNumber)
    {
        EventManager eventManagerInstance = EventManager.Instance;
        // Get the cell to update
        GameObject currentCell = grid[cellNumber];
        Transform currentCellTransform = currentCell.transform;
        // Get the enemy/null to be placed
        GameObject currentOccupant = currentCell.GetComponent<Cell>().enemy;
        // Debug.Log(currentOccupant);
        // Debug.Log("currentCell: " + currentCell);
        // Debug.Log("currentTransform: " + currentCellTransform);

        // Get above cell info
        if(cellNumber > 5)
        {
            GameObject aboveCell = grid[cellNumber - 6];
            Transform aboveCellTransform = aboveCell.transform;
            GameObject aboveOccupant = aboveCell.GetComponent<Cell>().enemy;

            // Debug.Log("aboveCell: " + aboveCell);
            // Debug.Log("aboveTransform: " + aboveCellTransform);

            // This must be delayed. Currently it uses coroutines
            if (aboveCell.GetComponent<Cell>().enemy == null)
            {
                // Detach the children from the current cell
                currentCellTransform.DetachChildren();
                // Set current occupant as child of above cell
                // ERROR currentOccupant is sometimes set to null during retreat
                currentOccupant.transform.SetParent(aboveCellTransform);
                // Set current occupant position to above cell
                currentOccupant.transform.localPosition = Vector3.zero;
                // Set current occupant's cell number
                currentOccupant.GetComponent<Enemy>().cellNumber = aboveCell.GetComponent<Cell>().number;
                // Update current cell's enemy info
                currentCell.GetComponent<Cell>().enemy = null;
                currentCell.GetComponent<Cell>().color = null;
                // Update above cell's enemy info
                aboveCell.GetComponent<Cell>().enemy = currentOccupant;
                aboveCell.GetComponent<Cell>().color = currentOccupant.GetComponent<Enemy>().color;
                // Check special
                fleetStatus();

            }
        }
        eventManagerInstance.StartCheckRetreat();
    }
    public void GameOver()
    {
        Debug.Log("GAME OVER");
        gameOver = true;
    }



}
