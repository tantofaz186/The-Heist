using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatReportData : MonoBehaviour
{
    public static CombatReportData instance { get; private set; }
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    public int reliquiasColetadas;
    public int dinheiroTotal;
    public int vezesPreso;
    public float tempoRun;



    public void AtualzarDados()
    {
        reliquiasColetadas = TotalMoney.instance.totalItems.Value;
        dinheiroTotal = TotalMoney.instance.totalMoney.Value;
        vezesPreso = Prison.instance.vezesPreso.Value;
        tempoRun = Timer.instance.totalTime.Value- Timer.instance.remainingTime.Value;
    }
}
