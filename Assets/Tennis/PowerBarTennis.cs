using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerBarTennis : MonoBehaviour
{
    public Slider slider;
    //public Gradient gradient;
    public int maxPower = 20;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = 0;
    }

    public void ChargePower(int points)
    {
        if (points > 0)
            slider.value = Mathf.Min(maxPower, slider.value + points);
        else
            slider.value = Mathf.Max(0, slider.value + points);
    }

    public void ClearPower()
    {
        slider.value = 0;
    }
}
