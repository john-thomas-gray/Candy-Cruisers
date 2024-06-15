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

    // FleetWipe
    private Vector3 initialGridPos;
    public int enemyCount = 0;

    // Retreat
    private List<GameObject> queue = new List<GameObject>();

    // Level
    [SerializeField]
    public int globalLevel = -1;
    [Header("Event Channels")]
    public IntEventChannelSO updateGlobalLevelChannel;
    public VoidEventChannelSO checkRetreatChannel;
    public VoidEventChannelSO gameOverEventChannel;
    public VoidEventChannelSO fleetWipeEC;

    private void OnEnable()
    {
        updateGlobalLevelChannel.OnEventRaised += setGlobalLevel;
        checkRetreatChannel.OnEventRaised += StartCheckRetreat;
        fleetWipeEC.OnEventRaised += FleetWipe;

    }
    private void OnDisable()
    {
        updateGlobalLevelChannel.OnEventRaised -= setGlobalLevel;
        checkRetreatChannel.OnEventRaised -= StartCheckRetreat;
        fleetWipeEC.OnEventRaised -= FleetWipe;
    }

    void setGlobalLevel(int level)
    {
        globalLevel = level;
        Debug.Log("GridManager, setGlobalLevel" + level);
    }

    void Awake()
    {
        gameOver = false;
        colorManager = ColorManager.Instance;
        initializeFleetGrid();
        populateFleet(6);
        fleetShift();
        initialGridPos = transform.position;
    }

    void Update()
    {
        FleetMovement();
        // Debug
        if(Input.GetKeyDown(KeyCode.C))
        {
            colorManager.totalEnemyCount();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            descend();
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
        // Debug.Log("fleetStatus");
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

    public void DecrementEnemyCount()
    {
        enemyCount--;
        if (enemyCount == 0)
        {
            fleetWipeEC.RaiseEvent();
        }
    }
    public void FleetWipe()
    {

        StartCoroutine(PopulateFleetAfterDelay());
    }

    private IEnumerator PopulateFleetAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        transform.position = initialGridPos;
        if (globalLevel < 4)
        {
            populateFleet(24);
        }
        else if (globalLevel < 7)
        {
            populateFleet(30);
        }
        else
        {
            populateFleet(36);
        }
        turnInterval = 0;
        timer = 0;
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
        enemyCount += (end - start);
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

    public void FleetMovement()
    {
        float moveIncrement = 0.10f;

        if (!gameOver)
        {
            if (turnInterval < 10)
            {
                if (timer < moveTimeByLevel(globalLevel) / (colorManager.colorCounts["Green"] + specialGreenCount))
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
                if (timer < moveTimeByLevel(globalLevel) / (colorManager.colorCounts["Green"] + specialGreenCount))
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

        float moveTimeByLevel(int level)
        {
            float baseMoveTime = 10.0f;
            float factor = 0.10f;
            float calculatedTime = baseMoveTime * (1 - (level - 1) * factor);
            return Mathf.Max(calculatedTime, 0); // Ensures the returned time is never negative

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

    public void beamIn(GameObject callingEnemy)
    {
        // Set FarthestEnemy
        int farthestEnemyInx = -1;
        int cellInx = -1;
        // Create dict
        Dictionary<int, Dictionary<string, int>> warpDict = new Dictionary<int, Dictionary<string, int>>();
        // Define enemy in wide scope
        GameObject enemy = null;
        // Iterate through grid list
        for (int i = 0; i < grid.Count; i++)
        {
            // Create warpZones dictionary
            Dictionary<string, int> warpZones = new Dictionary<string, int>();
            // Get enemy
            enemy = grid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                // Check up if not bot row
                if (i > 5 && grid[i - 6].GetComponent<Cell>().enemy == null)
                {
                    warpZones["up"] = (i - 6);
                }
                // Check left if not left column
                if (i % 6 != 0 && grid[i - 1].GetComponent<Cell>().enemy == null)
                {
                    warpZones["left"] = (i - 1);
                }
                // Check right if not right column
                if (i % 6 != 5 && grid[i + 1].GetComponent<Cell>().enemy == null)
                {
                    warpZones["right"] = (i + 1);
                }
                // Check down if not bot row
                if (i < 66 && grid[i + 6].GetComponent<Cell>().enemy == null)
                {
                    warpZones["down"] = (i + 6);
                }
                if (warpZones.Count > 0)
                {
                    warpDict[i] = warpZones;
                    farthestEnemyInx = i;
                }
            }
        }
        // If a potential warp zone exists
        if (warpDict.Count != 0)
        {
            int warpZone = -1;
            // Select valid warpZone
            while(warpZone == -1)
            {
                if(warpDict.Count == 0)
                {
                    // Debug.LogWarning("warpDict empty");
                    break;
                }
                cellInx = GetRandomCellIndex();
                if (InFarthestRow(cellInx) && !callingEnemy.GetComponent<Enemy>().super == true)
                {
                    // Remove down and right values from the furthest index
                    if (cellInx == farthestEnemyInx)
                    {
                        RemoveSelectedKey(warpDict[cellInx], new List<string>{ "down", "right"});
                        if (warpDict[cellInx].Count == 0)
                        {
                            warpDict.Remove(cellInx);
                            continue;
                        }
                    }
                    else // Remove down values from indexes in the same row as furthest index
                    {
                        RemoveSelectedKey(warpDict[cellInx], new List<string>{ "down" });
                        if (warpDict[cellInx].Count == 0)
                        {
                            warpDict.Remove(cellInx);
                            continue;
                        }
                    }

                }

                warpZone = SelectNullCell(warpDict[cellInx]);
            }
            if (warpZone >= 0)
            {
                GameObject warpCell = grid[warpZone];
                GameObject newEnemy = Instantiate(enemyPrefab, warpCell.transform);
                enemyCount++;
                // Assign enemy object to the cell
                warpCell.GetComponent<Cell>().enemy = newEnemy;
                if (callingEnemy.GetComponent<Enemy>().special == true)
                {
                    // Assign enemy's color
                    enemy = grid[cellInx].GetComponent<Cell>().enemy;
                    //BANDAID
                    colorManager.colorCounts[newEnemy.GetComponent<Enemy>().color]--;
                    newEnemy.GetComponent<Enemy>().color = enemy.GetComponent<Enemy>().color;
                    newEnemy.GetComponent<Enemy>().warpedIn = true;
                }
                // Assign cell's color
                warpCell.GetComponent<Cell>().color = newEnemy.GetComponent<Enemy>().color;
                newEnemy.GetComponent<Enemy>().checkNeighbors();
            }
        }
        else
        {
            Debug.LogWarning("No room to warp in!");
        }

        void RemoveSelectedKey(Dictionary<string, int> dict, List<string> excludedKeys = null)
        {
            if (excludedKeys == null)
                return;

            // Create a list to store keys to be removed
            List<string> keysToRemove = new List<string>();

            // Iterate over the dictionary and add keys to be removed to the list
            foreach (string key in dict.Keys)
            {
                if (excludedKeys.Contains(key))
                {
                    keysToRemove.Add(key);
                }
            }

            // Remove keys from the dictionary using the list
            foreach (string key in keysToRemove)
            {
                dict.Remove(key);
            }
        }

        int SelectNullCell(Dictionary<string, int> dict)
        {

            // Check which keys are present in the dictionary
            List<string> presentKeys = new List<string>();
            foreach (string key in dict.Keys)
            {
                presentKeys.Add(key);
            }

            // Generate a random index within the range of keys
            int randomIndex = UnityEngine.Random.Range(0, presentKeys.Count - 1);

            // Randomly choose a key among the present keys
            string chosenKey = presentKeys[randomIndex];

            // return the index of the appropriate null cell
            return dict[chosenKey];
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
            List<int> keys = new List<int>(warpDict.Keys);

            if (keys.Count == 0)
            {
                return -1;
            }

            int randomIndex = UnityEngine.Random.Range(0, keys.Count);

            return keys[randomIndex];
        }
    }
    public void checkRetreat()
    {
        // Debug.Log("checkRetreat");
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
        // Debug.Log("checkNeighborRetreat");
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
        // Get the cell to update
        GameObject currentCell = grid[cellNumber];
        Transform currentCellTransform = currentCell.transform;
        // Get the enemy/null to be placed
        GameObject currentOccupant = currentCell.GetComponent<Cell>().enemy;

        // Get above cell info
        if(cellNumber > 5)
        {
            GameObject aboveCell = grid[cellNumber - 6];
            Transform aboveCellTransform = aboveCell.transform;
            GameObject aboveOccupant = aboveCell.GetComponent<Cell>().enemy;

            // This must be delayed. Currently it uses coroutines
            if (currentOccupant && aboveCell.GetComponent<Cell>().enemy == null)
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
        StartCheckRetreat();
    }
    public void StartCheckRetreat()
    {
        // Start a coroutine for a delay
        StartCoroutine(DelayedInvoke());
    }

    private IEnumerator DelayedInvoke()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
        checkRetreat();
    }
    public void GameOver()
    {
        gameOver = true;
        gameOverEventChannel.RaiseEvent();
    }



}
