using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    // Laser color
    public string color = "Green";
    public string shotColor;
    public GameObject player;
    // Laser movement
    private float deletePlain = 5.3f;
    private float laserSpeed = 20f;
    // Destruction
    // private GridManager gridManagerInstance;
    // private GameObject fleet;
    ColorManager colorManager;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        shotColor = player.GetComponent<PlayerController>().shotColor;
        // gridManagerInstance = fleet.GetComponent<GridManager>();
        colorManager = ColorManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorManager.SetColor(this.gameObject, shotColor);

    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
    }

    void MoveLaser()
    {

        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);

        // Delete offscreen laser
        if(Mathf.Abs(transform.position.y) > deletePlain)
        {
            Destroy(this.gameObject);
        }
    }

    // void SetColor()
    // {
    //     Color purple = new Color(1f, 0f, 1f, 1f);
    //     SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    //     List<Color> skins = new List<Color> {Color.red, Color.yellow, Color.blue, Color.green, purple};
    //     List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };
    //     // // Get color of last shot
    //     // color = player.GetComponent<PlayerController>().shotColor;
    //     // Set the laser's tag to the appropriate color
    //     color = shotColor;
    //     this.gameObject.tag = color;
    //     // Get index of item in list
    //     int colorInx = colors.IndexOf(color);
    //     // Set the sprite's color
    //     Color spriteColor = skins[colorInx];
    //     spriteRenderer.color = spriteColor;
    // }


    private void OnTriggerEnter2D(Collider2D collision)
    {
            if(collision.gameObject.layer == 8)
            {
                GameObject collided = collision.gameObject;
                Enemy enemy = collided.GetComponent<Enemy>();
                string enemyColor = enemy.color;
                if(enemy != null && enemyColor == color)
                {
                    enemy.alive = false;
                    enemy.CheckNeighbors();
                    Destroy(this.gameObject);
                    // gridManagerInstance.FleetShift();
                }

            }
    }
}
