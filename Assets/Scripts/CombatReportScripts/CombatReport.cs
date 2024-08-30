using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CombatReportScripts
{
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

            if (Instance != null)
            {
                Destroy(gameObject);
            }
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
            else if (arg0.name == Loader.Scene.GameScene.ToString())
            {
                data = new CombatReportData();
                playersData = new List<CombatReportData>();
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

                data.dinheiroRecebido = EditorGUILayout.IntField("Dinheiro Recebido", data.dinheiroRecebido);
                data.distanciaPercorrida = EditorGUILayout.FloatField("Distancia Percorrida", data.distanciaPercorrida);
                data.itensColetados = EditorGUILayout.IntField("Itens Coletados", data.itensColetados);
                data.itensUsados = EditorGUILayout.IntField("Itens Usados", data.itensUsados);
                data.playerColor = EditorGUILayout.ColorField("Player Color", data.playerColor);
                data.reliquiasColetadas = EditorGUILayout.IntField("Reliquias Coletadas", data.reliquiasColetadas);
                data.vezesAtacado = EditorGUILayout.IntField("Vezes Atacado", data.vezesAtacado);
                data.vezesPreso = EditorGUILayout.IntField("Vezes Preso", data.vezesPreso);

                if (GUILayout.Button("Send Data"))
                {
                    CombatReport combatReport = (target as CombatReport)!;
                    combatReport.SendDataToServerRpc(data);
                }
            }
        }

        #endif
    }
}