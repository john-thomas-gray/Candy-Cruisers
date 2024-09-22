using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    // Missile color
    public GameObject player;
    // Destroy Timer
    private float destroyTimer = 0;

    private float destroyPlain = 5.3f;
    public bool homing;
    private float initialZRotation;
    private float rotationSpeed = .3f;
    private float maxRotationDifference = 35f;

    // Raycast
    private float rayLength;
    private LayerMask layersToHit = 1 << 9;

    void Start()
    {
        player = GameObject.Find("Player");
        initialZRotation = transform.eulerAngles.z;
        rayLength = transform.localScale.y * .25f;

        if (gameObject.name.Contains("Homing"))
        {
            homing = true;
            // Debug.Log("working");
        }
        else
        {
            homing = false;
            // Debug.Log("not working");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveMissile();
        // Destroy missile if onscreen too long
        destroyTimer += Time.deltaTime;
        if(!homing && destroyTimer > 2f || destroyTimer > 5f)
        {
            Destroy(this.gameObject);
        }

        // Destroy off-screen missiles
        if(Mathf.Abs(transform.position.y) > destroyPlain)
        {
            Destroy(this.gameObject);
        }
    }
    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, layersToHit);
        if (hit.collider != null)
        {
            GameObject hitGameObject = hit.collider.gameObject;
            if(hitGameObject.layer == 9)
            {
                PlayerController playerControllerScript = hitGameObject.GetComponent<PlayerController>();
                if(playerControllerScript && !playerControllerScript.spawnProtection)
                {
                    playerControllerScript.hit();
                    Destroy(this.gameObject);
                }
            }
        }
    }

    void MoveMissile()
    {

        float missileSpeed = 5.0f;
        if (homing == false)
        {
            transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
        } else
        {
            Homing();
        }
    }

    void Homing()
    {
        float homingSpeed = 3.5f;
        // Debug.Log("homing");
        transform.Translate(Vector3.up * homingSpeed * Time.deltaTime);

        // Calculate potential new rotation values
        float newZRotationClockwise = transform.eulerAngles.z + rotationSpeed;
        float newZRotationCounterclockwise = transform.eulerAngles.z - rotationSpeed;

        if (player.transform.position.x > transform.position.x)
        {
            // Rotate clockwise if within bounds
            if (Mathf.Abs(newZRotationClockwise - initialZRotation) <= maxRotationDifference)
            {
                transform.rotation = Quaternion.Euler(0, 0, newZRotationClockwise);
            }
        }
        else if (player.transform.position.x < transform.position.x)
        {
            // Rotate counterclockwise if within bounds
            if (Mathf.Abs(newZRotationCounterclockwise - initialZRotation) <= maxRotationDifference)
            {
                transform.rotation = Quaternion.Euler(0, 0, newZRotationCounterclockwise);
            }
        }

    }

}
