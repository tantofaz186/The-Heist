using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CombatReportScripts
{
    public class CombatReport : NetworkBehaviour
    {

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
            Instance = this;
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;

            DontDestroyOnLoad(this);

            if (SceneManager.GetActiveScene().name == Loader.Scene.CombatReportScene.ToString())
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
            // SendDataToServerRpc(data);
        }

        [Rpc(SendTo.Everyone, RequireOwnership = false)]
        public void ServerSendDataRpc()
        {
            combatReportUI.SetUI(playersData);
        }

        private void SetUI()
        {
            System.Random rnd = new System.Random();
            List<CombatReportData> temp = new List<CombatReportData>();
            foreach (CombatReportData data in playersData)
            {
                var aux = data;

                char[] playerName = new char[rnd.Next(5, 50)];
                for (int j = 0; j < playerName.Length; j++)
                {
                    playerName[j] = (char)rnd.Next(65, 90);
                }

                aux.playerName = new string(playerName);
                temp.Add(aux);
            }


            combatReportUI.SetUI(temp);
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
                    combatReport.AskForDataRpc();
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

                if (GUILayout.Button("Send Random Data"))
                {
                    CombatReport combatReport = (target as CombatReport)!;


                    char[] playerName = new char[Random.Range(5, 20)];
                    for (int j = 0; j < playerName.Length; j++)
                    {
                        playerName[j] = (char)Random.Range(65, 90);
                    }

                    combatReport.SendDataToServerRpc(
                        new CombatReportData()
                        {
                            dinheiroRecebido = Random.Range(0, 100),
                            distanciaPercorrida = Random.Range(40f, 10000f),
                            itensColetados = Random.Range(0, 100),
                            itensUsados = Random.Range(0, 50),
                            playerColor = Random.ColorHSV(saturationMin: 1, saturationMax: 1, hueMin: 0, hueMax: 1, valueMin: 1,
                                valueMax: 1),
                            reliquiasColetadas = Random.Range(0, 100),
                            vezesAtacado = Random.Range(0, 100),
                            vezesPreso = Random.Range(0, 100),
                            playerName = new string(playerName),
                        });
                }
            }
        }

        #endif
    }
}