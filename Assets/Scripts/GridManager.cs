using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{

    // Enemy prefab
    public GameObject enemyPrefab;
    public int neighborsChecked = 0;

    // Enemy specials
    public int specialGreenCount = 0;

    // List of all cell objects in the grid & cellPrefab gameobject
    public List<GameObject> grid = new List<GameObject>();
    public GameObject[] fleetGrid = new GameObject[60];
    public GameObject cellPrefab;

    // Variable for when fleet changes directions
    int turnInterval = 0;
    private float timer;
    private bool hasDescended = false;

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

    [Header("Scoring")]
    [SerializeField]
    private ScoreManagerSO scoreManager;
    public GameObject scoreDisplay;

    private static readonly int[] dx = { -1, 1, 0, 0 }; // Horizontal shifts (left, right)
    private static readonly int[] dy = { 0, 0, -1, 1 }; // Vertical shifts (up, down)
    private const int width = 6;
    private const int height = 10;
    public Canvas canvas;

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
    }

    void Awake()
    {
        gameOver = false;
        colorManager = ColorManager.Instance;
        initializeFleetGrid();
        populateFleet(12);
        initialGridPos = transform.position;
    }

    void FixedUpdate()
    {
        FleetMovement();

    }

    void Update()
    {
        Cheats();
    }

    void initializeFleetGrid()
    {
        int cellInx = 0;
        int columns = width;
        int rows = fleetGrid.Length / columns;
        float cellOffset = 0.75f;
        float gridOffsetX = 1.875f;
        float gridOffsetY = 3.75f;

        for(int row = 0; row < rows; row++)
        {
            for(int column = 0; column < columns; column++)
            {
                // 1. Set position of cells and grid
                float xPosition = (float)column;
                float yPosition = (float)row;

                xPosition = xPosition * cellOffset;
                yPosition = yPosition * cellOffset;

                xPosition = xPosition - gridOffsetX;
                yPosition = yPosition - gridOffsetY;

                Vector3 cellLocation = new Vector3(xPosition, yPosition, 0);
                // 2. Add cells

                GameObject cell = Instantiate(cellPrefab, transform);
                // Name
                cell.GetComponent<Cell>().name = "Cell " + cellInx.ToString();
                // Number
                cell.GetComponent<Cell>().number = cellInx;
                // Location
                cell.transform.localPosition = cellLocation;

                // // 3. Add cells to fleetGrid array
                fleetGrid[cellInx] = cell;
                cellInx++;

            }
        }
    }

    // THIS FUNCTION IS ALMOST CERTAINLY NOT DRY
    // THERE MUST BE A BETTER PLACE TO MAKE SURE SPECIALS ARE SET
    public void fleetStatus()
    {
        // Iterate through the cells in the grid
        for (int i = 0; i < fleetGrid.Length; i++)
        {
            // Get the cell
            GameObject currentCell = fleetGrid[i];
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
        if (globalLevel < 2)
        {
            populateFleet(12);
        }
        else if (globalLevel < 4)
        {
            populateFleet(18);
        }
        else if (globalLevel < 7)
        {
            populateFleet(24);
        }
        else
        {
            populateFleet(30);
        }
        turnInterval = 0;
        timer = 0;
    }

    void populateFleet(int end, int start = 0, string forcedColor = null)
    {

        // Iterate through the selected number of rows
        for (int i = start; i < end; i++)
        {
            // Instantiate enemy at the cell's transform
            GameObject cell = fleetGrid[i];
            GameObject enemy = Instantiate(enemyPrefab, cell.transform);
            // Assign enemy object to the cell
            cell.GetComponent<Cell>().enemy = enemy;
            // Assign cell's color
                Debug.Log(forcedColor);
            if (forcedColor != null)
            {
                colorManager.SetEnemyColor(enemy, forcedColor);
            }
            cell.GetComponent<Cell>().color = enemy.GetComponent<Enemy>().color;
        }
        enemyCount += (end - start);
        fleetStatus();
    }

    void Cheats()
    {
        // Enemy Counts
        if(Input.GetKeyDown(KeyCode.C))
        {
            colorManager.totalEnemyCount();
        }
        // Forced Spawns
        if(Input.GetKeyDown(KeyCode.D))
        {
            descend();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            descend("Red");
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            descend( "Yellow");
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            descend( "Blue");
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            descend( "Green");
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            descend( "Purple");
        }
    }


    public void descend(string forcedColor = null)
    {
        for (int i = fleetGrid.Length - 6; i < fleetGrid.Length; i++)
                {
                    GameObject currentCell = fleetGrid[i];
                    if (currentCell.GetComponent<Cell>().enemy)
                    {
                        GameOver();
                    }
                }

        if(!gameOver)
        {
            for(int i = fleetGrid.Length - 1; i > 5; i--)
            {
                // Get the cell to update
                GameObject receivingCell = fleetGrid[i];
                Transform receivingCellTransform = receivingCell.transform;

                // Get the enemy/null to be placed
                GameObject givingCell = fleetGrid[i - 6];
                GameObject newOccupant = givingCell.GetComponent<Cell>().enemy;

                if(newOccupant != null)
                {
                    // Detach the children from the current cell
                    receivingCellTransform.DetachChildren();

                    // Set the new occupant as the child of the current cell
                    newOccupant.transform.SetParent(receivingCellTransform);

                    // Update the new occupant's position
                    newOccupant.transform.localPosition = Vector3.zero;

                    // Update cell's enemy info
                    receivingCell.GetComponent<Cell>().enemy = newOccupant;
                    receivingCell.GetComponent<Cell>().color = newOccupant.GetComponent<Enemy>().color;

                }
                else
                {
                    receivingCell.GetComponent<Cell>().enemy = null;
                }
            }
            populateFleet(6, 0, forcedColor);
        }

    }

    public void FleetMovement()
    {
        float moveIncrement = 0.10f;

        if (!gameOver)
        {
            if (turnInterval < 10)
            {
                if (timer < moveTimeByLevel(globalLevel))
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
                if (timer < moveTimeByLevel(globalLevel))
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
            float calculatedTime = Mathf.Max(baseMoveTime * (1 - (level - 1) * factor));
            // Debug.Log("moveTime: " + calculatedTime / (colorManager.colorCounts["Green"] + specialGreenCount/2));

            return calculatedTime / (colorManager.colorCounts["Green"] + specialGreenCount/2);

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
        // Iterate through fleetGrid array
        for (int i = 0; i < fleetGrid.Length; i++)
        {
            // Create warpZones dictionary
            Dictionary<string, int> warpZones = new Dictionary<string, int>();
            // Get enemy
            enemy = fleetGrid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                // Check up if not bot row
                if (i > 5 && fleetGrid[i - 6].GetComponent<Cell>().enemy == null)
                {
                    warpZones["up"] = (i - 6);
                }
                // Check left if not left column
                if (i % 6 != 0 && fleetGrid[i - 1].GetComponent<Cell>().enemy == null)
                {
                    warpZones["left"] = (i - 1);
                }
                // Check right if not right column
                if (i % 6 != 5 && fleetGrid[i + 1].GetComponent<Cell>().enemy == null)
                {
                    warpZones["right"] = (i + 1);
                }
                // Check down if not bot row
                if (i < fleetGrid.Length - 6 && fleetGrid[i + 6].GetComponent<Cell>().enemy == null)
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
                GameObject warpCell = fleetGrid[warpZone];
                GameObject newEnemy = Instantiate(enemyPrefab, warpCell.transform);
                enemyCount++;
                // Assign enemy object to the cell
                warpCell.GetComponent<Cell>().enemy = newEnemy;
                if (callingEnemy.GetComponent<Enemy>().special == true)
                {
                    // Assign enemy's color
                    enemy = fleetGrid[cellInx].GetComponent<Cell>().enemy;
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
        for (int i = 0; i < fleetGrid.Length; i++)
        {
            GameObject enemy = fleetGrid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.group = 0;
            }
        }
        int group = 0;
        for (int i = 0; i < fleetGrid.Length; i++)
        {
            GameObject enemy = fleetGrid[i].GetComponent<Cell>().enemy;
            if(enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                // If an enemy has not been grouped
                if (enemyScript.group == 0 && enemyScript.alive)
                {
                    group++;
                    enemyScript.group = group;
                    // Have enemy run neighbor checks until all connected enemies are labelled group number (coroutine?)
                    checkNeighborRetreat(fleetGrid[i].GetComponent<Cell>().number, group);
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
        fleetStatus();
    }
    public void checkNeighborRetreat(int cellNumber, int group)
    {
        // Debug.Log("checkNeighborRetreat");
        if(cellNumber > 5)
        {
            if(fleetGrid[cellNumber - 6].GetComponent<Cell>().enemy != null && fleetGrid[cellNumber - 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                fleetGrid[cellNumber - 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(fleetGrid[cellNumber - 6].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber - 6, group);
            }
        }
        if(cellNumber % 6 != 0)
        {
            if(fleetGrid[cellNumber - 1].GetComponent<Cell>().enemy != null && fleetGrid[cellNumber - 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                fleetGrid[cellNumber - 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(fleetGrid[cellNumber - 1].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber - 1, group);
            }
        }
        if((cellNumber + 1) % 6 != 0)
        {
            if(fleetGrid[cellNumber + 1].GetComponent<Cell>().enemy != null && fleetGrid[cellNumber + 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                fleetGrid[cellNumber + 1].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(fleetGrid[cellNumber + 1].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber + 1, group);
            }
        }
        if(cellNumber < fleetGrid.Length - 6)
        {
            if(fleetGrid[cellNumber + 6].GetComponent<Cell>().enemy != null && fleetGrid[cellNumber + 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group == 0)
            {
                fleetGrid[cellNumber + 6].GetComponent<Cell>().enemy.GetComponent<Enemy>().group = group;
                queue.Add(fleetGrid[cellNumber + 6].GetComponent<Cell>().enemy);
                checkNeighborRetreat(cellNumber + 6, group);
            }
        }
    }
    public void retreat(int cellNumber)
    {
        // Get the cell to update
        GameObject currentCell = fleetGrid[cellNumber];
        Transform currentCellTransform = currentCell.transform;
        // Get the enemy/null to be placed
        GameObject currentOccupant = currentCell.GetComponent<Cell>().enemy;

        // Get above cell info
        if(cellNumber > 5)
        {
            GameObject aboveCell = fleetGrid[cellNumber - 6];
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

    private static bool InBounds(int x, int y)
    {
        return x >= 0 && x < height && y >= 0 && y < width;
    }

    // Breadth-First Search to explore all connected cells with the same color
    // FIX: Add a counter to keep track of how many enemies have been checked and
    // use this to set specials
    private void Bfs(GameObject[] grid, bool[] visited, int x, int y, string color)
    {
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((x, y));
        visited[x * width + y] = true;
        int multiplier = 1;

        while (queue.Count > 0)
        {
            int levelCount = queue.Count;

            for (int i = 0; i < levelCount; i++)
            {
                var (cx, cy) = queue.Dequeue();
                GameObject thisCell = grid[cx * width + cy];
                GameObject thisEnemy = thisCell.GetComponent<Cell>().enemy;
                Enemy thisEnemyScript = thisEnemy.GetComponent<Enemy>();
                int score = 100;
                scoreManager.IncreaseScore(score);
                // Call method to display score at defeated enemy's location
                // Fix color part so that it has a color converter
                VisualizeMultiplier(thisEnemy.transform.position, thisEnemy.GetComponent<Enemy>().color, multiplier);
                // Make sure to look at score manager
                // Debug.Log($"Visiting cell {cx * width + cy} (Color: {thisEnemyScript.color}) (Score: {score * multiplier})");
                // Explore neighbors
                for (int d = 0; d < 4; d++)
                {
                    int nx = cx + dx[d];
                    int ny = cy + dy[d];
                    if (InBounds(nx, ny))
                    {
                        int neighborIndex = nx * width + ny;
                        GameObject cell = grid[neighborIndex];
                        if (cell.GetComponent<Cell>().enemy)
                        {
                            GameObject neighbor = cell.GetComponent<Cell>().enemy;
                            Enemy neighborScript = neighbor.GetComponent<Enemy>();
                            if (!visited[neighborIndex] && neighborScript.color == color)
                            {
                                visited[neighborIndex] = true;
                                scoreManager.IncreaseScore(score, multiplier);
                                queue.Enqueue((nx, ny));
                            }
                        }
                    }
                }
            }
            multiplier++;
        }
    }

    public void TallyScore(GameObject[] grid, GameObject enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript.cellNumber < 0 || enemyScript.cellNumber >= grid.Length)
        {
            Debug.LogWarning("Cell index is out of the grid's bounds.");
            return;
        }


        int x = enemyScript.cellNumber / width;
        int y = enemyScript.cellNumber % width;
        bool[] visited = new bool[grid.Length];

        // Debug.Log($"Starting BFS from cell {enemyScript.cellNumber}, which has color {enemyScript.color}.");
        Bfs(grid, visited, x, y, enemyScript.color);
    }
    public void VisualizeMultiplier(Vector3 location, string color, int multiplier)
    {
        GameObject instance = Instantiate(scoreDisplay, location, Quaternion.identity);
        TextOnSpot textOnSpot = instance.GetComponent<TextOnSpot>();
        textOnSpot.SetMultiplier(multiplier, color);
    }
}
