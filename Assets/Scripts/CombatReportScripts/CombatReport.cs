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
    List<CombatReportData> playersData;

    [SerializeField]
    CombatReportUI combatReportUI;

    public static CombatReport Instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        DontDestroyOnLoad(this);

        if (IsServer)
        {
            data = new CombatReportData();
            Instance = this;
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }
        if(SceneManager.GetActiveScene().name == Loader.Scene.CombatReportScene.ToString())
        {
            combatReportUI = FindObjectOfType<CombatReportUI>();
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
    public void AskForDataRpc()
    {
        ServerSendDataRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SendDataToServerRpc(CombatReportData _data)
    {
        playersData.Add(_data);
    }
    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void AskForAllDataRpc()
    {
        SendDataToServerRpc(data);
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

    [Serializable]
    public struct CombatReportData : INetworkSerializable
    {
        [SerializeField]
        internal Color playerColor;

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

        [SerializeField]
        internal float distanciaPercorrida;

        [SerializeField]
        internal int abriuPrisoes;

        [SerializeField]
        internal int itensUsados;

        [SerializeField]
        internal int vezesAtacado;

        [SerializeField]
        internal int vezesVisto;

        public override string ToString()
        {
            return
                $"Reliquias coletadas: {reliquiasColetadas}, Itens coletados: {itensColetados}, Dinheiro recebido: {dinheiroRecebido}, Vezes preso: {vezesPreso}";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerColor);
            serializer.SerializeValue(ref reliquiasColetadas);
            serializer.SerializeValue(ref itensColetados);
            serializer.SerializeValue(ref dinheiroRecebido);
            serializer.SerializeValue(ref vezesPreso);
            serializer.SerializeValue(ref totalRunTime);
            serializer.SerializeValue(ref distanciaPercorrida);
            serializer.SerializeValue(ref abriuPrisoes);
            serializer.SerializeValue(ref itensUsados);
            serializer.SerializeValue(ref vezesAtacado);
            serializer.SerializeValue(ref vezesVisto);

        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(CombatReport))]
    public class CombatReportEditor : Editor
    {
        CombatReportData data;
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

            data.abriuPrisoes = EditorGUILayout.IntField("Abriu Prisoes", data.abriuPrisoes);
            data.dinheiroRecebido = EditorGUILayout.IntField("Dinheiro Recebido", data.dinheiroRecebido);
            data.distanciaPercorrida = EditorGUILayout.FloatField("Distancia Percorrida", data.distanciaPercorrida);
            data.itensColetados = EditorGUILayout.IntField("Itens Coletados", data.itensColetados);
            data.itensUsados = EditorGUILayout.IntField("Itens Usados", data.itensUsados);
            data.playerColor = EditorGUILayout.ColorField("Player Color", data.playerColor);
            data.reliquiasColetadas = EditorGUILayout.IntField("Reliquias Coletadas", data.reliquiasColetadas);
            data.totalRunTime = EditorGUILayout.FloatField("Total Run Time", data.totalRunTime);
            data.vezesAtacado = EditorGUILayout.IntField("Vezes Atacado", data.vezesAtacado);
            data.vezesPreso = EditorGUILayout.IntField("Vezes Preso", data.vezesPreso);
            data.vezesVisto = EditorGUILayout.IntField("Vezes Visto", data.vezesVisto);

            if (GUILayout.Button("Send Data"))
            {
                CombatReport combatReport = (target as CombatReport)!;
                combatReport.SendDataToServerRpc(data);
            }
        }
    }

    #endif
}