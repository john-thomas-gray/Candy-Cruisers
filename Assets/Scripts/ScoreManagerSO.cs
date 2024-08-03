using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "ScoreManager", menuName = "ScriptableObjects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    // Change score to a long
    public int score = 0;
    public int startingScore = 0;
    public int enemiesDestroyed = 0;
    public int comboMultiplier = 1;
    public int magicMultiplier = 1;

    public LevelManagerSO LevelManager;

    [System.NonSerialized]
    public UnityEvent<int> scoreChangeEvent;
    public IntEventChannelSO enemyDestroyedECSO;
    public VoidEventChannelSO fleetWipeEC;


    private void OnEnable() {
        score = startingScore;
        enemiesDestroyed = 0;
        if (scoreChangeEvent == null)
        {
            scoreChangeEvent = new UnityEvent<int>();
        }
        fleetWipeEC.OnEventRaised += FleetWipe;
    }

    private void OnDisable()
    {
        fleetWipeEC.OnEventRaised -= FleetWipe;
    }
    public void IncreaseScore(int amount, int chainMultiplier = 1) {
        score += (amount + (10 * (LevelManager.level - 1))) * chainMultiplier * comboMultiplier * magicMultiplier;
        enemiesDestroyed += 1;
        enemyDestroyedECSO.RaiseEvent(enemiesDestroyed);
        scoreChangeEvent.Invoke(score);
    }

    public void FleetWipe()
    {
        score += 10000 * LevelManager.level;
        scoreChangeEvent.Invoke(score);
    }
}
