using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool alive;
    // Movement
    private float moveSpeed;
    private float screenBound = 3f;

    // ColorManager
    ColorManager colorManager;
    public bool colorSet;
    public string color;
    public GameObject Body;


    // Tongue

    public GameObject Tongue;
    public bool tongueReady = true;
    private bool magicTongue;

    // Death and Respawn
    public bool spawnProtection = false;
    private SpriteRenderer spriteRenderer;
    private float respawnTimer = 5;


    // GameOver
    public bool gameOver = false;

    [Header("Event Channels")]
    public VoidEventChannelSO gameOverEventChannel;
    public VoidEventChannelSO fleetWipeEC;
    public BoolEventChannelSO SetMagicTongueChannel;

    void Awake()
    {
        alive = true;
        moveSpeed = 5f;
        // Get ColorManager instance
        colorManager = ColorManager.Instance;

    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setColor();
    }

    private void OnEnable()
    {
        gameOverEventChannel.OnEventRaised+= GameOver;
        SetMagicTongueChannel.OnEventRaised += SetMagicTongue;
    }

    private void OnDisable()
    {
        gameOverEventChannel.OnEventRaised -= GameOver;
        SetMagicTongueChannel.OnEventRaised -= SetMagicTongue;
    }

    void Update()
    {
        if(alive && !gameOver && Tongue.GetComponent<Tongue>().deflected == false)
        {
            playerMovement();
            BallisticTongueProjection();
        }

        if(magicTongue && colorSet == false)
        {
            colorManager.Multicolor(this.gameObject);
            colorSet = true;
        }

        if(!alive && respawnTimer >= 5)
        {
            respawnTimer = 0;
        }
        if(respawnTimer < 5)
        {
            respawnPlayer();
        }

    }

    void playerMovement()
    {
        // Move Left
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + Vector3.left * moveSpeed * Time.deltaTime;
        }
        // Move Right
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + Vector3.right * moveSpeed * Time.deltaTime;
        }
        if(transform.position.x > screenBound)
        {
            transform.position = transform.position + Vector3.right * -2 * screenBound;
        }
        if(transform.position.x < -screenBound)
        {
            transform.position = transform.position + Vector3.right * 2 * screenBound;
        }
    }
    void BallisticTongueProjection()
    {
        colorSet = false;

        if( Input.GetKeyDown(KeyCode.Space) &&
            tongueReady == true
        )
        {
            Tongue.GetComponent<Tongue>().Project();
        }
    }

    public void setColor()
    {
        color = colorManager.RandomOnscreenColor();
        colorManager.SetColor(this.gameObject, color);

        Tongue.GetComponent<Tongue>().SetColor(color);

        colorManager.SetColor(Body, color);
    }

    public void death()
    {
        Vector3 deadPosition = this.gameObject.transform.position;
        deadPosition.y -= 0.25f;
        transform.position = deadPosition;
    }

    public void hit()
    {
        if(alive && !spawnProtection)
        {
            alive = false;
            death();
        }
    }

    void GameOver()
    {
        gameOver = true;
    }

    private IEnumerator BlinkSprite()
    {
        Debug.Log("Blinking");
        int count = 0;
        while (count < 6)
        {
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.enabled = !spriteRenderer.enabled;
            count ++;
        }
    }

    public void respawnPlayer()
   {
    float respawnTime = 1.5f;
    float iFrames = 3f;
    respawnTimer += Time.deltaTime;
    if(respawnTimer >= respawnTime && alive == false)
    {
        Vector3 newPosition = transform.position;
        newPosition.y += 0.25f;
        transform.position = newPosition;
        spawnProtection = true;
        alive = true;
        StartCoroutine(BlinkSprite());
    } else if (respawnTimer >= iFrames)
    {
        spawnProtection = false;
    }
   }

   private void SetMagicTongue(bool b)
    {
        magicTongue = b;
    }
}
