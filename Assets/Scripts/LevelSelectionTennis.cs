using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionTennis : MonoBehaviour
{
    public static int gameMode = 1;

    public static int selectedLength = 7;
    [SerializeField] TMP_Dropdown gameLengthDropdown;

    // Start is called before the first frame update
    void Start()
    {
        gameLengthDropdown.value = 1; // default 7
    }

    public void SelectLength(int index)
    {
        switch (index)
        {
            case 0:
                selectedLength = 5; break;
            case 1:
                selectedLength = 7; break;
            case 2:
                selectedLength = 10; break;
        }
    }

    public void PlayTennis()
    {
        SceneManager.LoadScene("Scenes/Tennis");
    }
}
