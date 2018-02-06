using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour {

    public int score;
    public Text scoreBoard;
    public static ScoreKeeper Instance;

    void Start()
    {
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoad;
        DontDestroyOnLoad(gameObject);
        scoreBoard = GameObject.Find("Score").GetComponent<Text>();
        updateScoreBoard();
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        scoreBoard = GameObject.Find("Score").GetComponent<Text>();
        updateScoreBoard();
    }

    public void incrementScore()
    {
        score++;
        scoreBoard.text = "Score: " + score;
    }

    public void resetScore()
    {
        score = 0;
        scoreBoard.text = "Score: " + score;
    }

    public void updateScoreBoard()
    {
        if(scoreBoard == null) { return; }
        scoreBoard.text = "Score: " + score;
    }

    public void Lose()
    {
        SceneManager.LoadScene("Lose");
    }

    public void Restart()
    {

    }
}
