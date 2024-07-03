using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SquashBall : MonoBehaviour
{
    Vector3 initialPos;
    public string hitter;
    public bool inPlay = false;
    public int bounces;
    //public bool deuceCurrent = true; // first point is on deuce side
    public bool justServed;
    public bool successfulShot;
    bool pointEnded;

    public Transform player;
    public Transform cam;

    AudioSource bounceAudio;
    public AudioClip hitFrontWall;
    public AudioClip hitGlass;
    public AudioClip outCall;

    SpecialEffectsSquash effectManager;
    LogicSquash scoreManager;
    public bool pointsEarned;

    private float timer = 0;
    private int resetTime = 2;

    // Start is called before the first frame update
    void Start()
    {
        bounceAudio = GetComponent<AudioSource>();
        effectManager = GameObject.FindGameObjectWithTag("Special Effects").GetComponent<SpecialEffectsSquash>();
        scoreManager = GameObject.Find("Logic Squash").GetComponent<LogicSquash>();
        initialPos = transform.position;
        bounces = 0;
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
            GetComponent<TrailRenderer>().enabled = false;

            if (scoreManager.gameTimeRemaining > 0)
                player.GetComponent<PlayerSquash>().ResetPlayer();

            pointEnded = false;
            timer = 0;
        }

        // in case terrain collider didn't work
        if (transform.position.y < 45 && inPlay)
        {
            StopBall();
            scoreManager.UpdatePointEnderText("Out");
            scoreManager.AddScore(-2, false);
            EndPoint();
        }
    }

    public void EndPoint()
    {
        inPlay = false;
        pointEnded = true;
        //scoreManager.UpdatePointEnderText();
    }

    public void StopBall()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // what to do when the ball hits something
    private void OnCollisionEnter(Collision collision) // 'collision' is the object the ball has collided into
    {
        if (collision.transform.CompareTag("Ground") && bounces < 2 && inPlay)
        {
            bounceAudio.PlayOneShot(bounceAudio.clip, 0.4f);
            bounces++;

            if (bounces == 1 && !successfulShot)
            {
                AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
                scoreManager.UpdatePointEnderText("Out");
                scoreManager.AddScore(-2, false);
                EndPoint();
            }
            else if (bounces == 2)
            {
                // last shot after time runs out doesn't count as end of point
                if (scoreManager.gameTimeRemaining > 0)
                {
                    scoreManager.UpdatePointEnderText("2 bounces");
                    scoreManager.AddScore(-2, false);
                }
                EndPoint();
            }
        }

        else if (collision.transform.CompareTag("Terrain"))  // ball goes out of the court area completely
        {
            StopBall();

            if (inPlay)
            {
                AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
                scoreManager.UpdatePointEnderText("Out");
                scoreManager.AddScore(-2, false);
                EndPoint();
            }
        }

        else if (collision.transform.CompareTag("SquashIn") && inPlay)
        {
            AudioSource.PlayClipAtPoint(hitFrontWall, transform.position, 1);
            if (justServed)
            {
                justServed = false;
            }
            player.GetComponent<PlayerSquash>().EnableCollider();
            successfulShot = true;

            // animal appears and runs away
            effectManager.SpawnAnimal(new Vector3(transform.position.x, transform.position.y, 12f));
        }

        else if (collision.transform.CompareTag("Wall") && inPlay)
        {
            AudioSource.PlayClipAtPoint(hitGlass, transform.position, 1);
        }
    }

    // end the point if the ball hits an 'out' area
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Out") && inPlay)
        {
            AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
            scoreManager.UpdatePointEnderText("Out");
            scoreManager.AddScore(-2, false);
            EndPoint();
        }
        else if (justServed && other.CompareTag("Fault") && inPlay)
        {
            AudioSource.PlayClipAtPoint(outCall, cam.position, 0.7f);
            scoreManager.UpdatePointEnderText("Serve Out");
            scoreManager.AddScore(-1, false);
            EndPoint();
        }

        if (other.CompareTag("PointTargetOuter") && inPlay && bounces == 0 && successfulShot && !pointsEarned)
        {
            scoreManager.AddScore(5, false);
            pointsEarned = true;
        }
        else if (other.CompareTag("PointTargetMiddle") && inPlay && bounces == 0 && successfulShot && !pointsEarned)
        {
            scoreManager.AddScore(10, false);
            pointsEarned = true;
        }
        else if (other.CompareTag("PointTargetInner") && inPlay && bounces == 0 && successfulShot && !pointsEarned)
        {
            scoreManager.AddScore(20, false);
            pointsEarned = true;
        }
    }
}
