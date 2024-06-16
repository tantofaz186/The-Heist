using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatReportUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalMoneyText;
    [SerializeField] TextMeshProUGUI totalItemsText;
    [SerializeField] TextMeshProUGUI totalPrisonsText;
    [SerializeField] TextMeshProUGUI totalTimeText;


    public void Start()
    {
        SetUI();
    }
    
    public void SetUI()
    {
        totalMoneyText.text = "Total Money: " + CombatReportData.instance.dinheiroTotal;
        totalItemsText.text = "Collected Items: " + CombatReportData.instance.reliquiasColetadas;
        totalPrisonsText.text = "Total Prisons: " + CombatReportData.instance.vezesPreso;
        float totalRunTime = CombatReportData.instance.tempoRun;
        int minutes = Mathf.FloorToInt( totalRunTime / 60);
        int seconds = Mathf.FloorToInt(totalRunTime% 60);
        totalTimeText.text = "Run Time: " +string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
