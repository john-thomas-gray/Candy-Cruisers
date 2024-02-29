using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    // Laser color
    public string color;
    public string shotColor;
    public GameObject player;
    // Laser movement
    private float deletePlain = 5.3f;
    private float laserSpeed = 20f;
    // ColorManager
    ColorManager colorManager;


    void Start()
    {
        GameObject player = GameObject.Find("Player");
        shotColor = player.GetComponent<PlayerController>().shotColor;
        colorManager = ColorManager.Instance;
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
