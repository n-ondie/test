using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SportSelection : MonoBehaviour
{
    public void PlayVolleyball()
    {
        SceneManager.LoadScene("Scenes/Volleyball");
    }

    public void PlayTennis()
    {
        SceneManager.LoadScene("Scenes/Tennis");
    }

    public void PlaySquash()
    {
        SceneManager.LoadScene("Scenes/Squash");
    }

    public void PlayCanoeing()
    {
        SceneManager.LoadScene("Scenes/Canoeing");
    }

    public void PlayWindsurfing()
    {
        SceneManager.LoadScene("Scenes/Windsurfing");
    }

}
