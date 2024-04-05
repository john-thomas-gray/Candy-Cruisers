using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelManagerSO")]
public class LevelManagerSO : ScriptableObject
{
    public int level = 0;
    public int startingLevel = 0;

    // Scoring
    [SerializeField]
    private ScoreManagerSO scoreManager;

    private void OnEnable() {
        scoreManager.scoreChangeEvent.AddListener(LevelUp);
    }

    private void LevelUp(int score) {
        if(level == 0 && score >= 24)
        {
            level = 1;
        }
        Debug.Log("Score: " + score);
    }
}
