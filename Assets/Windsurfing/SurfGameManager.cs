using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SurfGameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public TextMeshProUGUI animalCountText;
    public TextMeshProUGUI scoreText;
    public AudioSource soundEffect;

    private int animalCount = 0; 
    private int playerScore = 0;
    private bool isPaused = false;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateAnimalCountText();
        UpdateScoreText(); 
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }
        UpdateAnimalCountText();
        UpdateScoreText(); 
    }

    public void IncrementAnimalCount()
    {
        animalCount++;
        UpdateAnimalCountText();
    }
    private void UpdateAnimalCountText()
    {
        animalCountText.text = "Animals Found: " + animalCount;
    }

    public void AddScore(int score)
    {
        playerScore += score;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + playerScore;
    }

    private void TogglePauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            isPaused = false;
            soundEffect.UnPause();
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
            soundEffect.Pause();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        soundEffect.UnPause();
        
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenu.SetActive(false); // Hide the pause menu
        soundEffect.UnPause();
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene("Menu"); 
    }
}
