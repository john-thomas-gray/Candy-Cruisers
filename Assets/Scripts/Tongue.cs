using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tongue : MonoBehaviour
{
    public GameObject player;
    public LevelManagerSO LevelManager;
    public ScoreManagerSO ScoreManager;
    public TMP_Text comboText;
    Color actualEnemyColor;
    public string color;
    private bool colorSet;
    ColorManager colorManager;
    private bool enemyHit = false;
    public bool deflected = false;
    public bool retracting = false;
    private float originY;
    private bool magicTongue = false;
    private int magicValue = 0;
    private bool magicReset = false;
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
    public IntEventChannelSO SetMagicValueChannel;
    public VoidEventChannelSO fleetWipeEC;
    private void OnEnable()
    {
        SetMagicTongueChannel.OnEventRaised += SetMagicTongue;
        SetMagicValueChannel.OnEventRaised += SetMagicValue;
        fleetWipeEC.OnEventRaised += FleetWipeMagicReset;
    }
    private void OnDisable()
    {
        SetMagicTongueChannel.OnEventRaised -= SetMagicTongue;
        SetMagicValueChannel.OnEventRaised -= SetMagicValue;
        fleetWipeEC.OnEventRaised -= FleetWipeMagicReset;
    }
    void Awake()
    {
        colorManager = ColorManager.Instance;
    }
    void Start()
    {
        rayLength = transform.localScale.y * .25f;
        speedFactor = 0f;
        originY = transform.position.y;
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed() * Time.deltaTime);

        if (transform.position.y >= maxLength)
        {
            Retract();
        }

        if (player.GetComponent<PlayerController>().alive && transform.position.y <= originY)
        {
            transform.position = new Vector3(transform.position.x, originY, transform.position.z);
            if(retracting == true)
            {
                TongueReset();
                retracting = false;
            }
        }
        if (magicValue > 0)
        {
            colorManager.Multicolor(this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("magic Value: " + magicValue);
        }

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
                    actualEnemyColor = hitGameObject.GetComponent<SpriteRenderer>().color;
                    string enemyColor = enemyScript.color;
                    if(enemyScript != null)
                    {
                        enemyScript.hit(color, magicValue);

                        if (magicValue > 0)
                        {
                            enemyHit = true;
                        }
                        else if(color == enemyColor)
                        {
                            enemyHit = true;
                            Retract();
                        }
                        else if(color != "Yellow" && enemyScript.isImitation)
                        {
                            StartCoroutine(enemyScript.reveal());
                        }
                    }
                }
                else if(hit.collider.gameObject.layer == 10 && magicValue == 0)
                {
                    Shield shieldScript = hitGameObject.GetComponent<Shield>();

                    if(color != shieldScript.shieldEnemyColor)
                    {
                        // BASIC ABSORB
                        if (!shieldScript.special)
                        {
                            shieldScript.deactivate();
                            Retract();
                        }
                        // SPECIAL DEFLECT
                        // deflected bool keeps raycast from firing multiple times
                        if (deflected == false && shieldScript.special)
                        {
                            Retract();
                            deflected = true;
                            // deflected retract speed scales inversely with level
                            speedFactor = -0.25f + (.01f * LevelManager.level);
                        }

                    }
                }
            }
        }
        // Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * rayLength, Color.red);
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
    }
    public void TongueReset()
    {
        if (magicValue > 0)
        {
            SetMagicValueChannel.RaiseEvent(-1);
        }
        if (magicTongue)
        {
            SetMagicValueChannel.RaiseEvent(1);
            SetMagicTongueChannel.RaiseEvent(false);
        }
        if (magicReset)
        {
            SetMagicValueChannel.RaiseEvent(-10);
            magicReset = false;
        }

        if(magicValue == 0 && colorSet == false && player.GetComponent<PlayerController>().alive)
        {
            ScoreManager.magicMultiplier = 1;
            player.GetComponent<PlayerController>().setColor();
            colorSet = true;
        }

        if (magicValue > 0)
        {
            ScoreManager.magicMultiplier += 1;
        }

        if (enemyHit)
        {
            ScoreManager.comboMultiplier += 1;
            comboText.color = actualEnemyColor;
        }
        else
        {
            ScoreManager.comboMultiplier = 1;
        }
        enemyHit = false;

        deflected = false;

        player.GetComponent<PlayerController>().tongueReady = true;
    }

    public void SetColor(string inputColor)
    {
        colorManager.SetColor(this.gameObject, inputColor);
    }

    private void SetMagicTongue(bool b)
    {
        magicTongue = b;
    }

    private void SetMagicValue(int i)
    {
        magicValue += i;
        magicValue = magicValue > 2 ? 2 : magicValue;
        magicValue = magicValue < 0 ? 0 : magicValue;
    }

    private void FleetWipeMagicReset()
    {
        magicReset = true;
    }
}
