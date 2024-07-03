using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTarget : MonoBehaviour
{
    float speed = 1.5f;
    Vector3 targetPos;
    //[SerializeField] Transform centre;
    //[SerializeField] Transform centreLeft;
    //[SerializeField] Transform centreRight;
    //[SerializeField] Transform front;
    //[SerializeField] Transform frontLeft;
    //[SerializeField] Transform frontRight;
    //[SerializeField] Transform back;
    //[SerializeField] Transform backLeft;
    //[SerializeField] Transform backRight;
    public Transform[] courtPoints;

    public Transform ball;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = ChooseNextPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == targetPos)
            targetPos = ChooseNextPos();

        if (ball.GetComponent<SquashBall>().inPlay)
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    Vector3 ChooseNextPos()
    {
        int randVal = Random.Range(0, courtPoints.Length);
        return courtPoints[randVal].position;
    }
}
