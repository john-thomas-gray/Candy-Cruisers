// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MissileController : MonoBehaviour
// {
//     public GameObject player;
//     static System.Random random = new System.Random();
//     public string color;
//     public bool launched;
//     public bool alive;
//     public float tankSpeed;
//     private float missileSpeed = 10f;
//     private float deletePlain = 5.3f;

//     void Awake()
//     {

//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         Debug.Log("START!");
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         color = player.GetComponent<PlayerController>().color;
//         alive = player.GetComponent<PlayerController>().alive;
//         Debug.Log(alive);
//         Debug.Log(color);
//         launched = true;
//         // Debug.Log("missile launched " + launched);
//         transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
//         if(Mathf.Abs(transform.position.y) > deletePlain)
//         {
//             launched = false;
//             // Debug.Log("missile launched " + launched);
//             Destroy(gameObject);
//         }
//     }

//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         if(collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
//             {
//                 launched = false;
//                 Destroy(gameObject);
//             }
//     }

//     private void setColor()
//     {
//         Color purple = new Color(1f, 0f, 1f, 1f);
//         SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
//         // !! MIGHT NEED TO CHANGE THE SCOPE OF THESE !!
//         List<Color> skins = new List<Color> {Color.red, Color.yellow, Color.blue, Color.green, purple};
//         List<string> colors = new List<string> { "Red", "Yellow", "Blue", "Green", "Purple" };
//         // Get color of last shot
//         color = player.GetComponent<PlayerController>().lastShot;
//         // Set the enemy's tag to the appropriate color
//         this.gameObject.tag = color;
//         // Get index of item in list
//         int colorInx = colors.IndexOf(color);
//         Debug.Log("missile color: " + color);
//         // Set the sprite's color
//         Color spriteColor = skins[colorInx];
//         spriteRenderer.color = spriteColor;
//     }
// }
