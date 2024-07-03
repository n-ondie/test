using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class OpponentVolleyball : MonoBehaviour
{
    float speed = 4.5f;
    float digForce = 12;
    float xLim = 7.5f;
    float yTop = 7.2f;

    public Transform player;
    public Transform ball;
    public Transform aimTarget;
    Vector3 aimTargetInitPos;
    Vector3 targetPos;
    [SerializeField] Transform opponentRecPos;
    public Transform[] targets;

    public AudioClip digAudio;
    public AudioClip flatAudio;

    LogicVolleyball scoreManager;
    PauseMenuVolleyball pauseMenu;
    Animator animator;
    ShotManagerVolleyball shotManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        scoreManager = GameObject.Find("Logic Volleyball").GetComponent<LogicVolleyball>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenuVolleyball>();
        shotManager = GetComponent<ShotManagerVolleyball>();

        targetPos = transform.position;
        aimTargetInitPos = aimTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        // follow the ball sideways within the court bounds
        if (ball.GetComponent<Volleyball>().hitter == "player" && ball.position.x > xLim)
        {
            targetPos.x = xLim;
        }
        else if (ball.GetComponent<Volleyball>().hitter == "player" && ball.position.x < -xLim)
        { 
            targetPos.x = -xLim;
        }
        else
        {
            targetPos.x = ball.position.x;
        }

        // if player has just aimed a shot over the net and the ball hasn't gone past the opponent...
        if (ball.GetComponent<Volleyball>().aimedPos != new Vector3(0, 0, 0) && ball.position.z > 0 &&
            ball.position.z < transform.position.z && ball.GetComponent<Volleyball>().inPlay)
        {
            // move between the ball and aimed position for a high shot behind the aimed position otherwise (flat shot and serve)
            if (ball.GetComponent<Volleyball>().playerShot == "high")
                targetPos.z = Mathf.Min(14, ball.position.z);
                //targetPos.z = Mathf.Min(14, (2 * ball.position.z + ball.GetComponent<Volleyball>().aimedPos.z) / 3);
            else if (ball.GetComponent<Volleyball>().playerShot == "flat")
                targetPos.z = Mathf.Min(14, ball.GetComponent<Volleyball>().aimedPos.z + 0.5f);
            else
                targetPos.z = Mathf.Min(14, ball.GetComponent<Volleyball>().aimedPos.z + 1.5f);
        }
        // otherwise follow the ball if it's on the opponent's side
        else if (ball.position.z > 0)
        {
            targetPos.z = ball.position.z;
        }
        // and stay in the middle if it's on the player's side
        else
        {
            targetPos.z = opponentRecPos.position.z;
        }

        if (ball.GetComponent<Volleyball>().inPlay)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (transform.position != targetPos && !pauseMenu.gamePaused)
                animator.Play("walking2-vb");
        }
    }

    // choose a target at random for the opponent to hit to
    Vector3 PickTarget()
    {
        int randVal = Random.Range(0, targets.Length);
        return targets[randVal].position;
    }

    // choose a shot type at random
    ShotVolleyball PickShot()
    {
        int randVal = Random.Range(0, 2);
        if (randVal == 0)
            return shotManager.high;
        else //if (randVal == 1)
            return shotManager.flat;
    }

    // hit the ball if it is within range of the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && ball.GetComponent<Volleyball>().inPlay)
        {
            // increase player's score when opponent hits the ball after a successful shot over the net
            if (ball.GetComponent<Volleyball>().hitter == "player")
            {
                scoreManager.AddScore(1, false);
                player.GetComponent<PlayerVolleyball>().EnableCollider();
                if (player.GetComponent<PlayerVolleyball>().justServed)
                {
                    player.GetComponent<PlayerVolleyball>().justServed = false;
                }
            }

            float hDist = ball.position.x - transform.position.x;
            float vDist = ball.position.z - transform.position.z;
            int randVal = Random.Range(0, 2);

            if (ball.GetComponent<Volleyball>().hits < 1 && randVal == 0) // just dig on own side, don't hit over the net
            {
                other.GetComponent<Rigidbody>().velocity = new Vector3(hDist, digForce, 0);
                AudioSource.PlayClipAtPoint(digAudio, transform.position, 1);
                ball.GetComponent<Volleyball>().hits++;

                if (ball.position.y > yTop)
                    animator.Play("pushup2");
                else
                    animator.Play("dig2");
            }
            else
            {
                ShotVolleyball currentShot = PickShot();
                ball.GetComponent<Volleyball>().hits = 0;

                float xNoise = Random.Range(-1f, 1f);
                float zNoise;
                if (currentShot == shotManager.flat)
                    zNoise = Random.Range(0f, 0.5f); // don't aim too long for a flat shot
                else
                    zNoise = Random.Range(-0.5f, 0.5f);

                // https://discussions.unity.com/t/how-to-make-enemy-cannonball-fall-on-moving-target-position/25258/2
                var dir = (PickTarget() + new Vector3(xNoise, 0f, zNoise)) - transform.position; // get target direction
                var h = dir.y;  // get height difference
                dir.y = 0;  // retain only the horizontal direction
                var dist = dir.magnitude;  // get horizontal distance
                dir.y = dist + currentShot.upForce;  // set elevation to 45 degrees and adjust by upForce
                dist += h;  // correct for different heights
                var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude) + currentShot.hitForce / 10;
                other.GetComponent<Rigidbody>().velocity = vel * dir.normalized;

                if (currentShot == shotManager.high)
                {
                    AudioSource.PlayClipAtPoint(digAudio, transform.position, 1);
                    animator.Play("dig2");
                }
                else if (currentShot == shotManager.flat)
                {
                    AudioSource.PlayClipAtPoint(flatAudio, transform.position, 1);
                    animator.Play("flat2-vb");
                }
            }

            ball.GetComponent<Volleyball>().hitter = "opponent";
            ball.GetComponent<Volleyball>().aimedPos = new Vector3(0, 0, 0);
            ball.GetComponent<Volleyball>().playerShot = null;
        }
    }

    public void ResetOpponent()
    {
        transform.position = opponentRecPos.position;
    }
}
