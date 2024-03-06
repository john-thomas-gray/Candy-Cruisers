using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool alive;
    // Movement
    private float tankSpeed = 4f;
    private float screenBoundLeft = -2.45f;
    private float screenBoundRight = 2.45f;
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
    // Update is called once per frame
    void Update()
    {
        if(alive)
        {
            playerMovement();
            fireLaser();
        }

        if(colorManager.magicLaser)
        {
            colorManager.Multicolor(this.gameObject);
        }
        Debug.Log(spawnProtection);
    }

    void playerMovement()
    {
        // Move Left
        if(Input.GetKey(KeyCode.LeftArrow) && transform.position.x > screenBoundLeft)
        {
            transform.position = transform.position + Vector3.left * tankSpeed * Time.deltaTime;
        }
        // Move Right
        else if(Input.GetKey(KeyCode.RightArrow) && transform.position.x < screenBoundRight)
        {
            transform.position = transform.position + Vector3.right * tankSpeed * Time.deltaTime;
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
        this.gameObject.transform.position = new Vector3(-6f, -4.69f, -2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if(collision.gameObject.layer == 11 && spawnProtection == false)
            {
                GameObject collided = collision.gameObject;
                Debug.Log("Ouch!");
                alive = false;
                death();
            }

    }
}
