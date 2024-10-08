using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/LevelManagerSO")]
public class LevelManagerSO : ScriptableObject
{
    public int level = 1;

    [SerializeField]
    private static readonly int startingLevel = 1;

    private const int baseEnemies = 18;

    // Scoring
    [SerializeField]
    private ScoreManagerSO scoreManager;

    // Broadcasting
    public IntEventChannelSO updateGlobalLevelChannel;
    public IntEventChannelSO enemyDestroyedECSO;

    private void OnEnable()
    {
        level = startingLevel;

        // if (scoreManager != null)
        // {
        //     scoreManager.scoreChangeEvent.AddListener(LevelUp);
        // }

        if (updateGlobalLevelChannel != null)
        {
            updateGlobalLevelChannel.RaiseEvent(level);
        }

        enemyDestroyedECSO.OnEventRaised += LevelUp;

    }

    private void OnDisable()
    {
        // if (scoreManager != null)
        // {
        //     scoreManager.scoreChangeEvent.RemoveListener(LevelUp);
        // }
        enemyDestroyedECSO.OnEventRaised -= LevelUp;
    }

    private void LevelUp(int enemies_destroyed)
    {
        if (enemies_destroyed >= EnemiesForLevelUp(level, baseEnemies))
        {
            level += 1;
            Debug.Log("Leveled up to Level " + level + " at " + enemies_destroyed + " enemies destroyed");

            if (updateGlobalLevelChannel != null)
            {
                updateGlobalLevelChannel.RaiseEvent(level);
            }
        }
    }

    public static int EnemiesForLevelUp(int currentLevel, int baseEnemies)
{
    if (currentLevel < 1)
        return 0; // Return 0 for invalid level entries

    int totalEnemiesDestroyed = 0;

    // Sum the enemies required for each level up to the current level
    for (int level = 1; level <= currentLevel; level++)
    {
        totalEnemiesDestroyed += baseEnemies * level;
    }

    return totalEnemiesDestroyed;
}
}
