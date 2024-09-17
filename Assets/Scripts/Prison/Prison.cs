using System;
using System.Collections;
using System.Collections.Generic;
using CombatReportScripts;
using Unity.Netcode;
using UnityEngine;

public class Prison : NetworkBehaviour
{
    public static Prison instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public NetworkVariable<int> totalPrisoners = new NetworkVariable<int>(0);
    public Transform prisonTransform;

    public void FindPrison()
    {
        prisonTransform = GameObject.FindGameObjectWithTag("Prison").transform;
    }

    [Rpc(SendTo.Server)]
    public void AddPrisonerRpc()
    {
        Debug.Log("AddPrisonerRpc");
        totalPrisoners.Value++;
        if (totalPrisoners.Value >= NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            StopGameRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void ReleasePrisonerRpc()
    {
        Debug.Log("ReleasePrisonerRpc");
        totalPrisoners.Value = 0;
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    void StopGameRpc()
    {
        Debug.Log("EveryOneInPrison");
        Loader.LoadCombatReportScene();
    }
}