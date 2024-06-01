using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreManager", menuName = "ScriptableObjects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    public int score = 0;
    public int startingScore = 0;
    public int enemies_destroyed = 0;

    [System.NonSerialized]
    public UnityEvent<int> scoreChangeEvent;

    private void OnEnable() {
        score = startingScore;
        enemies_destroyed = 0;
        if (scoreChangeEvent == null)
        {
            scoreChangeEvent = new UnityEvent<int>();
        }
    }

    public void IncreaseScore(int amount) {
        score += amount;
        enemies_destroyed += 1;
        scoreChangeEvent.Invoke(score);
    }
}
