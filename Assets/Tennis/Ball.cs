using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Ball : MonoBehaviour
{
    Vector3 initialPos;
    public string hitter;
    public bool inPlay = false;
    public int bounces;
    //public bool deuceCurrent = true; // first point is on deuce side
    public int rotation = 0;
    public bool justServed;
    bool pointEnded;
    public bool fault;
    bool faultBreak;

    public Transform player;
    public Transform opponent;
    public Transform cam;

    AudioSource bounceAudio;
    public AudioClip netAudio;
    public AudioClip hitWall;
    public AudioClip outCall;
    public AudioClip faultCall;

    LogicTennis scoreManager;
    private float timer = 0;
    private int resetTime = 3;

    public PowerBarTennis powerBar;
    public bool powerShot;

    // Start is called before the first frame update
    void Start()
    {
        bounceAudio = GetComponent<AudioSource>();
        scoreManager = GameObject.Find("Logic Tennis").GetComponent<LogicTennis>();
        //initialPos = transform.position;
        bounces = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // when a rally ends, players reset for next point when timer reaches resetTime
        if (timer < resetTime && (pointEnded || faultBreak))
        {
            timer += Time.deltaTime;
        }
        else if (timer >= resetTime && (pointEnded || faultBreak))
        {
            StopBall();

            // check if a player has won; must win by 2;
            if (Mathf.Max(scoreManager.playerScore, scoreManager.opponentScore) >= scoreManager.winningScore && 
                Mathf.Abs(scoreManager.playerScore - scoreManager.opponentScore) >= 2)
            {
                scoreManager.GameOver();
            }
            else
            {
                if (pointEnded)
                {
                    // check if it is now match point
                    if (Mathf.Max(scoreManager.playerScore, scoreManager.opponentScore) >= scoreManager.winningScore - 1 &&
                    scoreManager.playerScore != scoreManager.opponentScore)
                    {
                        scoreManager.MatchPoint(true);
                    }
                    else
                        scoreManager.MatchPoint(false);
                    //deuceCurrent = !deuceCurrent; // change sides for next point
                    rotation = (rotation + 1) % 4;
                    pointEnded = false;
                }
                else
                    faultBreak = false;

                GetComponent<TrailRenderer>().enabled = false;
                player.GetComponent<Player>().ResetPlayer();
                opponent.GetComponent<Opponent>().ResetOpponent();
            }

            timer = 0;
        }

        // in case terrain collider didn't work
        if (transform.position.y < 0 && inPlay)
        {
            if (bounces >= 1)
            {
                scoreManager.UpdatePointEnderText("2 bounces");
                if (hitter == "player")
                    EndPoint("player");
                else if (hitter == "opponent")
                    EndPoint("opponent");
            }
            else if (bounces == 0) // hit straight into the wall without hitting the court
            {
                if (justServed)
                {
                    ServeOut();
                }
                else
                {
                    AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
                    scoreManager.UpdatePointEnderText("Out");
                    if (hitter == "player")
                        EndPoint("opponent");
                    else if (hitter == "opponent")
                        EndPoint("player");
                }
            }
        }
    }

    void FixedUpdate()
    {
        // speed up the ball after increasing scale to match volleyball (x2.4)
        if (powerShot)
            GetComponent<Rigidbody>().AddForce(Physics.gravity * 0.26f);
        else
            GetComponent<Rigidbody>().AddForce(Physics.gravity * 0.15f);
    }

    public void EndPoint(string pointWinner)
    {
        inPlay = false;
        powerShot = false;
        pointEnded = true;
        fault = false;
        if (pointWinner == "player")
            scoreManager.playerScore++;
        else if (pointWinner == "opponent")
        {
            scoreManager.opponentScore++;
            powerBar.ChargePower(-2);
        }
        scoreManager.UpdateScoreText();
    }

    public void ServeOut()
    {
        AudioSource.PlayClipAtPoint(faultCall, cam.position, 0.7f);
        if (fault) // second serve fault
        {
            scoreManager.UpdatePointEnderText("Double fault");
            if (hitter == "player")
                EndPoint("opponent");
            else if (hitter == "opponent")
                EndPoint("player");
        }
        else // first serve fault
        {
            inPlay = false;
            fault = true;
            faultBreak = true;
            scoreManager.UpdatePointEnderText("Fault");
            scoreManager.UpdateScoreText();
            powerBar.ChargePower(-1);
        }
    }

    public void StopBall()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // what to do when the ball hits something
    private void OnCollisionEnter(Collision collision) // 'collision' is the object the ball has collided into
    {
        if (collision.transform.CompareTag("Ground"))
        {
            if (inPlay)
            {
                bounceAudio.PlayOneShot(bounceAudio.clip, 0.5f);
                bounces++;

                if (bounces == 1)
                {
                    if (hitter == "player" && transform.position.z < 0 && transform.position.z > -2)
                    {
                        scoreManager.UpdatePointEnderText("Net tape");
                        EndPoint("opponent");
                    }
                    else if (hitter == "opponent" && transform.position.z > 0 && transform.position.z < 2)
                    {
                        scoreManager.UpdatePointEnderText("Net tape");
                        EndPoint("player");
                    }
                }
                else if (bounces == 2) // first bounce was in the court, second bounce ends the point
                {
                    scoreManager.UpdatePointEnderText("2 bounces");
                    if (hitter == "player")
                        EndPoint("player");
                    else if (hitter == "opponent")
                        EndPoint("opponent");
                }
            }
        }

        else if (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Terrain"))
        {
            StopBall();

            if (bounces == 1 && inPlay) // hit the wall after bouncing in the court
            {
                AudioSource.PlayClipAtPoint(hitWall, transform.position, 0.6f);
                scoreManager.UpdatePointEnderText("2 bounces");
                if (hitter == "player")
                    EndPoint("player");
                else if (hitter == "opponent")
                    EndPoint("opponent");
            }
            else if (bounces == 0 && inPlay) // hit straight into the wall without hitting the court
            {
                AudioSource.PlayClipAtPoint(hitWall, transform.position, 0.6f);
                if (justServed)
                {
                    ServeOut();
                }
                else
                {
                    AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
                    scoreManager.UpdatePointEnderText("Out");
                    if (hitter == "player")
                        EndPoint("opponent");
                    else if (hitter == "opponent")
                        EndPoint("player");
                }
            }
        }

        else if (collision.transform.CompareTag("Net"))
        {
            StopBall();

            if (justServed && inPlay)
            {
                ServeOut();
            }
            else if (inPlay)
            {
                AudioSource.PlayClipAtPoint(netAudio, cam.position, 0.7f);
                scoreManager.UpdatePointEnderText("Net");
                if (hitter == "player")
                    EndPoint("opponent");
                else if (hitter == "opponent")
                    EndPoint("player");
            }
        }
    }

    // end the point if the ball lands out
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Out") && inPlay && bounces == 0)
        {
            if (justServed)
            {
                ServeOut();
            }
            else
            {
                AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
                scoreManager.UpdatePointEnderText("Out");
                if (transform.position.z > 20) // adjust if needed
                    EndPoint("opponent");
                else if (transform.position.z < -20)
                    EndPoint("player");
                else if (hitter == "player")
                    EndPoint("opponent");
                else if (hitter == "opponent")
                    EndPoint("player");
            }
        }

        else if (justServed && (rotation == 0 || rotation == 2) && (other.CompareTag("Fault") || other.CompareTag("FaultDeuce")) && inPlay && bounces == 0)
        {
            ServeOut();
        }

        else if (justServed && (rotation == 1 || rotation == 3) && (other.CompareTag("Fault") || other.CompareTag("FaultAd")) && inPlay && bounces == 0)
        {
            ServeOut();
        }
    }

}
