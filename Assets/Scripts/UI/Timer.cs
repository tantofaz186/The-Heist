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
    private NetworkVariable<float> timeRan;

    [SerializeField]
    private float startTime;

    public event Action OnTimerEnd;

    private void Awake()
    {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        totalTime.Value = startTime;
        remainingTime.Value = startTime;
        timeRan.Value = 0;
        if (IsServer) InvokeRepeating(nameof(CountTime), 1, 1);
    }

    void CountTime()
    {
        if (remainingTime.Value > 0)
        {
            remainingTime.Value -= 1;
            timeRan.Value += 1;
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
        OnTimerEnd?.Invoke();
        Loader.LoadCombatReportScene();
        remainingTime.Value = 0;
    }
}