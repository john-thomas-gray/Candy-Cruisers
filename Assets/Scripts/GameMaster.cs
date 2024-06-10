using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private static GameMaster gameMaster;
    // public GameObject gameOverScreen;

    // Player
    public GameObject player;
    public PlayerController playerController;
    public float respawnTimer = 5;
    public static GameMaster Instance
    {
        get
        {
            // If the colorManager doesn't exist, create it
            if (gameMaster == null)
            {
                GameObject gameMasterObject = new GameObject("GameMaster");
                gameMaster = gameMasterObject.AddComponent<GameMaster>();
            }

            return gameMaster;
        }
    }

    void Awake()
    {
        Screen.SetResolution(1080, 1920, true);
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            restartGame();
        }
        if(!playerController.alive && respawnTimer >= 5)
        {
            respawnTimer = 0;
        }
        if(respawnTimer < 5)
        {
            respawnPlayer();
        }
        // Debug.Log(respawnTimer);
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
//    public void gameOver()
//    {
//     gameOverScreen.SetActive(true);
//    }

   public void respawnPlayer()
   {
    // Debug.Log("respawn");
    float respawnTime = 3f;
    float iFrames = 5f;
    respawnTimer += Time.deltaTime;
    if(respawnTimer >= respawnTime && playerController.alive == false)
    {
        player.transform.position = new Vector3(0f, -4.69f, -2f);
        playerController.spawnProtection = true;
        playerController.alive = true;
    } else if (respawnTimer >= iFrames)
    {
        playerController.spawnProtection = false;
    }
   }
}
