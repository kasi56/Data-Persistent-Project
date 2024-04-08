using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    public GameObject LeaderBoard;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private ScoreData sd;

    private void Awake()
    {
        LeaderBoard.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        ScoreText.text = $"Score : 0 Name:{ManPage.Instance.PlayerName}";
        LoadRank();
        var score = GetHighScores().ToArray();
        BestScoreText.text = $"Best Score : {score[0].score} Name : {score[0].name}";

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LeaderBoard.SetActive(true);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points} Name:{ManPage.Instance.PlayerName}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        //�������а񣬸�����������
        if(sd.scores.Count != 0)
        {
            foreach (var palyer in sd.scores)
            {
                if (palyer.name == ManPage.Instance.PlayerName)
                {
                    palyer.score = m_Points > palyer.score ? m_Points : palyer.score;
                    SaveScore();
                    return; 
                }
            }
            AddScore(new Score(ManPage.Instance.PlayerName, m_Points));
        }
        else
        {
            AddScore(new Score(ManPage.Instance.PlayerName, m_Points));
        }
        SaveScore();
    }

    public IEnumerable<Score> GetHighScores()
    {
        return sd.scores.OrderByDescending(x => x.score);
    }
    public void AddScore(Score score)
    {
        sd.scores.Add(score);
    }

    public void SaveScore()
    {
        string json = JsonUtility.ToJson(sd);
        PlayerPrefs.SetString("scores", json);
    }

    void LoadRank()
    {
        sd = new ScoreData();
        string json = PlayerPrefs.GetString("scores");
        if (!string.IsNullOrEmpty(json))
        {
            sd = JsonUtility.FromJson<ScoreData>(json);

        }
        else
        {
            AddScore(new Score(ManPage.Instance.PlayerName, m_Points));
        }
    }
}
