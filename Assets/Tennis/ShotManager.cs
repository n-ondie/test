using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // enables this class to be seen from the inspector
public class Shot
{
    public float upForce;
    public float hitForce;
}

public class ShotManager : MonoBehaviour
{
    // when this script is added to the player, you can adjust their upForce and hitForce in Unity for each of the following shot types
    public Shot topspin;
    public Shot flat;
    public Shot flatServe;
    public Shot kickServe;
    public Shot powerShot;
}
