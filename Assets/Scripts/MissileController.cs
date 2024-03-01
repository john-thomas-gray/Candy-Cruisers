using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    // Missile color
    public GameObject player;
    // Missile movement
    private float deletePlain = -5.3f;
    private float missileSpeed = 5.0f;
    // ColorManager

    void Start()
    {
        GameObject player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
    {
        MoveMissile();

    }

    void MoveMissile()
    {

        transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);

        // Delete offscreen missile
        if(transform.position.y < deletePlain)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if(collision.gameObject.layer == 9)
            {
                GameObject collided = collision.gameObject;
                PlayerController playerScript = collided.GetComponent<PlayerController>();
                Debug.Log("Ouch!");
                playerScript.alive = false;
                Destroy(this.gameObject);

            }

    }
}
