using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatReport : NetworkBehaviour
{
    // private static Dictionary<ulong, PlayerWithData> players;

    [SerializeField]
    public CombatReportData data;

    [SerializeField]
    CombatReportUI combatReportUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }
    }

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == Loader.Scene.CombatReportScene.ToString())
        {
            combatReportUI = FindObjectOfType<CombatReportUI>();
            SetUI();
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SendDataToServerRpc()
    {
        
    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void AskForDataRpc()
    {
        ServerSendDataRpc();
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void ServerSendDataRpc()
    {
        combatReportUI.SetUI(data);
    }

    private void SetUI()
    {
        AskForDataRpc();
    }
    // private class PlayerWithData
    // {
    //     internal NetworkClient client;
    //     internal CombatReportData data;
    //
    //     internal PlayerWithData(NetworkClient client, CombatReportData data)
    //     {
    //         this.client = client;
    //         this.data = data;
    //     }
    //
    //     internal PlayerWithData(NetworkClient client)
    //     {
    //         this.client = client;
    //         data = new CombatReportData
    //         {
    //             reliquiasColetadas = 0,
    //             itensColetados = 0,
    //             dinheiroRecebido = 0,
    //             vezesPreso = 0
    //         };
    //     }
    // }

    [Serializable]
    public struct CombatReportData
    {
        [SerializeField]
        internal int reliquiasColetadas;

        [SerializeField]
        internal int itensColetados;

        [SerializeField]
        internal int dinheiroRecebido;

        [SerializeField]
        internal int vezesPreso;

        [SerializeField]
        internal float totalRunTime;
        public override string ToString()
        {
            return
                $"Reliquias coletadas: {reliquiasColetadas}, Itens coletados: {itensColetados}, Dinheiro recebido: {dinheiroRecebido}, Vezes preso: {vezesPreso}";
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(CombatReport))]
    public class CombatReportEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Ask For Data"))
            {
                CombatReport combatReport = (target as CombatReport)!;
                combatReport.AskForDataRpc();
            }

            if (GUILayout.Button("Send Data"))
            {
                CombatReport combatReport = (target as CombatReport)!;
                combatReport.ServerSendDataRpc();
            }

            if (GUILayout.Button("Set UI"))
            {
                CombatReport combatReport = (target as CombatReport)!;
                combatReport.SetUI();
            }
        }
    }

    #endif
}