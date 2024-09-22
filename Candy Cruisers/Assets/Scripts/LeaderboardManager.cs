using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardManager : MonoBehaviour
{
    public ScoreManagerSO scoreManagerSO;
    public VoidEventChannelSO gameOverEventChannel;
    private int count = 0;
    private void OnEnable()
    {
        gameOverEventChannel.OnEventRaised += SaveHighScore;
    }
    private void OnDisable()
    {
        gameOverEventChannel.OnEventRaised -= SaveHighScore;
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
}

[System.Serializable]
public class Score
{
    public string playerName;
    public int score;
}
