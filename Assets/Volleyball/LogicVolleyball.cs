using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LogicVolleyball : MonoBehaviour
{
    int playerScore = 0;
    public int ralliesRemaining;
    //public string pointEnder;

    [SerializeField] TMP_Text playerScoreText;
    [SerializeField] GameObject[] chancesLeftIcons;
    [SerializeField] TMP_Text pointEnderText;
    //[SerializeField] TMP_Text superSpeedText;
    //[SerializeField] TMP_Text doublePointsText;

    public GameObject gameOverScreen;
    public bool gameOver;
    [SerializeField] TMP_Text finalScoreText;

    int multiplier = 1;
    public bool doublePoints;
    float doublePointsDuration = 10;
    float doublePointsTime = 0;
    public GameObject doublePointsBox;
    public Image doublePointsBar;
    public TMP_Text doublePointsTimeText;

    public Transform ball;
    public GameObject floatingText;

    // Start is called before the first frame update
    void Start()
    {
        ralliesRemaining = LevelSelectionVolleyball.selectedLength;

        // only show as many icons as rallies to be played
        for (int i = ralliesRemaining; i < chancesLeftIcons.Length; i++)
        {
            chancesLeftIcons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                //UpdateEffectText("Double Points", false);
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
    public void StartRally()
    {
        UpdatePointEnderText("");
        ralliesRemaining--;

        // e.g. 2 rallies remaining, grey out 3rd icon (index 2)
        chancesLeftIcons[ralliesRemaining].GetComponent<Image>().color = Color.gray;
    }

    //public void UpdatePlayerScoreText()
    //{
    //    playerScoreText.text = "Score: " + playerScore;
    //}

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

    //public void UpdateEffectText(string effect, bool enabled)
    //{
    //    if (effect == "Super Speed")
    //    {
    //        if (enabled)
    //            superSpeedText.text = "Super Speed Enabled";
    //        else
    //            superSpeedText.text = "";
    //    }
    //    else if (effect == "Double Points")
    //    {
    //        if (enabled)
    //            doublePointsText.text = "Double Points Enabled";
    //        else
    //            doublePointsText.text = "";
    //    }
    //}

    public void EnableDoublePoints()
    {
        doublePointsTime += doublePointsDuration;
        doublePoints = true;
        doublePointsBox.SetActive(true);
        multiplier = 2;
        //UpdateEffectText("Double Points", true);
    }

    void ShowFloatingText(string pointsText, Color textColor)
    {
        GameObject pointsInstance = Instantiate(floatingText, ball.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = pointsText;
        pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().color = textColor;
    }
}
