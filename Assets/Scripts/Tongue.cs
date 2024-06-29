using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    public GameObject player;
    public string color;
    private bool deflected = false;
    private float originY;
    private bool magicTongue = false;
    private float maxLength = 4.95f;
    public int maxSpeed = 20;
    private float speedFactor = 0f;
    private float speed() {
        return maxSpeed * speedFactor;
    }

    // Raycast
    private float rayLength;
    private LayerMask layersToHit = (1 << 8 | 1 << 10); // Combine enemy and shield layers

    void Start()
    {
        rayLength = transform.localScale.y * .25f;
        speedFactor = 0f;
        originY = transform.position.y;
    }

    void Update()
{
    transform.Translate(Vector3.up * speed() * Time.deltaTime);

    if (Mathf.Abs(transform.position.y) > maxLength)
    {
        Retract();
    }

    if (player.GetComponent<PlayerController>().alive && transform.position.y <= originY)
    {
        transform.position = new Vector3(transform.position.x, originY, transform.position.z);
    }
}

    void FixedUpdate()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position,
                                                    Vector2.up,
                                                    rayLength,
                                                    layersToHit);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                GameObject hitGameObject = hit.collider.gameObject;
                if(hit.collider.gameObject.layer == 8)
                {
                    Enemy enemyScript = hitGameObject.GetComponent<Enemy>();
                    string enemyColor = enemyScript.color;
                    if(enemyScript != null)
                    {
                        enemyScript.hitByLaser(color, magicTongue);
                        if(color == enemyColor && !magicTongue)
                        {
                            Retract();
                        }
                    }
                }
                else if(hit.collider.gameObject.layer == 10 && !magicTongue)
                {
                    Shield shieldScript = hitGameObject.GetComponent<Shield>();

                    if(color != shieldScript.shieldEnemyColor)
                    {
                        // BASIC ABSORB
                        if (!shieldScript.special)
                        {
                            // Destroy shield
                            Retract();
                        }
                        // SPECIAL DEFLECT
                        // deflected bool keeps raycast from firing multiple times
                        if (deflected == false && shieldScript.special)
                        {
                            Retract();
                            deflected = true;
                            // colorManager.turnWhite(this.gameObject);
                            speedFactor = -0.5f;
                        }

                    }
                }
            }
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * rayLength, Color.red);
    }

    public void Project()
    {
        speedFactor = 1.0f;
    }

    public void Retract()
    {
        speedFactor = -1.5f;
    }

}
