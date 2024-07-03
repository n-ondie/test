using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour   
{
    int mode;
    Vector3 initPos;
    Vector3 targetPos;
    float zMoveReceive = -4.8f;
    float xServeBoundary = 3.6f;

    //float force = 13;
    bool aiming; // default false
    public bool serving = true;
    bool ballTossed;

    float serveTime;
    float serveTimeLimit = 15;
    public GameObject serveTimerBox;
    public TMP_Text serveTimerText;

    float speed;
    float autoSpeed = 7.5f;
    float normalSpeed = 9.5f;
    float fastSpeed = 12f;
    float aimSpeed = 19f;
    float ballTossForce = 14;
    float yCentre = 5.85f; // 4.68f
    float xLimServe = 7.2f;
    float xLimPlayer = 12.5f;
    float zLimPlayerBack = -28.8f;

    public Transform ball;
    public Transform opponent;
    public Transform aimTarget;
    Vector3 aimTargetInitPos;
    Vector3 aimTargetServeDeuce;
    Vector3 aimTargetServeAd;
    float targetServeOffsetX = 3.84f;
    float targetServeOffsetZ = -6f;
    float xLimTarget = 12.5f;
    float zLimTargetEnd = 30f;

    AudioSource movementAudio;
    public AudioClip topspinAudio;
    public AudioClip flatAudio;
    public AudioClip serveAudio;

    LogicTennis scoreManager;
    PauseMenuTennis pauseMenu;
    Animator animator;
    ShotManager shotManager;
    Shot currentShot;

    [SerializeField] Transform serveDeuce;
    [SerializeField] Transform serveAd;
    [SerializeField] Transform receiveDeuce;
    [SerializeField] Transform receiveAd;

    bool superSpeed;
    float superSpeedDuration = 15;
    float superSpeedTime = 0;
    public GameObject superSpeedBox;
    public Image superSpeedBar;
    public TMP_Text superSpeedTimeText;

    public PowerBarTennis powerBar;

    // Start is called before the first frame update
    void Start()
    {
        mode = LevelSelectionTennis.gameMode;
        initPos = transform.position; // (0, 3.65, 23.5)
        targetPos = transform.position;

        movementAudio = GetComponent<AudioSource>();
        movementAudio.volume = 0.2f;
        animator = GetComponent<Animator>(); // reference to current animator
        scoreManager = GameObject.Find("Logic Tennis").GetComponent<LogicTennis>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenuTennis>();

        shotManager = GetComponent<ShotManager>();
        currentShot = null;
        speed = normalSpeed;

        aimTargetInitPos = aimTarget.position;
        aimTargetServeDeuce = aimTarget.position + new Vector3(-targetServeOffsetX, 0, targetServeOffsetZ);
        aimTargetServeAd = aimTarget.position + new Vector3(targetServeOffsetX, 0, targetServeOffsetZ);

        // start the game serving on the deuce side
        transform.position = serveDeuce.position;
        aimTarget.position = aimTargetServeDeuce;
        ball.transform.position = transform.position + new Vector3(0.5f, 2.2f, 0.5f); // Vector3(0.5f, 0, 0.5f)
        ball.GetComponent<Ball>().StopBall();
        ball.GetComponent<TrailRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false; // disable the collider when serving

        serveTimerBox.SetActive(true);
        serveTime = serveTimeLimit;
    }

    // Update is called once per frame
    void Update()
    {
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
                if (mode == 2)
                    speed = autoSpeed;
                else
                    speed = normalSpeed;
                //scoreManager.UpdateEffectText("Super Speed", false);
            }
        }

        // Serve countdown: fault if no serve when timer runs out
        if (serving)
        {
            if (serveTime > 0)
            {
                serveTime -= Time.deltaTime;
                if (serveTime > 5)
                    serveTimerText.text = "";
                else
                    serveTimerText.text = Mathf.CeilToInt(serveTime).ToString();
            }
            else
            {
                ServeTimeout();
            }
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // full body or head movements: can both move and aim (smile to aim)
        if (mode == 0 || mode == 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !pauseMenu.gamePaused)
                aiming = true;
            else if (Input.GetKeyUp(KeyCode.Space))
                aiming = false;

        }
        // eyegaze: can only aim, movement is automatic
        else if (mode == 2)
        {
            aiming = true;
        }

        // select a topspin shot
        if (Input.GetKeyDown(KeyCode.B) && !serving && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.topspin;
            //currentShot = shotManager.powerShot; // testing
        }

        // select a flat shot
        if (Input.GetKeyDown(KeyCode.V) && !serving && !pauseMenu.gamePaused) 
        {
            currentShot = shotManager.flat;
        }

        // if power bar is full, select the power shot
        if (Input.GetKeyDown(KeyCode.C) && powerBar.slider.value == powerBar.maxPower && !serving && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.powerShot;
        }

        // when serving, disable gravity and place ball in front of server if ball has not been tossed or was not hit after a toss
        if (serving)
        {
            if (ball.transform.position.y <= yCentre)
            {
                ball.GetComponent<Rigidbody>().useGravity = false;
                ball.transform.position = transform.position + new Vector3(0.2f, 2.2f, 0.5f); // Vector3(0.5f, 0, 0.5f)
                ball.GetComponent<Ball>().StopBall();
                ballTossed = false;
            }
            else
                ball.GetComponent<Rigidbody>().useGravity = true;
        }
        // select a flat serve or a kick serve and toss the ball up (use same keys as flat and topspin groundstrokes)
        if (Input.GetKeyDown(KeyCode.V) && serving && !ballTossed && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.flatServe;
            ballTossed = true;
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0, ballTossForce, 0);
        }
        if (Input.GetKeyDown(KeyCode.B) && serving && !ballTossed && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.kickServe;
            ballTossed = true;
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0, ballTossForce, 0);
        }
        // press the same key when the ball is in the air to hit the serve
        if ((currentShot == shotManager.flatServe && Input.GetKeyDown(KeyCode.V) || currentShot == shotManager.kickServe && Input.GetKeyDown(KeyCode.B)) 
            && serving && ball.transform.position.y >= yCentre + 1 && !pauseMenu.gamePaused)
        {
            ballTossed = false;
            serving = false;
            animator.Play("serve-prepare-character");
            ball.transform.position = transform.position + new Vector3(0.2f, 4.75f, 0.5f); // Vector3(0.5f, 3.6f, 0.5f)
            ball.GetComponent<TrailRenderer>().enabled = true;
            Vector3 dir = aimTarget.position - transform.position;
            ball.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
            AudioSource.PlayClipAtPoint(serveAudio, transform.position, 1);
            powerBar.ChargePower(1);
            //animator.Play("serve-character");

            ResetServeTimer();
            scoreManager.UpdatePointEnderText("");
            ball.GetComponent<Ball>().hitter = "player";
            ball.GetComponent<Ball>().inPlay = true;
            ball.GetComponent<Ball>().justServed = true;
            ball.GetComponent<Ball>().bounces = 0;
            aimTarget.position = aimTargetInitPos;
            currentShot = shotManager.topspin; // default shot during rallies
        }

        // moving the aim target when aiming
        if (aiming && !scoreManager.gameOver)
        {
            // restrict position of aim target within court area
            if ((aimTarget.transform.position.x < -xLimTarget && h < 0) || (aimTarget.transform.position.x > xLimTarget && h > 0))
                h = 0;
            if ((aimTarget.transform.position.z <= 0 && v < 0) || (aimTarget.transform.position.z > zLimTargetEnd && v > 0))
                v = 0;

            aimTarget.Translate(new Vector3(h, 0, v) * aimSpeed * Time.deltaTime);
        }

        // player movement, controlled by arrow keys or WASD
        if (mode == 0 || mode == 1)
        {
            if ((h != 0 || v != 0) && !aiming && !scoreManager.gameOver)
            {
                // restrict player movement within court area
                if ((transform.position.x < -xLimPlayer && h < 0) || (transform.position.x > xLimPlayer && h > 0) ||
                    (serving && ball.GetComponent<Ball>().rotation == 0 && (transform.position.x < 0.2 && h < 0 || transform.position.x > xLimServe && h > 0)) ||
                    (serving && ball.GetComponent<Ball>().rotation == 3 && (transform.position.x > -0.2 && h > 0 || transform.position.x < -xLimServe && h < 0)))
                    h = 0;
                if (serving || (transform.position.z < zLimPlayerBack && v < 0) || (transform.position.z > -2 && v > 0))
                    v = 0;

                transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

                if ((h != 0 || v != 0) && !pauseMenu.gamePaused)
                {
                    if (!movementAudio.isPlaying)
                        movementAudio.Play();
                    animator.Play("walking");
                }
            }

            //if ((h != 0 || v != 0) && !aiming && !scoreManager.gameOver && !movementAudio.isPlaying)
            //    movementAudio.Play();
            if (h == 0 && v == 0 && movementAudio.isPlaying)
                movementAudio.Stop();
        }
        
        if (mode == 2)
        {
            AutoMove();
        }
    }

    public void EnableCollider() // called when oppponent makes first contact after the serve
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void AutoMove()
    {
        if (ball.GetComponent<Ball>().inPlay)
        {
            // movement just after opponent serves from deuce side
            if (ball.GetComponent<Ball>().justServed && ball.GetComponent<Ball>().rotation == 2 &&
            ball.position.z > zMoveReceive && ball.position.x < transform.position.x)
                targetPos.x = transform.position.x;
            // movement just after opponent serves from ad side
            else if (ball.GetComponent<Ball>().justServed && ball.GetComponent<Ball>().rotation == 1 &&
            ball.position.z > zMoveReceive && ball.position.x > transform.position.x)
                targetPos.x = transform.position.x;
            // movement just after hitting the ball: return to the centre
            else if (ball.GetComponent<Ball>().hitter == "player")
            {
                targetPos.x = 0;
                targetPos.z = initPos.z;
            }
            // follow the ball horizontally if hitter is opponent
            else
            {
                if (ball.position.x > xLimPlayer)
                    targetPos.x = xLimPlayer;
                else if (ball.position.x < -xLimPlayer)
                    targetPos.x = -xLimPlayer;
                else if (Mathf.Abs(ball.position.x - transform.position.x) > 1)
                    targetPos.x = ball.position.x;

                // move backwards if ball is too far away horizontally
                Vector3 ballDir = ball.position - transform.position;
                float xDist = Mathf.Abs(ballDir.x);
                float zDist = Mathf.Abs(ballDir.z);
                if (xDist > 0.5 * zDist)
                {
                    float retreatDist = xDist - 0.5f * zDist;
                    targetPos.z = Mathf.Max(zLimPlayerBack, initPos.z - retreatDist);
                }
            }

            //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

        else if (opponent.GetComponent<Opponent>().serving && ball.GetComponent<Ball>().rotation == 2)
        {
            if (opponent.transform.position.x < -xServeBoundary)
                targetPos.x = receiveAd.position.x + (-xServeBoundary - opponent.transform.position.x);
            else
                targetPos.x = receiveDeuce.position.x;

            //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (opponent.GetComponent<Opponent>().serving && ball.GetComponent<Ball>().rotation == 1)
        {
            if (opponent.transform.position.x > xServeBoundary)
                targetPos.x = receiveDeuce.position.x - (opponent.transform.position.x - xServeBoundary);
            else
                targetPos.x = receiveAd.position.x;

            //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

        if (transform.position != targetPos && !serving && !pauseMenu.gamePaused)
        {
            //Debug.Log(AnimatorIsPlaying());
            if (!movementAudio.isPlaying)
                movementAudio.Play();
            //if (!AnimatorIsPlaying())
            //{
            //    animator.Play("walking");
            //    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            //}
            animator.Play("walking");
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (movementAudio.isPlaying)
            movementAudio.Stop();
    }

    // hit the ball if it is within range of the player
    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        // only hit the ball if it's in play
        if (other.CompareTag("Ball") && ball.GetComponent<Ball>().inPlay && currentShot != null)
        {
            Vector3 dir = aimTarget.position - transform.position; // vector from player to aimTarget
            // normalise to remove effect of distance of player from target
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);

            if (currentShot == shotManager.topspin)
            {
                AudioSource.PlayClipAtPoint(topspinAudio, transform.position, 1);
                powerBar.ChargePower(1);
            }
            else if (currentShot == shotManager.flat)
            {
                AudioSource.PlayClipAtPoint(flatAudio, transform.position, 1);
                powerBar.ChargePower(2);
            }
            else if (currentShot == shotManager.powerShot)
            {
                AudioSource.PlayClipAtPoint(flatAudio, transform.position, 1);
                powerBar.ClearPower();
                ball.GetComponent<Ball>().powerShot = true;
                // hitting a power shot also slows down the opponent
                opponent.GetComponent<Opponent>().speed = opponent.GetComponent<Opponent>().slowSpeed;
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

            currentShot = shotManager.topspin; // back to default shot topspin
            ball.GetComponent<Ball>().hitter = "player";
            ball.GetComponent<Ball>().bounces = 0;
            if (ball.GetComponent<Ball>().justServed)
            {
                ball.GetComponent<Ball>().justServed = false;
                opponent.GetComponent<Opponent>().EnableCollider();
            }
            aimTarget.position = aimTargetInitPos;  // reset position of aim target after hitting the ball
        }
    }

    public void ResetPlayer()
    {
        aiming = false; // in case an aiming key is being held down when Reset is called
        ResetServeTimer();

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
        if (ball.GetComponent<Ball>().rotation == 0 || ball.GetComponent<Ball>().rotation == 3)
        {
            serving = true;
            serveTimerBox.SetActive(true);
            GetComponent<BoxCollider>().enabled = false;

            if (ball.GetComponent<Ball>().rotation == 0)
            {
                transform.position = serveDeuce.position;
                aimTarget.position = aimTargetServeDeuce;
            }
            else // ball.GetComponent<Ball>().rotation == 3
            {
                transform.position = serveAd.position;
                aimTarget.position = aimTargetServeAd;
            }
        }
        else
        {
            aimTarget.position = aimTargetInitPos;
            EnableCollider();
            if (ball.GetComponent<Ball>().rotation == 1)
                transform.position = receiveAd.position;
            else // ball.GetComponent<Ball>().rotation == 2
                transform.position = receiveDeuce.position;
        }
    }

    public void EnableSuperSpeed()
    {
        superSpeedTime += superSpeedDuration;
        superSpeed = true;
        superSpeedBox.SetActive(true);
        speed = fastSpeed;
        //scoreManager.UpdateEffectText("Super Speed", true);
    }

    void ServeTimeout()
    {
        ball.GetComponent<Ball>().hitter = "player";
        ball.GetComponent<Ball>().ServeOut();
        serveTimerText.text = "Fault";
        serving = false;
    }

    void ResetServeTimer()
    {
        serveTime = serveTimeLimit;
        serveTimerBox.SetActive(false);
    }
}
