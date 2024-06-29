using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    public GameObject player;
    public string color;
    private bool colorSet;
    ColorManager colorManager;
    private bool deflected = false;
    public bool retracting = false;
    private float originY;
    private bool magicTongue = false;
    private bool unstoppable = false;
    private float maxLength = 4.95f;
    public int maxSpeed = 20;
    private float speedFactor = 0f;
    private float speed() {
        return maxSpeed * speedFactor;
    }

    // Raycast
    private float rayLength;
    private LayerMask layersToHit = (1 << 8 | 1 << 10); // Combine enemy and shield layers

    public BoolEventChannelSO SetMagicTongueChannel;
    private void OnEnable()
    {
        SetMagicTongueChannel.OnEventRaised += SetMagicTongue;
    }
    private void OnDisable()
    {
        SetMagicTongueChannel.OnEventRaised -= SetMagicTongue;
    }
    void Start()
    {
        // Get ColorManager instance
        colorManager = ColorManager.Instance;
        rayLength = transform.localScale.y * .25f;
        speedFactor = 0f;
        originY = transform.position.y;
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed() * Time.deltaTime);
        Debug.Log("Retracting: " + retracting);

        if (transform.position.y >= maxLength)
        {
            Retract();
        }

        if (player.GetComponent<PlayerController>().alive && transform.position.y < originY)
        {
            transform.position = new Vector3(transform.position.x, originY, transform.position.z);
            if(retracting == true)
            {
                TongueReset();
                retracting = false;
            }
        }
        if (magicTongue)
        {
            colorManager.Multicolor(this.gameObject);
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
            if (retracting == false && hit.collider != null)
            {
                GameObject hitGameObject = hit.collider.gameObject;
                if(hit.collider.gameObject.layer == 8)
                {
                    Enemy enemyScript = hitGameObject.GetComponent<Enemy>();
                    string enemyColor = enemyScript.color;
                    if(enemyScript != null)
                    {
                        enemyScript.hit(color, magicTongue);
                        if(color == enemyColor && !unstoppable)
                        {
                            Retract();
                            Debug.Log("FixedUpdateRetract");
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
        player.GetComponent<PlayerController>().tongueReady = false;
        colorSet = false;

    }

    public void Retract()
    {
        retracting = true;
        speedFactor = -1.5f;
        if (magicTongue)
        {
            colorManager.magicTongue = true;
        }
        if (unstoppable)
        {
            unstoppable = false;
            SetMagicTongueChannel.RaiseEvent(false);
            colorManager.magicTongue = false;
        }
    }
    public void TongueReset()
    {
        Debug.Log("Tongue Reset");
        if(colorSet == false)
        {
            player.GetComponent<PlayerController>().setPlayerColor();
            colorSet = true;
        }
        player.GetComponent<PlayerController>().tongueReady = true;
        if (unstoppable == false && magicTongue)
        {
            unstoppable = true;
        }

    }

    public void SetColor(string color)
    {
        colorManager.SetColor(this.gameObject, color);
    }

    private void SetMagicTongue(bool b)
    {
        magicTongue = b;
    }
}
