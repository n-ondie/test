using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderVolleyballNet : MonoBehaviour
{
    public Transform ball;
    LogicVolleyball scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.Find("Logic Volleyball").GetComponent<LogicVolleyball>();
    }

    // end the point if the ball is in play and goes under the net
    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        if (other.CompareTag("Ball") && ball.GetComponent<Volleyball>().inPlay)
        {
            scoreManager.UpdatePointEnderText("Under net");
            if (ball.GetComponent<Volleyball>().hitter == "player")
            {
                ball.GetComponent<Volleyball>().EndPoint(-1);
            }
            else if (ball.GetComponent<Volleyball>().hitter == "opponent")
            {
                ball.GetComponent<Volleyball>().EndPoint(4);
            }
        }
    }
}
