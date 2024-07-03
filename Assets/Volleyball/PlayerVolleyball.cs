using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVolleyball : MonoBehaviour
{
    int mode;
    bool aiming;
    bool serving = true;
    bool ballTossed;
    public bool justServed;

    float speed;
    float normalSpeed = 5f;
    float fastSpeed = 7f;
    float aimSpeed = 14f;
    float digForce = 12;
    float ballTossForce = 7;
    float yCentre = 1.9f + 3.3f;
    float yTop = 7.2f;
    float xLimPlayer = 11;
    float xLimTarget = 12.5f;
    float zLimPlayerBack = -26;
    float zLimTargetEnd = 28;

    float jumpVelocity;
    float jumpForce = 6;
    float gravity = -Physics.gravity.magnitude; //-9.81f
    bool isGrounded = true;

    public Transform ball;
    public Transform aimTarget;
    Vector3 aimTargetInitPos;
    float initialAngle;
    [SerializeField] Transform playerServePos;

    AudioSource movementAudio;
    public AudioClip digAudio;
    public AudioClip flatAudio;
    public AudioClip serveAudio;

    LogicVolleyball scoreManager;
    PauseMenuVolleyball pauseMenu;
    Animator animator;
    ShotManagerVolleyball shotManager;
    ShotVolleyball currentShot;

    bool superSpeed;
    float superSpeedDuration = 15;
    float superSpeedTime = 0;
    public GameObject superSpeedBox;
    public Image superSpeedBar;
    public TMP_Text superSpeedTimeText;

    // Start is called before the first frame update
    void Start()
    {
        mode = LevelSelectionVolleyball.gameMode;
        movementAudio = GetComponent<AudioSource>();
        movementAudio.volume = 0.2f;
        animator = GetComponent<Animator>();
        scoreManager = GameObject.Find("Logic Volleyball").GetComponent<LogicVolleyball>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenuVolleyball>();

        shotManager = GetComponent<ShotManagerVolleyball>();
        currentShot = shotManager.serve;
        speed = normalSpeed;

        transform.position = playerServePos.position;
        ball.transform.position = transform.position + new Vector3(0, 0.2f, 1.5f);
        ball.GetComponent<Volleyball>().StopBall();
        ball.GetComponent<TrailRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false; // disable the collider when serving

        if (mode == 2)
            aimTarget.position = new Vector3(0, 3.7f, 9f);
        aimTargetInitPos = aimTarget.position;
        jumpVelocity = 0;
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
                //scoreManager.UpdateEffectText("Super Speed", false);
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

        // select a high shot
        if (Input.GetKeyDown(KeyCode.B) && !serving && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.high;
        }

        // select a flat shot
        if (Input.GetKeyDown(KeyCode.V) && !serving && !pauseMenu.gamePaused)
        {
            currentShot = shotManager.flat;
        }

        // when serving, disable gravity and place ball in front of server if ball has not been tossed or was not hit after a toss
        if (serving)
        {
            if (ball.transform.position.y <= yCentre + 0.2)
            {
                ball.GetComponent<Rigidbody>().useGravity = false;
                ball.transform.position = transform.position + new Vector3(0, 0.2f, 1.5f);
                ball.GetComponent<Volleyball>().StopBall();
                ballTossed = false;
            }
            else
                ball.GetComponent<Rigidbody>().useGravity = true;
        }
        // press V to toss the ball up
        if (Input.GetKeyDown(KeyCode.V) && serving && !ballTossed && !pauseMenu.gamePaused)
        {
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0, ballTossForce, 0);
            //animator.Play("serve-prepare");
            ballTossed = true;
        }
        // while ball is in the air, press V again to hit the serve
        else if (Input.GetKeyDown(KeyCode.V) && serving && ball.transform.position.y >= yCentre + 1 && !pauseMenu.gamePaused)
        {
            ballTossed = false;
            serving = false;
            justServed = true;
            //Vector3 dir = aimTarget.position - transform.position;
            //ball.GetComponent<Rigidbody>().velocity = dir.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
            AudioSource.PlayClipAtPoint(serveAudio, transform.position, 1);
            ball.GetComponent<TrailRenderer>().enabled = true;
            animator.Play("serve-vb");

            // https://discussions.unity.com/t/how-to-make-enemy-cannonball-fall-on-moving-target-position/25258/2
            var dir = aimTarget.position - transform.position; // get target direction
            var hdiff = dir.y;  // get height difference
            dir.y = 0;  // retain only the horizontal direction
            var dist = dir.magnitude;  // get horizontal distance
            dir.y = dist + currentShot.upForce;  // set elevation to 45 degrees and adjust by upForce
            dist += hdiff;  // correct for different heights
            var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude) + currentShot.hitForce / 10;
            ball.GetComponent<Rigidbody>().velocity = vel * dir.normalized;

            ball.GetComponent<Volleyball>().hits = 0;
            ball.GetComponent<Volleyball>().hitter = "player";
            ball.GetComponent<Volleyball>().inPlay = true;
            ball.GetComponent<Volleyball>().aimedPos = aimTarget.position;
            ball.GetComponent<Volleyball>().playerShot = "serve";
            currentShot = null;
            aimTarget.position = aimTargetInitPos;

            scoreManager.StartRally();
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
        if ((h != 0 || v != 0) && !aiming && !scoreManager.gameOver)
        {
            // restrict player movement within court area
            if ((transform.position.x < -xLimPlayer && h < 0) || (transform.position.x > xLimPlayer && h > 0))
                h = 0;
            if (serving || (transform.position.z < zLimPlayerBack && v < 0) || (transform.position.z > -0.5 && v > 0))
                v = 0;

            transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

            if ((h != 0 || v != 0) && !pauseMenu.gamePaused)
            {
                if (!movementAudio.isPlaying)
                    movementAudio.Play();
                animator.Play("walking-vb");
            }
        }

        if (h == 0 && v == 0 && movementAudio.isPlaying)
            movementAudio.Stop();

        // jumping: https://gamedevbeginner.com/how-to-jump-in-unity-with-or-without-physics/
        jumpVelocity += gravity * Time.deltaTime;
        if (transform.position.y <= yCentre)
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && jumpVelocity <= 0)
        {
            jumpVelocity = 0;
        }
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !serving && !scoreManager.gameOver)
        {
            jumpVelocity = jumpForce;
        }
        transform.Translate(new Vector3(0, jumpVelocity, 0) * Time.deltaTime);
    }

    public void EnableCollider() // called when oppponent makes first contact after the serve
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    // hit the ball if it is within range of the player
    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        if (other.CompareTag("Ball") && ball.GetComponent<Volleyball>().hits < 3 && ball.GetComponent<Volleyball>().inPlay)
        {
            float hDist = ball.position.x - transform.position.x;
            float vDist = ball.position.z - transform.position.z;

            if (currentShot != null)
            {
                if (mode == 2) // eyegaze: randomly select position of aim target
                {
                    float xNoise = Random.Range(-6f, 6f);
                    float zNoise = Random.Range(-3.5f, 3f);
                    aimTarget.position = new Vector3(aimTargetInitPos.x + xNoise, aimTargetInitPos.y, aimTargetInitPos.z + zNoise);
                }

                ball.GetComponent<Volleyball>().hits = 0;
                ball.GetComponent<Volleyball>().aimedPos = aimTarget.position;
                if (currentShot == shotManager.flat)
                    ball.GetComponent<Volleyball>().playerShot = "flat";
                else
                    ball.GetComponent<Volleyball>().playerShot = "high";

                // https://discussions.unity.com/t/how-to-make-enemy-cannonball-fall-on-moving-target-position/25258/2
                var dir = aimTarget.position - transform.position; // get target direction
                var h = dir.y;  // get height difference
                dir.y = 0;  // retain only the horizontal direction
                var dist = dir.magnitude;  // get horizontal distance
                dir.y = dist + currentShot.upForce;  // set elevation to 45 degrees and adjust by upForce
                dist += h;  // correct for different heights
                var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude) + currentShot.hitForce / 10;
                other.GetComponent<Rigidbody>().velocity = vel * dir.normalized;

                GetComponent<BoxCollider>().enabled = false;
            }
            else // just dig on own side, don't hit over the net
            {
                other.GetComponent<Rigidbody>().velocity = new Vector3(hDist, digForce, vDist);
                ball.GetComponent<Volleyball>().hits++;
            }

            // play audio and animation
            //Vector3 ballDir = ball.position - transform.position;
            if (currentShot == shotManager.high || currentShot == null)
            {
                AudioSource.PlayClipAtPoint(digAudio, transform.position, 1);
                if (ball.position.y > yTop && currentShot == null)
                    animator.Play("pushup");
                else
                    animator.Play("dig");
            }
            else if (currentShot == shotManager.flat)
            {
                AudioSource.PlayClipAtPoint(flatAudio, transform.position, 1);
                animator.Play("flat-vb");
            }

            currentShot = null;
            ball.GetComponent<Volleyball>().hitter = "player";
            aimTarget.position = aimTargetInitPos;
        }
    }

    public void ResetPlayer()
    {
        aiming = false; // in case an aiming key is being held down when Reset is called
        transform.position = playerServePos.position;
        aimTarget.position = aimTargetInitPos;

        // prepare to serve
        serving = true;
        currentShot = shotManager.serve;
        ball.transform.position = transform.position + new Vector3(0, 0, 1);
        ball.GetComponent<Volleyball>().StopBall();
        GetComponent<BoxCollider>().enabled = false;
    }

    public void EnableSuperSpeed()
    {
        superSpeedTime += superSpeedDuration;
        superSpeed = true;
        superSpeedBox.SetActive(true);
        speed = fastSpeed;
        //scoreManager.UpdateEffectText("Super Speed", true);
    }
}
