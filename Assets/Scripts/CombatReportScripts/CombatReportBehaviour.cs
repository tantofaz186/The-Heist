using Unity.Netcode;

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
            PlayerData data = TheHeistGameMultiplayer.Instance.GetPlayerData();
            combatReportData.playerID = data.clientId;
            combatReportData.playerName = data.playerName;
            combatReportData.playerColor = TheHeistGameMultiplayer.Instance.GetPlayerColor(data.colorId);
        }

    }
}