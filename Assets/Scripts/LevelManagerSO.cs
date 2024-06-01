using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/LevelManagerSO")]
public class LevelManagerSO : ScriptableObject
{
    public int level = 1;
    [SerializeField]
    public const int startingLevel = 1;

    private const int BasePoints = 1500;
    private const double GrowthFactor = 1.5;

    // Scoring
    [SerializeField]
    private ScoreManagerSO scoreManager;

    // Broadcasting
    public IntEventChannelSO updateGlobalLevelChannel;

    private void OnEnable() {
        level = startingLevel;
        if (scoreManager != null)
        {
            scoreManager.scoreChangeEvent.AddListener(LevelUp);
        }
        updateGlobalLevelChannel.RaiseEvent(level);

    }

    private void OnDisable() {
        if (scoreManager != null)
        {
            scoreManager.scoreChangeEvent.RemoveListener(LevelUp);
        }
    }

    private void LevelUp(int score) {
        if (score >=  PointsForLevelUp(level))
        {
            level += 1;
            Debug.Log("Leveled up to Level " + level);
            if (updateGlobalLevelChannel != null)
            {
                updateGlobalLevelChannel.RaiseEvent(level);
            }
        }
    }

    public static int PointsForLevelUp(int currentLevel)
    {
        if (currentLevel < 1)
            return 0; // Return 0 for invalid level entries

        // Using the geometric series sum formula to calculate total points required up to the next level
        double totalPoints = BasePoints * (Math.Pow(GrowthFactor, currentLevel) - 1) / (GrowthFactor - 1);
        return (int)Math.Round(totalPoints);
    }
}
