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
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        totalTime.Value = startTime;
        remainingTime.Value = startTime;
        InvokeRepeating(nameof(CountTime), 1, 1);
    }

    public override void OnDestroy()
    {
        if (IsServer)
        {
            CombatReport.Instance.data.totalRunTime = totalTime.Value - remainingTime.Value;
        }

        base.OnDestroy();
    }

    void CountTime()
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

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void ChangeTimeRpc(float time)
    {
        remainingTime.Value = time;
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    void StopGameRpc()
    {
        Debug.Log("StopGameRpc");
        Loader.LoadCombatReportScene();
        remainingTime.Value = 0;
    }
}