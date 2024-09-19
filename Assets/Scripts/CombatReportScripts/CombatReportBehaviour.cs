using System;
using System.Collections;
using QFSW.QC.Actions;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Utils;

namespace CombatReportScripts
{
    public class CombatReportBehaviour : NetworkBehaviour
    {
        public CombatReportData combatReportData;
        public static CombatReportBehaviour Instance;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            Instance = this;
            ResetData();
        }

        private void ResetData()
        {
            PlayerData data = TheHeistGameMultiplayer.Instance.GetPlayerData();
            combatReportData.playerID = data.clientId;
            combatReportData.playerName = data.playerName;
            combatReportData.playerColor = TheHeistGameMultiplayer.Instance.GetPlayerColor(data.colorId);
            
            combatReportData = new CombatReportData
            {
                playerID = data.clientId,
                playerName =  data.playerName,
                playerColor = TheHeistGameMultiplayer.Instance.GetPlayerColor(data.colorId),
                reliquiasColetadas = 0,
                itensColetados = 0,
                dinheiroRecebido = 0,
                vezesPreso = 0,
                distanciaPercorrida = 0,
                itensUsados = 0,
                vezesAtacado = 0,
            };
        }
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerActions.Instance.ready);
            PlayerActions.Instance.PlayerInputActions.Player.Item1.performed += Item1Onperformed;
            PlayerActions.Instance.PlayerInputActions.Player.Item2.performed += Item2Onperformed;
            PlayerActions.Instance.PlayerInputActions.Player.Item3.performed += Item3Onperformed;
        }

        private void Item1Onperformed(InputAction.CallbackContext obj)
        {
            combatReportData.dinheiroRecebido += 50;
        }

        private void Item2Onperformed(InputAction.CallbackContext obj)
        {
            combatReportData.itensUsados++;
        }

        private void Item3Onperformed(InputAction.CallbackContext obj)
        {
            combatReportData.itensColetados++;
        }

        public void Save()
        {
            SaveSystem.SaveCombatReport(combatReportData);
        }
        private void OnDisable()
        {
            Save();
            if (PlayerActions.Instance != null)
            {
                PlayerActions.Instance.PlayerInputActions.Player.Item1.performed -= Item1Onperformed;
                PlayerActions.Instance.PlayerInputActions.Player.Item2.performed -= Item2Onperformed;
                PlayerActions.Instance.PlayerInputActions.Player.Item3.performed -= Item3Onperformed;
            }
        }

    }
}