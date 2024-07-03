using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HangGliderPointCounter : MonoBehaviour
{
    public float point;
    private float pointFactor = 1;
    [SerializeField] private GameObject ScoreUI;
    
    // Start is called before the first frame update
    void Start()
    {
        point = 0;
    }

    // Update is called once per frame
    void Update()
    {
        point += Time.deltaTime * pointFactor;
        ScoreUI.GetComponent<TextMeshProUGUI>().text =  Mathf.RoundToInt(point).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        point += other.GetComponent<GravityPull>().point;
        ScoreUI.GetComponent<TextMeshProUGUI>().text =  Mathf.RoundToInt(point).ToString();

    }
}
