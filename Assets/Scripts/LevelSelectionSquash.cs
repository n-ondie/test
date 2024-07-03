using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionSquash : MonoBehaviour
{
    public static int gameMode = 2;

    public static int selectedDuration = 60;
    //public int duration;
    //public TMP_Text durationText;
    [SerializeField] TMP_Dropdown durationDropdown;

    // Start is called before the first frame update
    void Start()
    {
        durationDropdown.value = 1; // default duration is 60 seconds
    }

    public void SelectDuration(int index)
    {
        switch (index)
        {
            case 0:
                selectedDuration = 30; break;
            case 1:
                selectedDuration = 60; break;
            case 2:
                selectedDuration = 90; break;
        }
    }

    public void PlaySquash()
    {  
        //selectedDuration = duration;
        SceneManager.LoadScene("Scenes/Squash");
    }
}
