using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static System.Random random = new System.Random();
    public bool alive;
    // Movement
    private float moveSpeed;
    private float touchMoveSpeed;
    private float screenBound = 3f;


    // ColorManager
    ColorManager colorManager;
    public bool colorSet;
    public string color;
    public GameObject Body;
    public GameObject EyeL;
    public GameObject EyeR;

    // Animation
    private SquashAndStretch squashAndStretch;


    // Tongue
    public GameObject Tongue;
    public bool tongueReady = true;
    private int magicValue;
    private float touchFireRange = 100f;
    private bool outOfRange = false;

    // Death and Respawn
    public bool spawnProtection = false;
    private SpriteRenderer spriteRenderer;
    private float respawnTimer = 5;
    private float lastTouch;


    // GameOver
    public bool gameOver = false;

    [Header("Event Channels")]
    public VoidEventChannelSO gameOverEventChannel;
    public VoidEventChannelSO fleetWipeEC;
    public IntEventChannelSO SetMagicValueChannel;

    // Touch Controls
    public Touch theTouch;
    private Vector3 targetPosition;
    private bool moveToTarget = false;
    private Vector2 startTouchPosition;
    private bool shouldFire = false;


    void Awake()
    {
        alive = true;
        moveSpeed = 5f;
        touchMoveSpeed = 20f;
        // Get ColorManager instance
        colorManager = ColorManager.Instance;
        lastTouch = Body.transform.position.x;
        squashAndStretch = GetComponent<SquashAndStretch>();

    }

    void Start()
    {
        targetPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        setColor();
    }

    private void OnEnable()
    {
        gameOverEventChannel.OnEventRaised += GameOver;
        SetMagicValueChannel.OnEventRaised += SetMagicValue;
    }

    private void OnDisable()
    {
        gameOverEventChannel.OnEventRaised -= GameOver;
        SetMagicValueChannel.OnEventRaised -= SetMagicValue;
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
        }

        if(magicValue > 0)
        {
            colorManager.Multicolor(this.gameObject);
        }

    }
    void FixedUpdate()
    {
        respawnPlayer();
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
            Debug.Log("Touch.y " + touch.position.y);

            if(touch.position.y < 2050)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    outOfRange = false;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    startTouchPosition = touch.position;
                    if (touch.position.x >= lastTouch - touchFireRange && touch.position.x <= lastTouch + touchFireRange && !outOfRange)
                    {
                        shouldFire = true;
                    }

                    lastTouch = touch.position.x;


                }
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                    worldPosition.y = transform.position.y;
                    targetPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);
                    moveToTarget = true;
                    if (touch.position.x <= lastTouch - touchFireRange || touch.position.x >= lastTouch + touchFireRange)
                    {
                        outOfRange = true;
                        Debug.Log("Out of range: " + outOfRange);
                    }

                }
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
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(-screenBound, transform.position.y, transform.position.z), touchMoveSpeed * Time.deltaTime);
                }
                else if (warpFromRight && transform.position.x > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(screenBound, transform.position.y, transform.position.z), touchMoveSpeed * Time.deltaTime);
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
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, touchMoveSpeed * Time.deltaTime);
            }

            // Check if reached the target position
            if (shouldFire)
            {
                moveToTarget = false;
                BallisticTongueProjection();
                shouldFire = false;
            }
        }
    }
    void BallisticTongueProjection()
    {

        if(tongueReady == true)
        {
            squashAndStretch.CheckForAndStartCoroutine();
            Tongue.GetComponent<Tongue>().Project();
        }
    }

    public void setColor()
    {
        color = colorManager.RandomOnscreenColor();
        colorManager.SetColor(this.gameObject, color);

        Tongue.GetComponent<Tongue>().SetColor(color);

        colorManager.SetColor(Body, color);
        colorSet = true;
    }

    public void death()
    {
        SetMagicValueChannel.RaiseEvent(-10);
        colorManager.turnWhite(this.gameObject);
        colorSet = false;
        Vector3 deadPosition = this.gameObject.transform.position;
        deadPosition.y -= 0.25f;
        transform.position = deadPosition;
        moveToTarget = false;
    }

    public void hit()
    {
        if(alive && spawnProtection == false)
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
            Tongue.GetComponent<SpriteRenderer>().enabled = !Tongue.GetComponent<SpriteRenderer>().enabled;
            Body.SetActive(!Body.activeSelf);
            // EyeL.SetActive(!EyeL.activeSelf);
            // EyeR.SetActive(!EyeR.activeSelf);
            count ++;
        }
    }

    public void respawnPlayer()
    {

        if(!alive && respawnTimer >= 3)
        {
            respawnTimer = 0;
        }
        if(respawnTimer < 3)
        {
            // Make most of this into a coroutine alla BlinkSprite
            float respawnTime = 1.5f;
            float iFrames = 3f;
            respawnTimer += Time.deltaTime;
            if(respawnTimer >= respawnTime && alive == false)
            {
                if (colorSet == false)
                {
                    setColor();

                }
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
    }

    private void SetMagicValue(int i)
    {
        magicValue += i;
        magicValue = magicValue > 2 ? 2 : magicValue;
        magicValue = magicValue < 0 ? 0 : magicValue;
    }

}
