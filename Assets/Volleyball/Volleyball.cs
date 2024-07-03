using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Volleyball : MonoBehaviour
{
    //Vector3 initialPos;
    public string hitter;
    public bool inPlay;
    public int hits;
    public Vector3 aimedPos;
    public string playerShot;

    public Transform player;
    public Transform opponent;
    public Transform cam;

    AudioSource bounceAudio;
    public AudioClip netAudio;
    public AudioClip hitWall;
    public AudioClip outCall;

    LogicVolleyball scoreManager;
    bool pointEnded;
    private float timer = 0;
    private int resetTime = 4;

    // Start is called before the first frame update
    void Start()
    {
        bounceAudio = GetComponent<AudioSource>();
        bounceAudio.volume = 0.15f;
        scoreManager = GameObject.Find("Logic Volleyball").GetComponent<LogicVolleyball>();
        //initialPos = transform.position;
        hits = 0;
        aimedPos = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // when a rally ends, players reset for next point when timer reaches resetTime
        if (timer < resetTime && pointEnded)
        {
            timer += Time.deltaTime;
        }
        else if (timer >= resetTime && pointEnded)
        {
            StopBall();

            if (scoreManager.ralliesRemaining > 0)
            {
                player.GetComponent<PlayerVolleyball>().ResetPlayer();
                opponent.GetComponent<OpponentVolleyball>().ResetOpponent();
            }
            else
            {
                scoreManager.GameOver();
            }

            GetComponent<TrailRenderer>().enabled = false;
            timer = 0;
            pointEnded = false;
        }
    }

    public void StopBall()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void EndPoint(int scoreChange)
    {
        inPlay = false;
        pointEnded = true;
        scoreManager.AddScore(scoreChange, false);
        //scoreManager.UpdatePointEnderText();
    }

    // what to do when the ball hits something
    private void OnCollisionEnter(Collision collision) // 'collision' is the object the ball has collided into
    {
        if (collision.transform.CompareTag("Net") && inPlay)
        {
            AudioSource.PlayClipAtPoint(netAudio, transform.position, 0.3f);
            scoreManager.UpdatePointEnderText("Net");
            if (hitter == "player")
            {
                EndPoint(-1);
            }
            else if (hitter == "opponent")
            {
                EndPoint(4);
            }
        }
        if (collision.transform.CompareTag("Wall") && inPlay)
        {
            AudioSource.PlayClipAtPoint(hitWall, transform.position, 0.6f);
            AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
            scoreManager.UpdatePointEnderText("Out");
            if (hitter == "player")
            {
                EndPoint(-1);
            }
            else if (hitter == "opponent")
            {
                EndPoint(4);
            }
        }

        if (collision.transform.CompareTag("Player Court") && inPlay) // ball landed on player's side of court
        {
            bounceAudio.PlayOneShot(bounceAudio.clip, 0.3f);
            if (hitter == "player")
            {
                if (hits == 3 && aimedPos == new Vector3(0, 0, 0)) // failed to select a shot within 3 hits
                    scoreManager.UpdatePointEnderText("3 hits");
                else
                    scoreManager.UpdatePointEnderText("Own side");
            }
            else if (hitter == "opponent")
                scoreManager.UpdatePointEnderText("In");

            EndPoint(-1);
        }

        else if (collision.transform.CompareTag("Opponent Court") && inPlay) // ball landed on opponent's side of court
        {
            bounceAudio.PlayOneShot(bounceAudio.clip, 0.3f);
            if (hitter == "player")
            {
                scoreManager.UpdatePointEnderText("In");
                EndPoint(5);
            }
            else if (hitter == "opponent")
            {
                scoreManager.UpdatePointEnderText("Own side");
                EndPoint(4);
            }
        }

        else if (collision.transform.CompareTag("Ground") && inPlay) // ball landed out
        {
            bounceAudio.PlayOneShot(bounceAudio.clip, 0.3f);
            AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
            scoreManager.UpdatePointEnderText("Out");
            if (hitter == "player")
            {
                EndPoint(-1);
            }
            else if (hitter == "opponent")
            {
                EndPoint(4);
            }
        }
    }
}
