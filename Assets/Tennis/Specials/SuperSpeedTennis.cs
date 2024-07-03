using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperSpeedTennis : MonoBehaviour
{
    float timer = 0;
    float lifespan;
    float[] availTimes = { 6, 8, 10, 12, 14 };

    SpecialEffectsTennis effectManager;
    LogicTennis scoreManager;
    Ball ball;

    // Start is called before the first frame update
    void Start()
    {
        effectManager = GameObject.FindGameObjectWithTag("Special Effects").GetComponent<SpecialEffectsTennis>();
        scoreManager = GameObject.Find("Logic Tennis").GetComponent<LogicTennis>();
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();

        // effect appears for a random length of time
        int randNum = Random.Range(0, availTimes.Length);
        lifespan = availTimes[randNum];
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < lifespan && !scoreManager.gameOver)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Disappear();
        }
    }

    void Disappear()
    {
        effectManager.effectsOnScreen--;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        if (other.CompareTag("Ball") && ball.inPlay && ball.hitter == "player")
        {
            //Debug.Log("Effect obtained");
            effectManager.EnableEffect("Super Speed");
            Disappear();
        }
    }
}
