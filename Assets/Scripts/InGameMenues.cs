using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InGameMenues : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] VoidEventChannelSO gameOverEventChannel;
    [SerializeField] ScoreManagerSO scoreManager;
    [SerializeField] LevelManagerSO levelManager;
    public TMP_Text scoreGameOver;
    public TMP_Text scoreInGame;

    public TMP_Text levelCounterInGame;

    public EventSystem eventSystem;
    public Button gameOverRestartButton;
    public Button playButton;

    private void OnEnable()
    {
        gameOverEventChannel.OnEventRaised+= GameOver;
    }

    private void OnDisable()
    {
        gameOverEventChannel.OnEventRaised -= GameOver;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
        // Change so Score Change event sets this
        scoreInGame.SetText(scoreManager.score.ToString());
        levelCounterInGame.SetText("L: " + levelManager.level);

    }

    public void Pause()
    {
        if(pauseMenu.activeInHierarchy == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else if(pauseMenu.activeInHierarchy == true)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    // make restart event channel
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        scoreManager.score = 0;
        scoreManager.enemies_destroyed = 0;
        levelManager.level = 1;
        Time.timeScale = 1;
        eventSystem.firstSelectedGameObject = playButton.gameObject;

    }
    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        scoreManager.score = 0;
        scoreManager.enemies_destroyed = 0;
        levelManager.level = 1;
        Time.timeScale = 1;
    }
    private void GameOver()
    {
        eventSystem.firstSelectedGameObject = gameOverRestartButton.gameObject;
        scoreGameOver.SetText(scoreManager.score.ToString());
        gameOverMenu.SetActive(true);
    }
}
