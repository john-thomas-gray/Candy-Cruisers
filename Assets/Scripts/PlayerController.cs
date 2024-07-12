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

    // Touch Controls
    public Touch theTouch;
    private Vector3 targetPosition;
    private bool moveToTarget = false;
    private Vector2 startTouchPosition;
    private bool swipeReady = true;
    private bool shouldFire = false;


    void Awake()
    {
        alive = true;
        moveSpeed = 5f;
        // Get ColorManager instance
        colorManager = ColorManager.Instance;
    }

    void Start()
    {
        targetPosition = transform.position;
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
            if(Input.GetKeyDown(KeyCode.Space))
            {
                BallisticTongueProjection();
            }
            // DetectSwipe();
        }

        if(magicTongue && colorSet == false)
        {
            colorManager.Multicolor(this.gameObject);
            // colorManager.Multicolor(Tongue);
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

        // Keyboard
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + Vector3.left * moveSpeed * Time.deltaTime;
        }
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
        // Touch Controls
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                if (touch.position.y > 200)
                {
                    shouldFire = true;
                }
            }
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                worldPosition.y = transform.position.y;
                targetPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);
                moveToTarget = true;
            }


        }

        if (moveToTarget)
        {
            // Calculate distances
            float distToLeftEdge = Mathf.Abs(-screenBound - transform.position.x);
            float distToRightEdge = Mathf.Abs(screenBound - transform.position.x);
            float distFromLeftEdgeToTarget = Mathf.Abs(-screenBound - targetPosition.x);
            float distFromRightEdgeToTarget = Mathf.Abs(screenBound - targetPosition.x);
            float directDistToTarget = Mathf.Abs(targetPosition.x - transform.position.x);

            // Determine if warping is shorter from the left side
            bool warpFromLeft = (distToLeftEdge + distFromRightEdgeToTarget) < directDistToTarget;

            // Determine if warping is shorter from the right side
            bool warpFromRight = (distToRightEdge + distFromLeftEdgeToTarget) < directDistToTarget;

            if (warpFromLeft || warpFromRight)
            {
                // Move towards the closest edge first
                if (warpFromLeft && transform.position.x < 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(-screenBound, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
                }
                else if (warpFromRight && transform.position.x > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(screenBound, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
                }

                // Check if reached the edge to warp
                if (Mathf.Abs(transform.position.x - (-screenBound)) < 0.1f)
                {
                    transform.position = new Vector3(screenBound, transform.position.y, transform.position.z);
                }
                else if (Mathf.Abs(transform.position.x - screenBound) < 0.1f)
                {
                    transform.position = new Vector3(-screenBound, transform.position.y, transform.position.z);
                }
            }
            else
            {
                // Directly move towards the target
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            // Check if reached the target position
            if (transform.position == targetPosition && shouldFire)
            {
                moveToTarget = false;
                // DetectSwipe();
                BallisticTongueProjection();
                shouldFire = false;
            }
        }
    }
    void DetectSwipe()
    {
        // if (Input.touchCount > 0)
        // {
        //     Debug.Log("Swiping");
        //     Touch touch = Input.GetTouch(0);
        //     Vector2 lastTouchPosition = touch.position;
        //     Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //     Debug.Log("lTP.y " + lastTouchPosition.y);
        //     Debug.Log("lTP.x " + lastTouchPosition.x);


        //     if(lastTouchPosition.y >= 400)
        //     {
        //         BallisticTongueProjection();
        //         lastTouchPosition = Vector2.zero;
        //     }

        // }
    }
    void BallisticTongueProjection()
    {
        colorSet = false;

        if(tongueReady == true)
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
        moveToTarget = false;
    }

    public void hit()
    {
        if(alive && !spawnProtection)
        {
            alive = false;
            death();
            Tongue.GetComponent<Tongue>().Retract();
        }
    }

    void GameOver()
    {
        gameOver = true;
    }

    private IEnumerator BlinkSprite()
    {
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
