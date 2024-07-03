using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraSquash : MonoBehaviour
{
    public Transform player;

    float xPos;
    float yPos = 6 + 50;
    //float zPos;
    float xLim = 4;

    float zDiff;
    float zDiffFront = -11;
    float zDiffFrontPos = -6;
    float zDiffBack = -9;
    float zDiffBackPos = -8;
    //float zEndWall = 10.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // LateUpdate is called once per frame, after all Update functions have been called
    void LateUpdate()
    {
        if (player.transform.position.x < -xLim)
            xPos = -xLim;
        else if (player.transform.position.x > xLim)
            xPos = xLim;
        else
            xPos = player.transform.position.x;

        // calculate how far behind the player to position the camera
        if (player.transform.position.z <= zDiffBackPos)
            zDiff = zDiffBack;
        else if (player.transform.position.z >= zDiffFrontPos)
            zDiff = zDiffFront;
        else
            zDiff = (player.transform.position.z - zDiffBackPos) / (zDiffFrontPos - zDiffBackPos) * (zDiffFront - zDiffBack) + zDiffBack;

        // calculate how much to rotate the camera sideways to always face the centre of the end wall
        //float zDistWall = zEndWall - transform.position.z;
        //float angleSideways = Mathf.Atan(transform.position.x / zDistWall) / Mathf.PI * 180; // positive if player is on the right

        transform.position = new Vector3(xPos, yPos, player.transform.position.z + zDiff);
        //transform.rotation = Quaternion.Euler(new Vector3(15, -angleSideways, 0));
    }
}
