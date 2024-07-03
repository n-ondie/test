using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add this line to access TextMeshPro components

public class LapTimeManager : MonoBehaviour
{
    public static int MinuteCount;
    public static int SecondCount;
    public static float MilliCount;
    public static string MilliDisplay;

    public GameObject MinuteBox;
    public GameObject SecondBox;
    public GameObject MilliBox;
    
    // Update is called once per frame
    void Update()
    {
        
        MilliCount += Time.deltaTime * 10;  // counting the time
        MilliDisplay = MilliCount.ToString("F0");   // convert it to string
        MilliBox.GetComponent<TextMeshProUGUI>().text = "" + MilliDisplay;    // update it to the component

        if (MilliCount > 9)
        {
            MilliCount = 0;
            SecondCount += 1;
        }
        
        if (SecondCount >= 60)
        {
            SecondCount = 0;
            MinuteCount += 1;
        }

        if (SecondCount <= 9)
        {
            SecondBox.GetComponent<TextMeshProUGUI>().text = "0" + SecondCount + ".";
        }
        else
        {
            SecondBox.GetComponent<TextMeshProUGUI>().text = "" + SecondCount + ".";
        }
        
        if (MinuteCount <= 9)
        {
            MinuteBox.GetComponent<TextMeshProUGUI>().text = "0" + MinuteCount + ":";
        }
        else
        {
            MinuteBox.GetComponent<TextMeshProUGUI>().text = "" + MinuteCount + ":";
        }
    }
}
