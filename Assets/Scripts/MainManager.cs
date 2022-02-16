using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;

public class MainManager : MonoBehaviour
{
    
    public static string PlayerNameTx;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    public InputField playerName;
    
    private bool m_Started = false;
    private int m_Points;
    private int BestPoint;
    private string BestPlayerName;
    private bool m_GameOver = false;


    // Start is called before the first frame update
    private void Awake()
    {
        LoadPoint();


    }
    void Start()
    {
        string titleScene = SceneManager.GetSceneByName("title").name;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != titleScene)
        {
            BestScoreText.text = "BestScore:" + BestPlayerName + ": " + BestPoint;
            ScoreText.text = "0";
            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);

            int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
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
    }

    private void Update()
    {
        string titleScene = SceneManager.GetSceneByName("title").name;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != titleScene)
        {
            if (!m_Started)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_Started = true;
                    float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
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
            }
        }
            
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > BestPoint || BestScoreText == null)
        {
            SavePoint();
        }
        
    }
    public void StarNew()
    {
        if (playerName.text != "")
        {
            PlayerNameTx = playerName.text;
            Debug.Log(PlayerNameTx);
            SceneManager.LoadScene(1);
        }
        else
        {
            playerName.text = "Please input a name.";
        }
        
    }
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }
    [Serializable]
    public class PlayerData
    {
        public string playername;
        public int score;
    }
    public void SavePoint()
    {
        PlayerData myData = new PlayerData();
        myData.playername = PlayerNameTx;
        myData.score = m_Points;
        string json = JsonUtility.ToJson(myData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
    public void LoadPoint()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            BestPlayerName = playerData.playername;
            BestPoint = playerData.score;


        }
    }
}
