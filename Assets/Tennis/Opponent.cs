using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Opponent : MonoBehaviour
{
    float scale = 2.4f;
    Vector3 initPos;
    public float speed; // footspeed
    public float normalSpeed = 8.5f;
    public float slowSpeed = 6f;
    float xLim = 13f;
    float xServeBoundary = 3.6f;
    float zMoveReceive = 4.8f;

    public bool serving;
    float serveTimer = 0;
    float serveHitTime = 2;
    float serveTargetX = 3.84f;
    float serveTargetY = 0.55f + 3f;
    float serveTargetZ = -14.4f + 6f;
    float ballTossForce = 14;

    public Transform ball;
    public Transform player;
    //public Transform aimTarget;
    public Transform[] targets;
    Vector3 targetPos;

    public AudioClip topspinAudio;
    public AudioClip flatAudio;
    public AudioClip serveAudio;

    LogicTennis scoreManager;
    PauseMenuTennis pauseMenu;
    Animator animator;
    ShotManager shotManager;

    bool stuckInTheMud;
    float stuckInTheMudDuration = 12;
    float stuckInTheMudTime = 0;
    public GameObject stuckInTheMudBox;
    public Image stuckInTheMudBar;
    public TMP_Text stuckInTheMudTimeText;

    [SerializeField] Transform serveDeuce;
    [SerializeField] Transform serveAd;
    [SerializeField] Transform receiveDeuce;
    [SerializeField] Transform receiveAd;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        scoreManager = GameObject.Find("Logic Tennis").GetComponent<LogicTennis>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenuTennis>();
        shotManager = GetComponent<ShotManager>();

        initPos = transform.position; // (0, 5.4, 24) (0, 3.65, 24)
        targetPos = transform.position;
        transform.position = receiveDeuce.position;
        speed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Stuck in the mud countdown
        if (stuckInTheMud && !scoreManager.gameOver)
        {
            if (stuckInTheMudTime > 0)
            {
                stuckInTheMudTime -= Time.deltaTime;
                stuckInTheMudTimeText.text = Mathf.CeilToInt(stuckInTheMudTime).ToString();
                stuckInTheMudBar.fillAmount = Mathf.Min(stuckInTheMudDuration, stuckInTheMudTime) / stuckInTheMudDuration;
            }
            else
            {
                stuckInTheMud = false;
                stuckInTheMudBox.SetActive(false);
                stuckInTheMudTime = 0;
                speed = normalSpeed;
                //scoreManager.UpdateEffectText("Stuck in the Mud", false);
            }
        }

        // if serving, wait 2 seconds before hitting the serve
        if (serving)
        {
            if (serveTimer < serveHitTime)
            {
                ball.GetComponent<Rigidbody>().useGravity = false;
                ball.transform.position = transform.position + new Vector3(0.5f, 2.2f, 0.5f); // Vector3(0.5f, 0, 0.5f)
                //ball.GetComponent<Ball>().StopBall();
                serveTimer += Time.deltaTime;
            }
            else if (ball.transform.position.y < transform.position.y + 5.2f) // transform.position.y + 4f
            {
                // toss the ball up
                ball.GetComponent<Rigidbody>().velocity = new Vector3(0, ballTossForce, 0);
                ball.GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                Serve();
                serveTimer = 0;
            }
        }

        Move();
    }

    // choose a target at random for the opponent to hit to
    Vector3 PickTarget()
    {
        int randVal = Random.Range(0, targets.Length);
        return targets[randVal].position;
    }

    // choose a shot type at random
    Shot PickShot()
    {
        int randVal = Random.Range(0, 2);
        if (randVal == 0)
            return shotManager.topspin;
        else //if (randVal == 1)
            return shotManager.flat;
    }

    // move towards the ball
    void Move()
    {
        if (ball.GetComponent<Ball>().inPlay)
        {
            // movement just after player serves from deuce side
            if (ball.GetComponent<Ball>().justServed && ball.GetComponent<Ball>().rotation == 0 &&
            ball.position.z < zMoveReceive && ball.position.x > transform.position.x)
                targetPos.x = transform.position.x;
            // movement just after player serves from ad side
            else if (ball.GetComponent<Ball>().justServed && ball.GetComponent<Ball>().rotation == 3 &&
            ball.position.z < zMoveReceive && ball.position.x < transform.position.x)
                targetPos.x = transform.position.x;
            // movement just after opponent serves
            else if (ball.GetComponent<Ball>().justServed && (ball.GetComponent<Ball>().rotation == 1 || ball.GetComponent<Ball>().rotation == 2))
                targetPos.x = 0;
            // normal movement during rallies, staying within the court area
            else
            {
                if (ball.position.x > xLim)
                    targetPos.x = xLim;
                else if (ball.position.x < -xLim)
                    targetPos.x = -xLim;
                else
                    targetPos.x = ball.position.x;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        // move sideways when waiting to receive
        else if (player.GetComponent<Player>().serving && ball.GetComponent<Ball>().rotation == 0)
        {
            if (player.transform.position.x > xServeBoundary)
                targetPos.x = receiveDeuce.position.x - (player.transform.position.x - xServeBoundary);
            else
                targetPos.x = receiveDeuce.position.x;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (player.GetComponent<Player>().serving && ball.GetComponent<Ball>().rotation == 3)
        {
            if (player.transform.position.x < -xServeBoundary)
                targetPos.x = receiveAd.position.x + (-xServeBoundary - player.transform.position.x);
            else
                targetPos.x = receiveAd.position.x;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

        if (transform.position != targetPos && !serving && !pauseMenu.gamePaused)
            animator.Play("walking2");
    }

    void Serve()
    {
        // randomly choose the serve target
        Vector3 serveTarget;
        float xNoise = Random.Range(-4f, 4f);
        float zNoise = Random.Range(-1f, 0.5f);
        if (ball.GetComponent<Ball>().rotation == 2) // serving from deuce side
            serveTarget = new Vector3(serveTargetX + xNoise, serveTargetY, serveTargetZ + zNoise);
        else
            serveTarget = new Vector3(-serveTargetX + xNoise, serveTargetY, serveTargetZ + zNoise);

        // first serve is flat serve, second serve is kick serve
        Shot currentShot;
        if (ball.GetComponent<Ball>().fault)
            currentShot = shotManager.kickServe;
        else
            currentShot = shotManager.flatServe;

        // hit the serve
        serving = false;
        animator.Play("serve-prepare2");
        ball.transform.position = transform.position + new Vector3(0.2f, 4.75f, 0.5f); // Vector3(0.5f, 3.6f, 0.5f)
        ball.GetComponent<TrailRenderer>().enabled = true;
        Vector3 dir = serveTarget - transform.position;
        ball.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
        AudioSource.PlayClipAtPoint(serveAudio, transform.position, 1);
        //animator.Play("serve2");

        scoreManager.UpdatePointEnderText("");
        ball.GetComponent<Ball>().hitter = "opponent";
        ball.GetComponent<Ball>().inPlay = true;
        ball.GetComponent<Ball>().justServed = true;
        ball.GetComponent<Ball>().bounces = 0;
    }
    public void EnableCollider() // called when player makes first contact after the serve
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    // hit the ball if it is within range of the player
    private void OnTriggerEnter(Collider other)
    {
        // only hit the ball if it's in play and don't volley from behind the baseline
        if (other.CompareTag("Ball") && ball.GetComponent<Ball>().inPlay && ball.GetComponent<Ball>().bounces == 1)
        {
            Shot currentShot;
            if (ball.GetComponent<Ball>().powerShot) // must hit a loopy shot in response to a power shot
            {
                currentShot = shotManager.topspin;
                if (!stuckInTheMud)
                    speed = normalSpeed;
            }
            else
                currentShot = PickShot();
            //Shot currentShot = PickShot();

            float xNoise = Random.Range(-0.5f, 0.5f) * scale;
            float zNoise = Random.Range(-1f, 0.5f) * scale;
            Vector3 dir = (PickTarget() + new Vector3(xNoise, 0f, zNoise)) - transform.position; // add noise to target
            // normalise to remove effect of distance of player from target
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);

            if (currentShot == shotManager.topspin)
                AudioSource.PlayClipAtPoint(topspinAudio, transform.position, 1);
            else if (currentShot == shotManager.flat)
                AudioSource.PlayClipAtPoint(flatAudio, transform.position, 1);

            // if ball is on the player's right, use the forehand animation; else use the backhand animation
            Vector3 ballDir = ball.position - transform.position;
            if (ballDir.x <= 0)
            {
                animator.Play("forehand2");
            }
            else
            {
                animator.Play("backhand2");
            }

            ball.GetComponent<Ball>().hitter = "opponent";
            ball.GetComponent<Ball>().bounces = 0;
            if (ball.GetComponent<Ball>().powerShot)
            {
                ball.GetComponent<Ball>().powerShot = false;
            }
            if (ball.GetComponent<Ball>().justServed)
            {
                ball.GetComponent<Ball>().justServed = false;
                player.GetComponent<Player>().EnableCollider();
            }
        }
    }

    public void ResetOpponent()
    {
        //if (ball.GetComponent<Ball>().deuceCurrent)
        //    transform.position = receiveDeuce.position;
        //else
        //    transform.position = receiveAd.position;

        // in case point ended due to power shot
        if (!stuckInTheMud)
            speed = normalSpeed;

        if (ball.GetComponent<Ball>().rotation == 1 || ball.GetComponent<Ball>().rotation == 2)
        {
            serving = true;
            GetComponent<BoxCollider>().enabled = false;

            if (ball.GetComponent<Ball>().rotation == 1)
                transform.position = serveAd.position;
            else // ball.GetComponent<Ball>().rotation == 2
                transform.position = serveDeuce.position;
        }
        else
        {
            EnableCollider();
            if (ball.GetComponent<Ball>().rotation == 0)
                transform.position = receiveDeuce.position;
            else // ball.GetComponent<Ball>().rotation == 3
                transform.position = receiveAd.position;
        }
    }

    public void EnableStuckInTheMud()
    {
        stuckInTheMudTime += stuckInTheMudDuration;
        stuckInTheMud = true;
        stuckInTheMudBox.SetActive(true);
        speed = slowSpeed;
        //scoreManager.UpdateEffectText("Stuck in the Mud", true);
    }
}
