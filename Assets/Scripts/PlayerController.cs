using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool alive;
    // Movement
    private float tankSpeed;
    private float screenBound = 3f;
    // GridManager
    private GameObject fleet;
    private GridManager gridManagerInstance;

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

    // Death and Respawn
    public bool spawnProtection = false;
    private SpriteRenderer spriteRenderer;
    private float respawnTimer = 5;


    // GameOver
    public bool gameOver = false;

    [Header("Event Channels")]
    public VoidEventChannelSO gameOverEventChannel;
    public VoidEventChannelSO fleetWipeEC;

    void Awake()
    {
        alive = true;
        tankSpeed = 5f;
        // Get fleet GO and Grid script
        GameObject fleet = GameObject.Find("Fleet");
        gridManagerInstance = fleet.GetComponent<GridManager>();
        // Get ColorManager instance
        colorManager = ColorManager.Instance;
        // Create a reference to the colorCounts dictionary
        colorCounts = colorManager.colorCounts;

    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (spawnProtection == false)
            {
                fireLaser();
            }
        }

        if(colorManager.magicLaser && colorSet == false)
        {
            colorManager.Multicolor(this.gameObject);
            colorSet = true;
        }

        if(!alive && respawnTimer >= 5)
        {
            respawnTimer = 0;
        }
        if(respawnTimer < 5)
        {
            respawnPlayer();
        }

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
        colorSet = false;

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
    public void death()
    {
        this.gameObject.transform.position = new Vector3(0f, -6.69f, -2f);
    }

    public void hit()
    {
        if(!spawnProtection)
        {
            Debug.Log("Hit");
            alive = false;
            death();
        }
    }

    void GameOver()
    {
        gameOver = true;
    }

    private IEnumerator BlinkSprite()
    {
        int count = 0;
        while (count < 6)
        {
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.enabled = !spriteRenderer.enabled;
            count ++;
        }
    }

    public void respawnPlayer()
   {
    // Debug.Log("respawn");
    float respawnTime = 3.5f;
    float iFrames = 5f;
    respawnTimer += Time.deltaTime;
    if(respawnTimer >= respawnTime && alive == false)
    {
        transform.position = new Vector3(0f, -4.69f, -2f);
        spawnProtection = true;
        alive = true;
        StartCoroutine(BlinkSprite());
    } else if (respawnTimer >= iFrames)
    {
        spawnProtection = false;
    }
   }
}
