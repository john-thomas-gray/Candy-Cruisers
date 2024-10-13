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
    public TMP_Text combo;
    public TMP_Text magicCombo;

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
        // This should be called from somewhere else
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
        // Change so Score Change event sets this
        scoreInGame.SetText(scoreManager.score.ToString());
        levelCounterInGame.SetText("L: " + levelManager.level);
        if (scoreManager.comboMultiplier > 1)
        {
            combo.SetText("x" + scoreManager.comboMultiplier.ToString());
        }
        else
        {
            combo.SetText("");
        }

        if (scoreManager.magicMultiplier > 1)
        {
            magicCombo.SetText("x" + scoreManager.magicMultiplier.ToString());
        }
        else
        {
            magicCombo.SetText("");
        }

        if (eventSystem.currentSelectedGameObject == null)
        {
            // Check if the up or down arrow keys are pressed
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                eventSystem.SetSelectedGameObject(gameOverRestartButton.gameObject);
            }
        }

    }

    public void Pause()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(playButton.gameObject);
        }

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
        scoreManager.enemiesDestroyed = 0;
        scoreManager.comboMultiplier = 1;
        scoreManager.magicMultiplier = 1;
        levelManager.level = 1;
        Time.timeScale = 1;
        eventSystem.firstSelectedGameObject = playButton.gameObject;

    }
    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        scoreManager.score = 0;
        scoreManager.enemiesDestroyed = 0;
        scoreManager.comboMultiplier = 1;
        scoreManager.magicMultiplier = 1;
        levelManager.level = 1;
        Time.timeScale = 1;
    }
    private void GameOver()
    {
        scoreManager.comboMultiplier = 1;
        scoreManager.magicMultiplier = 1;
        eventSystem.SetSelectedGameObject(null);
        scoreGameOver.text = scoreManager.score.ToString();
        gameOverMenu.SetActive(true);

        StartCoroutine(WaitAndSelectButton());
    }

    private IEnumerator WaitAndSelectButton()
    {
        yield return new WaitForSeconds(3.0f);

        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(gameOverRestartButton.gameObject);
        }
    }
}
