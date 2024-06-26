using System;
using UnityEngine;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    public static Timer instance { get; private set; }

    [SerializeField]
    public NetworkVariable<float> remainingTime;

    [SerializeField]
    public NetworkVariable<float> totalTime;

    [SerializeField]
    private float startTime;

    private void Awake()
    {
        instance = this;
        totalTime.Value = startTime;
        remainingTime.Value = startTime;
        combatReport = FindObjectOfType<CombatReport>();
        InvokeRepeating(nameof(CointTime), 1, 1);
    }

    CombatReport combatReport;

    public override void OnDestroy()
    {
        if (IsServer) combatReport.data.totalRunTime = totalTime.Value - remainingTime.Value;
        base.OnDestroy();
    }

    void CointTime()
    {
        if (!IsServer) return;
        if (remainingTime.Value > 0)
        {
            remainingTime.Value -= 1;
        }

        if (remainingTime.Value <= 0)
        {
            StopGameRpc();
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    void StopGameRpc()
    {
        Debug.Log("StopGameRpc");
        Loader.LoadCombatReportScene();
        remainingTime.Value = 0;
    }
}