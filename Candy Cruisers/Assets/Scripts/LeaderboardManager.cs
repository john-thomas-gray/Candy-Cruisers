using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public ScoreManagerSO scoreManagerSO;
    public VoidEventChannelSO gameOverEventChannel;
    private int count = 0;
    public TextMeshProUGUI leaderboardScores;
    private void OnEnable()
    {
        gameOverEventChannel.OnEventRaised += SaveHighScore;
    }
    private void OnDisable()
    {
        gameOverEventChannel.OnEventRaised -= SaveHighScore;
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            StartCoroutine(GetTopScores());
        }
    }

    private string serverUrl = "http://127.0.0.1:3000";
    public IEnumerator PostScore(string playerName, int score)
    {
        string json = JsonUtility.ToJson(new Score { playerName = playerName, score = score });

        UnityWebRequest request = new UnityWebRequest($"{serverUrl}/scores/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Score added successfully");
        }
    }

    private void SaveHighScore()
    {
        StartCoroutine(PostScore("Suerte", scoreManagerSO.score));
        Debug.Log(count);
        count++;
    }

    public IEnumerator GetTopScores()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{serverUrl}/scores");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);
            DisplayScores(scoreList);
        }
    }

    private void DisplayScores(ScoreList scoreList)
{
    leaderboardScores.text = "";
    int rank = 1;
    foreach (var score in scoreList.scores)
    {
        leaderboardScores.text += $"{rank}. {score.playerName} - {score.score}\n";
        rank++;
    }
}
    [System.Serializable]
    public class Score
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    public class ScoreList
    {
        public Score[] scores;
    }
}
