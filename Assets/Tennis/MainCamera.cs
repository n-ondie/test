using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform player;

    float xPos;
    float yPos = 12 + 3;
    float zPos;
    float xLim = 7.5f;
    float zBoundary = -22.8f;
    float zDefault = -36;
    //float zBack = -40.8f;
    float zDiff = 13.2f;

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

        //if (player.transform.position.z < -27.6)
        //    zPos = zBack;
        if (player.transform.position.z < zBoundary)
        {
            zPos = player.transform.position.z - zDiff;
        }
        else
            zPos = zDefault;

        // calculate how much to rotate the camera sideways to always face the centre of the end wall
        //float zDistWall = 32.4f - transform.position.z;
        //float angleSideways = Mathf.Atan(transform.position.x / zDistWall) / Mathf.PI * 180; // positive if player is on the right

        transform.position = new Vector3(xPos, yPos, zPos);
        //transform.rotation = Quaternion.Euler(new Vector3(15, -angleSideways, 0));
    }
}
