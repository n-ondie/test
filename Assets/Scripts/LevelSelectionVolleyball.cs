using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionVolleyball : MonoBehaviour
{
    public static int gameMode = 2;

    public static int selectedLength = 5;
    [SerializeField] TMP_Dropdown gameLengthDropdown;

    // Start is called before the first frame update
    void Start()
    {
        gameLengthDropdown.value = 1; // default 5
    }

    public void SelectLength(int index)
    {
        switch (index)
        {
            case 0:
                selectedLength = 3; break;
            case 1:
                selectedLength = 5; break;
            case 2:
                selectedLength = 7; break;
        }
    }

    public void PlayVolleyball()
    {
        SceneManager.LoadScene("Scenes/Volleyball");
    }
}
