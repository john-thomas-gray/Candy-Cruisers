using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool colorSet;
    public string shotColor;
    public string color;
    public bool alive;
    public float tankSpeed;
    private float screenBoundLeft = -2.45f;
    private float screenBoundRight = 2.45f;
    public GameObject laserPrefab;

    private GameObject fleet;
    private GridManager gridManagerInstance;
    private bool wipedOut;

    // Color Manager
    private Dictionary<string, int> colorCounts;
    ColorManager colorManager;

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
        if(Input.GetKey(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
        {
                // Debug.Log("shotColor: " + shotColor);
                // Debug.Log("color: " + color);
            colorCounts = colorManager.colorCounts;

            Debug.Log("COLORSET: " + colorManager.colorSet);
            if (colorManager.colorSet)
            {
                // Set independent shotColor variable, so missile can reference it once player changes colors
                shotColor = colorManager.shotColor;
                // Spawn laser in front of player
                Instantiate(laserPrefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation);
                // Make player a different color
                colorManager.SetColor(this.gameObject);
            }
        }
    }
}
