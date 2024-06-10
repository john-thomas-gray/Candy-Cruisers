using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool alive;
    // Movement
    private float tankSpeed = 3f;
    private float screenBound = 3f;
    // GridManager
    private GameObject fleet;
    private GridManager gridManagerInstance;
    private bool wipedOut;

    // ColorManager
    private Dictionary<string, int> colorCounts;
    ColorManager colorManager;
    public bool colorSet;
    public string shotColor;
    public string color;

    // Laser
    public GameObject laserPrefab;
    private float shotCoolDown = 0.4f;
    private float timeSinceLastShot = 0.0f;

    // GameMaster
    GameMaster gameMaster;
    public bool spawnProtection = false;

    // GameOver
    public bool gameOver = false;

    [Header("Event Channels")]
    public VoidEventChannelSO gameOverEventChannel;

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
        // Get Game Master instance
        gameMaster = GameMaster.Instance;

    }

    void Start()
    {
        colorManager.SetColor(this.gameObject);
    }

    private void OnEnable()
    {
        gameOverEventChannel.OnEventRaised+= GameOver;
    }

    private void OnDisable()
    {
        gameOverEventChannel.OnEventRaised -= GameOver;
    }

    void Update()
    {
        if(alive && !gameOver)
        {
            playerMovement();
            fireLaser();
        }

        if(colorManager.magicLaser)
        {
            colorManager.Multicolor(this.gameObject);
        }
        // Debug.Log(spawnProtection);
    }

    void playerMovement()
    {
        // Move Left
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + Vector3.left * tankSpeed * Time.deltaTime;
        }
        // Move Right
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + Vector3.right * tankSpeed * Time.deltaTime;
        }
        if(transform.position.x > screenBound)
        {
            transform.position = transform.position + Vector3.right * -2 * screenBound;
        }
        if(transform.position.x < -screenBound)
        {
            transform.position = transform.position + Vector3.right * 2 * screenBound;
        }
    }
    void fireLaser()
    {
        timeSinceLastShot += Time.deltaTime;

        if(Input.GetKey(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) && timeSinceLastShot >= shotCoolDown)
        {
                // Debug.Log("shotColor: " + shotColor);
                // Debug.Log("color: " + color);
            colorCounts = colorManager.colorCounts;

            if (colorManager.colorSet)
            {
                // Set independent shotColor variable, so missile can reference it once player changes colors
                shotColor = colorManager.shotColor;
                // Spawn laser in front of player
                Instantiate(laserPrefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation);
                // Make player a different color
                colorManager.SetColor(this.gameObject);
                // Reset cooldown
                timeSinceLastShot = 0.0f;
            }
        }
    }
    void death()
    {
        this.gameObject.transform.position = new Vector3(0f, -6.69f, -2f);
    }

    public void hit()
    {
        if(!spawnProtection)
        {
            alive = false;
            death();
        }
    }

    void GameOver()
    {
        gameOver = true;
    }
}
