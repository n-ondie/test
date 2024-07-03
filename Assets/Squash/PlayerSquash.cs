using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSquash : MonoBehaviour
{
    int mode;
    Vector3 initialPos;
    bool aiming;
    bool serving = true;

    float speed;
    float normalSpeed = 9.5f;
    float fastSpeed = 12f;
    float aimSpeed = 12f;
    float yCentre = 2f + 50; // 1.8f
    float xLimPlayer = 7;
    float zLimPlayer = 9;
    float xLimTarget = 7.4f;
    float yLimTargetUpper = 9.5f + 50;
    float yLimTargetLower = 0.9f + 50;

    public Transform ball;
    public Transform aimTarget;
    public Transform pointTarget;
    Vector3 aimTargetInitPos;
    Vector3 aimTargetNewPos;

    AudioSource movementAudio;
    public AudioClip shotAudio;

    LogicSquash scoreManager;
    PauseMenuSquash pauseMenu;
    Animator animator;
    ShotManagerSquash shotManager;
    ShotSquash currentShot;

    bool superSpeed;
    float superSpeedDuration = 15;
    float superSpeedTime = 0;
    public GameObject superSpeedBox;
    public Image superSpeedBar;
    public TMP_Text superSpeedTimeText;

    // Start is called before the first frame update
    void Start()
    {
        mode = LevelSelectionSquash.gameMode;
        initialPos = transform.position;
        aimTargetInitPos = aimTarget.position;
        aimTargetNewPos = aimTarget.position;

        movementAudio = GetComponent<AudioSource>();
        movementAudio.volume = 0.3f;
        animator = GetComponent<Animator>();
        scoreManager = GameObject.Find("Logic Squash").GetComponent<LogicSquash>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenuSquash>();

        shotManager = GetComponent<ShotManagerSquash>();
        currentShot = null;
        speed = normalSpeed;

        ball.transform.position = transform.position + new Vector3(0.2f, 1.78f, 0.5f); // Vector3(0.5f, 0, 0.5f)
        ball.GetComponent<SquashBall>().StopBall();
        ball.GetComponent<TrailRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false; // disable the collider when serving
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // SuperSpeed countdown
        if (superSpeed && !scoreManager.gameOver)
        {
            if (superSpeedTime > 0)
            {
                superSpeedTime -= Time.deltaTime;
                superSpeedTimeText.text = Mathf.CeilToInt(superSpeedTime).ToString();
                superSpeedBar.fillAmount = Mathf.Min(superSpeedDuration, superSpeedTime) / superSpeedDuration;
            }
            else
            {
                superSpeed = false;
                superSpeedBox.SetActive(false);
                superSpeedTime = 0;
                speed = normalSpeed;
            }
        }

        // full body or head movements: can both move and aim (smile to aim)
        if (mode == 0 || mode == 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !pauseMenu.gamePaused)
                aiming = true;
            else if (Input.GetKeyUp(KeyCode.Space))
                aiming = false;

        }
        // eyegaze: can only control movement
        //else if (mode == 2)
        //{
        //    aiming = false;
        //}

        // before serving, disable gravity and place ball in front of server
        if (serving)
        {
            if (ball.transform.position.y <= yCentre)
            {
                ball.GetComponent<Rigidbody>().useGravity = false;
                ball.transform.position = transform.position + new Vector3(0.2f, 1.78f, 0.5f); // Vector3(0.5f, 0, 0.5f)
                ball.GetComponent<SquashBall>().StopBall();
                //ballTossed = false;
            }
            else
                ball.GetComponent<Rigidbody>().useGravity = true;
        }

        // select a medium shot
        if (Input.GetKeyDown(KeyCode.B) && (ball.GetComponent<SquashBall>().inPlay || serving) && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.medium;
        }

        // select a hard shot
        if (Input.GetKeyDown(KeyCode.V) && (ball.GetComponent<SquashBall>().inPlay || serving) && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.hard;
        }

        // select a soft shot
        if (Input.GetKeyDown(KeyCode.C) && (ball.GetComponent<SquashBall>().inPlay || serving) && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.soft;
        }

        // if an shot selection key is released when serving, hit the serve
        if (Input.GetKeyUp(KeyCode.B) || Input.GetKeyUp(KeyCode.V) || Input.GetKeyUp(KeyCode.C) && !pauseMenu.gamePaused)
        {
            aiming = false;
            if (serving)
            {
                serving = false;
                ball.transform.position = transform.position + new Vector3(0.2f, 1.78f, 0.5f); // Vector3(0.5f, 0, 0.5f)
                ball.GetComponent<Rigidbody>().useGravity = true;
                ball.GetComponent<TrailRenderer>().enabled = true;
                Vector3 dir = aimTarget.position - transform.position;
                ball.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
                
                if (currentShot == shotManager.medium)
                {
                    AudioSource.PlayClipAtPoint(shotAudio, transform.position, 0.8f);
                }
                else if (currentShot == shotManager.soft)
                {
                    AudioSource.PlayClipAtPoint(shotAudio, transform.position, 0.5f);
                }
                else if (currentShot == shotManager.hard)
                {
                    AudioSource.PlayClipAtPoint(shotAudio, transform.position, 1);
                }
                animator.Play("forehand-character");

                scoreManager.UpdatePointEnderText("");
                ball.GetComponent<SquashBall>().hitter = "player";
                ball.GetComponent<SquashBall>().inPlay = true;
                ball.GetComponent<SquashBall>().justServed = true;
                ball.GetComponent<SquashBall>().bounces = 0;
                ball.GetComponent<SquashBall>().successfulShot = false;
                ball.GetComponent<SquashBall>().pointsEarned = false;
                aimTarget.position = aimTargetInitPos;
                currentShot = shotManager.medium; // default shot during rallies
            }
        }

        // moving the aim target when one of the above keys is pressed
        if (aiming && !scoreManager.gameOver)
        {
            // restrict position of aim target within end wall
            if ((aimTarget.transform.position.x < -xLimTarget && h < 0) || (aimTarget.transform.position.x > xLimTarget && h > 0))
                h = 0;
            if ((aimTarget.transform.position.y <= yLimTargetLower && v < 0) || (aimTarget.transform.position.y > yLimTargetUpper && v > 0))
                v = 0;

            aimTarget.Translate(new Vector3(h, 0, v) * aimSpeed * Time.deltaTime);
        }
        else if (mode == 2 && !scoreManager.gameOver)
        {
            AutoAim();
        }

        // player movement, controlled by arrow keys or WASD
        if ((h != 0 || v != 0) && !aiming && !scoreManager.gameOver)
        {
            // restrict player movement within court area
            if ((transform.position.x < -xLimPlayer && h < 0) || (transform.position.x > xLimPlayer && h > 0))
                h = 0;
            if (serving || (transform.position.z < -zLimPlayer && v < 0) || (transform.position.z > zLimPlayer && v > 0))
                v = 0;

            transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

            if ((h != 0 || v != 0) && !pauseMenu.gamePaused)
            {
                if (!movementAudio.isPlaying)
                    movementAudio.Play();
                animator.Play("walking");
            }
        }

        if (h == 0 && v == 0 && movementAudio.isPlaying)
            movementAudio.Stop();
    }
    public void EnableCollider() // called once the serve lands in
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void AutoAim()
    {
        float yMiddle = 55f;
        float yLow = 52.5f;
        float yHigh = 56.5f;
        float zFront = 6;
        float zMiddle = 0;
        float zBack = -6;

        if (ball.GetComponent<SquashBall>().inPlay || serving)
        {
            aimTargetNewPos.x = (transform.position.x + pointTarget.position.x) / 2;

            if (pointTarget.position.z > zMiddle && !serving) // move target down
            {
                aimTargetNewPos.y = yMiddle - (pointTarget.position.z / zFront) * (yMiddle - yLow);
            }
            if (pointTarget.position.z < zMiddle) // move target up
            {
                aimTargetNewPos.y = yMiddle + (pointTarget.position.z / zBack) * (yHigh - yMiddle);
            }
        }

        aimTarget.transform.position = Vector3.MoveTowards(aimTarget.transform.position, aimTargetNewPos, aimSpeed * Time.deltaTime);
    }

    // hit the ball if it is within range of the player
    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        // only hit the ball if it's in play
        if (other.CompareTag("Ball") && ball.GetComponent<SquashBall>().inPlay && scoreManager.gameTimeRemaining > 0) // && currentShot != null
        {
            Vector3 dir = aimTarget.position - transform.position; // vector from player to aimTarget
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);

            if (currentShot == shotManager.medium)
            {
                AudioSource.PlayClipAtPoint(shotAudio, transform.position, 0.8f);
            }
            else if (currentShot == shotManager.soft)
            {
                AudioSource.PlayClipAtPoint(shotAudio, transform.position, 0.5f);
            }
            else if (currentShot == shotManager.hard)
            {
                AudioSource.PlayClipAtPoint(shotAudio, transform.position, 1);
            }

            // if ball is on the player's right, use the forehand animation; else use the backhand animation
            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("forehand-character");
            }
            else
            {
                animator.Play("backhand-character");
            }

            GetComponent<BoxCollider>().enabled = false; // disable collider, enable if shot hits end wall successfully
            currentShot = shotManager.medium; // back to default shot
            ball.GetComponent<SquashBall>().hitter = "player";
            ball.GetComponent<SquashBall>().bounces = 0;
            ball.GetComponent<SquashBall>().successfulShot = false;
            ball.GetComponent<SquashBall>().pointsEarned = false;

            if (mode != 2)
                aimTarget.position = aimTargetInitPos;  // reset position of aim target after hitting the ball
        }
    }

    public void ResetPlayer()
    {
        //if (ball.GetComponent<Ball>().deuceCurrent)
        //{
        //    transform.position = serveDeuce.position;
        //    aimTarget.position = aimTargetServeDeuce;
        //}
        //else
        //{
        //    transform.position = serveAd.position;
        //    aimTarget.position = aimTargetServeAd;
        //}
        transform.position = initialPos;
        aimTarget.position = aimTargetInitPos;
        aiming = false; // in case an aiming key is being held down when Reset is called
        serving = true;
        GetComponent<BoxCollider>().enabled = false;
    }

    public void EnableSuperSpeed()
    {
        superSpeedTime += superSpeedDuration;
        superSpeed = true;
        superSpeedBox.SetActive(true);
        speed = fastSpeed;
    }
}
