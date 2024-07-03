using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CanoeGameManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public GameObject pauseMenu;

    private float startTime;
    private bool isRunning;
    private bool isPaused;
    private float pausedTime;
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        isRunning = true;
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }
        
        if (isRunning && !isPaused)
        {
            float timePassed = Time.time - startTime;
            string minutes = Mathf.Floor(timePassed / 60).ToString("00");
            string seconds = (timePassed % 60).ToString("00");
            timeText.text = "Time: " + minutes + ":" + seconds;
        }
    }

     public void RestartGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene("Menu"); 
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenu.SetActive(false); // Hide the pause menu
    }

    public void TogglePauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            isPaused = true;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
            pauseMenu.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            startTime += Time.time - pausedTime;
            isPaused = false;
        }
        else
        {
            pausedTime = Time.time;
            isPaused = true;
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }
    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }
    public void ResetTimer()
    {
        startTime = Time.time;
        isPaused = false;
    }

}
