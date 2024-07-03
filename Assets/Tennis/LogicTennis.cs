using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LogicTennis : MonoBehaviour
{
    public int playerScore = 0;
    public int opponentScore = 0;
    public int winningScore;
    public string pointEnder;

    [SerializeField] TMP_Text playerScoreText;
    [SerializeField] TMP_Text opponentScoreText;
    [SerializeField] TMP_Text raceToText;
    [SerializeField] TMP_Text pointEnderText;
    public GameObject matchPointBox;
    //[SerializeField] TMP_Text superSpeedText;
    //[SerializeField] TMP_Text stuckInTheMudText;

    public GameObject gameOverScreen;
    public bool gameOver;
    public TMP_Text finalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        winningScore = LevelSelectionTennis.selectedLength;
        raceToText.text = "Race to " + winningScore;
        //superSpeedText.text = "";
        //stuckInTheMudText.text = "";
    }

    public void UpdateScoreText()
    {
        playerScoreText.text = "Player: " + playerScore;
        opponentScoreText.text = "Opponent: " + opponentScore;
    }

    public void UpdatePointEnderText(string pointEnder)
    {
        pointEnderText.text = pointEnder;
    }

    public void MatchPoint(bool mp)
    {
        if (mp)
        {
            if (playerScore > opponentScore)
                matchPointBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(-500, 485);
            else
                matchPointBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(-500, 427);
        }
        matchPointBox.SetActive(mp);
    }

    public void GameOver()
    {
        if (playerScore > opponentScore)
            finalScoreText.text = "Player wins " + playerScore + " - " + opponentScore;
        else
            finalScoreText.text = "Opponent wins " + opponentScore + " - " + playerScore;
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
    //    else if (effect == "Stuck in the Mud")
    //    {
    //        if (enabled)
    //            stuckInTheMudText.text = "Stuck in the Mud Enabled";
    //        else
    //            stuckInTheMudText.text = "";
    //    }
    //}
}
