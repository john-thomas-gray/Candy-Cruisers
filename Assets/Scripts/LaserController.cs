using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    // Laser color
    public string color;
    public string shotColor;
    public GameObject player;
    private bool magicLaser = false;
    // Laser movement
    private float destroyPlain = 5.3f;
    private float laserSpeed = 20f;
    // ColorManager
    ColorManager colorManager;
    // Max time on screen
    private float destroyTimer = 0;

    // Raycast

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        shotColor = player.GetComponent<PlayerController>().shotColor;
        colorManager = ColorManager.Instance;
        colorManager.SetColor(this.gameObject, shotColor);
        if(colorManager.magicLaser)
        {
            magicLaser = true;
        }
        colorManager.magicLaser = false;

    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
        if(magicLaser)
        {
            colorManager.Multicolor(this.gameObject);
        }
        // Destroy laser if onscreen too long
        destroyTimer += Time.deltaTime;
        if(destroyTimer > 1.5)
        {
            Destroy(this.gameObject);
        }

    }

    void MoveLaser()
    {

        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);

        // Destroy offscreen laser
        if(Mathf.Abs(transform.position.y) > destroyPlain)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            // Hitting an enemy
            if(collision.gameObject.layer == 8)
            {
                GameObject collided = collision.gameObject;
                Enemy enemy = collided.GetComponent<Enemy>();
                string enemyColor = enemy.color;
                if(enemy != null && magicLaser)
                {
                    enemy.alive = false;
                    enemy.checkNeighbors();
                }
                else if(enemy != null && enemyColor == color)
                {
                    enemy.alive = false;
                    enemy.checkNeighbors();
                    Destroy(this.gameObject);
                }

            }
            // Hitting a shield
            if (magicLaser == false && collision.gameObject.layer == 10)
            {
                GameObject collided = collision.gameObject;
                Shield shieldScript = collided.GetComponent<Shield>();
                float reflectRotation = 180f;
                if (color != shieldScript.shieldEnemyColor)
                {
                    // BASIC ABSORB
                    if (!shieldScript.special)
                    {
                        shieldScript.absorb();
                        Destroy(this.gameObject);
                    }
                    // SPECIAL DEFLECT
                    if (shieldScript.special)
                    {
                        colorManager.turnWhite(this.gameObject);
                        transform.Rotate(new Vector3(0f, 0f, reflectRotation));
                        laserSpeed = laserSpeed * 0.5f;
                        this.gameObject.layer = 11;
                    }
                }
            }

    }
}
