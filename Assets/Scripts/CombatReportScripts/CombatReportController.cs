using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CombatReportScripts
{
    public class CombatReportController : MonoBehaviour
    {
        // ver progresso da partida
        // separar cada tipo de "ação", "item roubado", (ou outra coisa que a gente quiser), por jogador
        // ex: esfinge, quadros, prisões, etc
        // mvp ganha um bônus

        public static CombatReportController Instance { get; private set; }

        [SerializeField]
        private CombatReportData data;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                Instance = this;
                ResetController();
                SceneManager.sceneLoaded += OnGameStart;
            }
        }

        private void OnGameStart(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == Loader.Scene.GameScene.ToString())
            {
                ResetController();
            }
        }

        private void ResetController()
        {
            ulong clientId = NetworkManager.Singleton.LocalClientId;
            int playerIndex = TheHeistGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(clientId);
            data = new CombatReportData
            {
                playerID = clientId,
                playerName = (FixedString64Bytes)TheHeistGameMultiplayer.Instance.GetPlayerName(),
                playerColor = TheHeistGameMultiplayer.Instance.GetPlayerColor(playerIndex),
                reliquiasColetadas = 0,
                itensColetados = 0,
                dinheiroRecebido = 0,
                vezesPreso = 0,
                distanciaPercorrida = 0,
                itensUsados = 0,
                vezesAtacado = 0,
            };
        }

        public void AddRelicCollected()
        {
            data.reliquiasColetadas++;
        }

        public void AddItemCollected()
        {
            data.itensColetados++;
        }

        public void AddMoneyReceived(int money)
        {
            data.dinheiroRecebido += money;
        }

        public void AddArrested()
        {
            data.vezesPreso++;
        }

        public void AddDistance(float distance)
        {
            data.distanciaPercorrida += distance;
        }

        public void AddAttacked()
        {
            data.vezesAtacado++;
        }
    }
}