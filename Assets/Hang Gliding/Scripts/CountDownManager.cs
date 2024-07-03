using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add this line to access TextMeshPro components

public class DownManager : MonoBehaviour
{
    public int Minute;
    public int Second;
    public static float Milli = 0;
    public static string MilliDisplay;

    public GameObject MinuteBox;
    public GameObject SecondBox;
    public GameObject MilliBox;

    private void Start()
    {
        SecondBox.GetComponent<TextMeshProUGUI>().text = Second.ToString("D2") + ".";
        MinuteBox.GetComponent<TextMeshProUGUI>().text = Minute.ToString("D2") + ":";
        MilliBox.GetComponent<TextMeshProUGUI>().text = Milli.ToString();    // update it to the component


    }

    // Update is called once per frame
    void Update()
    {
        
        Milli -= Time.deltaTime * 10;  // counting the time
        if (Milli <= 0)
        {
            Milli = 9;
            Second -= 1;
        }
        MilliDisplay = Milli.ToString("F0");   // convert it to string
        MilliBox.GetComponent<TextMeshProUGUI>().text = "" + MilliDisplay;    // update it to the component
        
        if (Second <= 0)
        {
            Second = 59;
            Minute -= 1;
        }
        
        SecondBox.GetComponent<TextMeshProUGUI>().text = Second.ToString("D2") + ".";
        MinuteBox.GetComponent<TextMeshProUGUI>().text = Minute.ToString("D2") + ":";

    }
}
