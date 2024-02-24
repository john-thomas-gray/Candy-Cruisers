// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerController : MonoBehaviour
// {
//     static System.Random random = new System.Random();
//     public string lastShotColor = "Green";
//     public string color;
//     public bool alive;
//     public float tankSpeed;
//     private float screenBoundLeft = -2.45f;
//     private float screenBoundRight = 2.45f;
//     public GameObject laserPrefab;

//     private GameObject fleet;
//     private GridManager gridManagerInstance;
//     private Dictionary<string, int> colorCounts;
//     private int enemyTotal;


//     void Awake()
//     {
//         alive = true;
//         tankSpeed = 6f;
//         // Get fleet GO and Grid script
//         GameObject fleet = GameObject.Find("Fleet");
//         gridManagerInstance = fleet.GetComponent<GridManager>();
//         // Create a reference to the colorCounts dictionary
//         colorCounts = gridManagerInstance.colorCounts;
//         // Create a reference to the enemyTotal int
//         enemyTotal = gridManagerInstance.enemyTotal;

//     }

//     void Start()
//     {
//         SetColor();
//     }
//     // Update is called once per frame
//     void Update()
//     {
//         if(alive)
//         {
//             playerMovement();
//             fireLaser();
//         }
//     }

//     void playerMovement()
//     {
//         if(Input.GetKey(KeyCode.LeftArrow) && transform.position.x > screenBoundLeft)
//         {
//             transform.position = transform.position + Vector3.left * tankSpeed * Time.deltaTime;
//         }
//         else if(Input.GetKey(KeyCode.RightArrow) && transform.position.x < screenBoundRight)
//         {
//             transform.position = transform.position + Vector3.right * tankSpeed * Time.deltaTime;
//         }
// }
//     void fireLaser()
//     {
//         if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
//         {
//             lastShotColor = color;
//             Instantiate(laserPrefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation);
//             // GameObject laser = Instantiate(laserPrefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation);
//             SetColor();
//         }
//     }

//     public void SetColor()
//     {
//         Color purple = new Color(1f, 0f, 1f, 1f);
//         SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
//         // !! MIGHT NEED TO CHANGE THE SCOPE OF THESE !!
//         List<Color> skins = new List<Color> {Color.red, Color.yellow, Color.blue, Color.green, purple};
//         List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };
//         bool colorSet = false;
//         int recursionLimit = 5;
//         // Check if a like-colored enemy exists
//             // THERE IS PROBABLY A MORE EFFICIENT WAY TO DO THIS

//         while(colorSet == false && recursionLimit > 0 && enemyTotal > 0)
//         {
//             // Set color string to a random "color"
//             int randomIndex = random.Next(colors.Count);
//             color = colors[randomIndex];

//             // Check if randomly selected color is the same as any onscreen enemies
//             if (colorCounts[color] > 0)
//             {
//                 // Set the sprite's color
//                 Color spriteColor = skins[randomIndex];
//                 spriteRenderer.color = spriteColor;
//                 colorSet = true;
//             }
//             recursionLimit--;
//         }
//         // Prevent endless loop in the case all enemies are dead
//         if (recursionLimit <= 0)
//         {
//             // Set color string to a random "color"
//             int randomIndex = random.Next(colors.Count);
//             color = colors[randomIndex];
//             // Set the sprite's color
//             Color spriteColor = skins[randomIndex];
//             spriteRenderer.color = spriteColor;

//         }
//     }
// }
