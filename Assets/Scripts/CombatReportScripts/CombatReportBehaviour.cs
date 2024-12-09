using Unity.Netcode;
using Utils;

namespace CombatReportScripts
{
    public class CombatReportBehaviour : NetworkBehaviour
    {
        public CombatReportData combatReportData;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(!IsOwner)
            {
                enabled = false;
                return;
            }
            ResetData();
        }

        public void ResetData()
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
                pulos = 0,
            };
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                CombatReport.Instance.SetValuesRpc(combatReportData);
            }
            base.OnNetworkDespawn();
        }
    }
}