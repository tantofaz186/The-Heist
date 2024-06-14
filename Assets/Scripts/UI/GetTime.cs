using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetTime : MonoBehaviour
{
   
    [SerializeField] TextMeshProUGUI timerText;
    
    private void Start()
    {
        timerText.gameObject.SetActive(true);
    }


    private void Update()
    {
        int minutes = Mathf.FloorToInt(Timer.instance.remainingTime.Value / 60);
        int seconds = Mathf.FloorToInt(Timer.instance.remainingTime.Value% 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if(Timer.instance.remainingTime.Value <= 60)
        {
            OneMinute();
        }
    }
    
    
    void OneMinute()
    {
        timerText.color = Color.red;
    }

    
}
