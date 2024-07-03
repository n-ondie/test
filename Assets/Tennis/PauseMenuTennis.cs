using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuTennis : MonoBehaviour
{
    public bool gamePaused = false;
    float gameTimescale;

    public GameObject pauseMenuUI;
    LogicTennis scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.Find("Logic Tennis").GetComponent<LogicTennis>();

        if (LevelSelectionTennis.gameMode == 0)
            gameTimescale = 1f;
        else if (LevelSelectionTennis.gameMode == 1)
            gameTimescale = 0.8f;
        else if (LevelSelectionTennis.gameMode == 2)
            gameTimescale = 0.8f;
        Time.timeScale = gameTimescale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !scoreManager.gameOver)
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = gameTimescale;
        gamePaused = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Menu button clicked");
        SceneManager.LoadScene("Scenes/Menu");
        Time.timeScale = 1f;
    }
}
