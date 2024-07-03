using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePointsVolleyball : MonoBehaviour
{
    float timer = 0;
    float lifespan;
    float[] availTimes = { 6, 8, 10, 12, 14 };

    SpecialEffectsVolleyball effectManager;
    //Volleyball ball;
    LogicVolleyball scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        effectManager = GameObject.FindGameObjectWithTag("Special Effects").GetComponent<SpecialEffectsVolleyball>();
        //ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Volleyball>();
        scoreManager = GameObject.Find("Logic Volleyball").GetComponent<LogicVolleyball>();

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
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Effect obtained");
            effectManager.EnableEffect("Double Points");
            Disappear();
        }
    }
}
