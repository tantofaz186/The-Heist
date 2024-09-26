using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace CombatReportScripts
{
    public class CombatReport : NetworkBehaviour
    {
        NetworkVariable<CombatReportData> player1 = new();
        NetworkVariable<CombatReportData> player2 = new();
        NetworkVariable<CombatReportData> player3 = new();
        NetworkVariable<CombatReportData> player4 = new();
        NetworkVariable<int> readyClients = new();
        private int numberOfPlayers;

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
            else
            {
                Instance = this;

                SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;

                DontDestroyOnLoad(this);

                if (SceneManager.GetActiveScene().name == Loader.Scene.CombatReportScene.ToString())
                {
                    combatReportUI = FindObjectOfType<CombatReportUI>();
                }
            }
        }

        int called = 0;
        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == Loader.Scene.CombatReportScene.ToString())
            {
                Debug.Log($"Called {++called}");
                combatReportUI = FindObjectOfType<CombatReportUI>();
                SetValuesRpc(SaveSystem.LoadCombatReport());
                if (IsServer) StartCoroutine(WaitForReady());
            }
            else if (arg0.name == Loader.Scene.GameScene.ToString() && IsServer)
            {
                readyClients.Value = 0;
                player1 =
                    player2 =
                        player3 =
                            player4 = new NetworkVariable<CombatReportData>();
            }
        }

        [Rpc(SendTo.Everyone, RequireOwnership = false)]
        public void ServerSendDataRpc()
        {
            List<CombatReportData> playersData = new List<CombatReportData>
            {
                player1.Value,
                player2.Value,
                player3.Value,
                player4.Value,
            };
            combatReportUI.SetUI(playersData);
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetValuesRpc(CombatReportData _data)
        {
            Debug.Log($"{_data.playerID} : {_data.playerName}");
            switch (_data.playerID)
            {
                case 0:
                    player1.Value = _data;
                    break;
                case 1:
                    player2.Value = _data;
                    break;
                case 2:
                    player3.Value = _data;
                    break;
                case 3:
                    player4.Value = _data;
                    break;
                default:
                    Debug.LogError($"Player ID not found : {_data.playerID}");
                    break;
            }
            readyClients.Value++;
        }

        private IEnumerator WaitForReady()
        {
            yield return new WaitUntil(() =>
            {
                Debug.Log($"Waiting for players ready {readyClients.Value} of {NetworkManager.Singleton.ConnectedClients.Count} players ready.");
                return readyClients.Value == NetworkManager.Singleton.ConnectedClients.Count;
            });
            ServerSendDataRpc();
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
                    combatReport.SetValuesRpc(SaveSystem.LoadCombatReport());
                }

                if (GUILayout.Button("Send Data"))
                {
                    CombatReport combatReport = (target as CombatReport)!;
                    combatReport.ServerSendDataRpc();
                }
            }
        }

        #endif
    }
}