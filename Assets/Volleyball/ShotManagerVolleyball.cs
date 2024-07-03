using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // enables this class to be seen from the inspector
public class ShotVolleyball
{
    public float upForce;
    public float hitForce;
}

public class ShotManagerVolleyball : MonoBehaviour
{
    public ShotVolleyball high;
    public ShotVolleyball flat;
    public ShotVolleyball serve;
}
