using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuartPointTrigger : MonoBehaviour
{
    public GameObject LapCompleteTrig;
    public GameObject QuarterLapTrig;

    private void OnTriggerEnter()
    { 
        LapCompleteTrig.SetActive(true);    
        QuarterLapTrig.SetActive(false);
    }
}
