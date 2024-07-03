using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // enables this class to be seen from the inspector
public class ShotSquash
{
    public float upForce;
    public float hitForce;
}

public class ShotManagerSquash : MonoBehaviour
{
    public ShotSquash medium;
    public ShotSquash hard;
    public ShotSquash soft;
}
