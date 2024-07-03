using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraVolleyball : MonoBehaviour
{
    public Transform player;
    public Transform ball;

    float zDiff;
    float zDiffFront = -10;
    float zDiffFrontPos = -10;
    float zDiffBack = -7;
    float zDiffBackPos = -16;
    float zEndWall = 30;

    float minAngle = 8.5f;
    float maxAngle = 30;

    // Start is called before the first frame update
    void Start()
    {
        zDiff = zDiffBack;
    }

    // LateUpdate is called once per frame, after all Update functions have been called
    void LateUpdate()
    {
        // calculate how much to rotate the camera upwards if the ball is too high
        float zDistBall = ball.position.z - transform.position.z;
        float yDistBall = ball.position.y - transform.position.y;
        float angleUpToBall = Mathf.Atan(yDistBall / zDistBall) / Mathf.PI * 180;
        float angleAdjust;
        if (angleUpToBall <= minAngle || zDistBall <= 0)
            angleAdjust = 0;
        else if (angleUpToBall >= maxAngle)
            angleAdjust = maxAngle - minAngle;
        else // (angleUpToBall > minAngle && angleUpToBall < maxAngle)
            angleAdjust = angleUpToBall - minAngle;

        // calculate how far behind the player to position the camera
        if (player.transform.position.z <= zDiffBackPos)
            zDiff = zDiffBack;
        else if (player.transform.position.z >= zDiffFrontPos)
            zDiff = zDiffFront;
        else
            zDiff = (player.transform.position.z - zDiffBackPos) / (zDiffFrontPos - zDiffBackPos) * (zDiffFront - zDiffBack) + zDiffBack;

        // calculate how much to rotate the camera sideways to always face the centre of the end wall
        float zDistWall = zEndWall - transform.position.z;
        float angleSideways = Mathf.Atan(transform.position.x / zDistWall) / Mathf.PI * 180; // positive if player is on the right

        transform.position = player.transform.position + new Vector3(0, 5, zDiff);
        transform.rotation = Quaternion.Euler(new Vector3(20 - angleAdjust, -angleSideways, 0));
    }
}
