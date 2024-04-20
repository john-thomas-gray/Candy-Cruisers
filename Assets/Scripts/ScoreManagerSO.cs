using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreManager", menuName = "ScriptableObjects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    public int score = 0;
    public int startingScore = 0;

    [System.NonSerialized]
    public UnityEvent<int> scoreChangeEvent;

    private void OnEnable() {
        score = startingScore;
        if (scoreChangeEvent == null)
        {
            scoreChangeEvent = new UnityEvent<int>();
        }
    }

    public void IncreaseScore(int amount) {
        score += amount;
        scoreChangeEvent.Invoke(score);
    }

}
