using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LogicSquash : MonoBehaviour
{
    public int playerScore = 0;
    //public string pointEnder;
    [SerializeField] TMP_Text playerScoreText;
    [SerializeField] TMP_Text pointEnderText;

    public GameObject gameOverScreen;
    public bool gameOver;
    [SerializeField] TMP_Text finalScoreText;

    float gameDuration;
    public float gameTimeRemaining;
    public TMP_Text timeRemainingText;
    public Image timeRemainingRing;

    int multiplier = 1;
    public bool doublePoints;
    float doublePointsDuration = 10;
    float doublePointsTime = 0;
    public GameObject doublePointsBox;
    public Image doublePointsBar;
    public TMP_Text doublePointsTimeText;
    float extraTimeIncrement = 10;

    public Transform ball;
    public GameObject floatingText;

    // Start is called before the first frame update
    void Start()
    {
        gameDuration = LevelSelectionSquash.selectedDuration;
        gameTimeRemaining = gameDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // Game countdown
        if (gameTimeRemaining > 0)
        {
            gameTimeRemaining -= Time.deltaTime;
            timeRemainingText.text = Mathf.CeilToInt(gameTimeRemaining).ToString();
            timeRemainingRing.fillAmount = Mathf.Min(gameDuration, gameTimeRemaining) / gameDuration;
        }
        else if (!ball.GetComponent<SquashBall>().inPlay && !gameOver)
        {
            GameOver();
        }

        // Super Score countdown
        if (doublePoints && !gameOver)
        {
            if (doublePointsTime > 0)
            {
                doublePointsTime -= Time.deltaTime;
                doublePointsTimeText.text = Mathf.CeilToInt(doublePointsTime).ToString();
                doublePointsBar.fillAmount = Mathf.Min(doublePointsDuration, doublePointsTime) / doublePointsDuration;
            }
            else
            {
                doublePoints = false;
                doublePointsBox.SetActive(false);
                doublePointsTime = 0;
                multiplier = 1;
            }
        }
    }

    public void AddScore(int points, bool coin)
    {
        string addScoreText;
        Color addScoreColor;

        // Super Score effect
        if (points > 0)
        {
            points *= multiplier;
            addScoreText = "+" + points;
            addScoreColor = Color.green;
        }
        else
        {
            addScoreText = points.ToString();
            addScoreColor = Color.red;
        }

        playerScore += points;
        if (playerScore < 0)
            playerScore = 0;
        playerScoreText.text = "Score: " + playerScore;

        if (!coin)
            ShowFloatingText(addScoreText, addScoreColor);
    }

    public void UpdatePointEnderText(string pointEnder)
    {
        pointEnderText.text = pointEnder;
    }

    public void GameOver()
    {
        finalScoreText.text = "Final Score: " + playerScore;
        gameOverScreen.SetActive(true);
        gameOver = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Menu button clicked");
        SceneManager.LoadScene("Scenes/Menu");
    }

    public void EnableDoublePoints()
    {
        doublePointsTime += doublePointsDuration;
        doublePoints = true;
        doublePointsBox.SetActive(true);
        multiplier = 2;
    }

    public void AddExtraTime()
    {
        gameTimeRemaining += extraTimeIncrement;
    }

    void ShowFloatingText(string pointsText, Color textColor)
    {
        GameObject pointsInstance = Instantiate(floatingText, ball.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = pointsText;
        pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().color = textColor;
    }
}
