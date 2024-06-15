using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CombatReport : NetworkBehaviour
{
    private static Dictionary<ulong, PlayerWithData> players;

    [SerializeField]
    private float timeToInvoke = 2f;
    

    private void Start()
    {

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        players = new Dictionary<ulong, PlayerWithData>();
        players.Add(NetworkManager.Singleton.LocalClientId, new PlayerWithData(NetworkManager.Singleton.LocalClient));
        Invoke(nameof(NewMethod), timeToInvoke);
    }

    private void NewMethod()
    {
        // Debug.Log("players.Keys");
        // foreach (var key in players.Keys)
        // {
        //     Debug.Log(key);
        // }
        // Debug.Log("players.Values");
        //
        // foreach (var value in players.Values)
        // {
        //     Debug.Log(value.client);
        // }
        // Debug.Log("players.Values.Select(p => p.data)");
        // foreach (var player in players.Values)
        // {
        //     Debug.Log(player.data);
        // }
        Invoke(nameof(NewMethod), timeToInvoke);
    }

    private class PlayerWithData
    {
        internal NetworkClient client;
        internal CombatReportData data;

        internal PlayerWithData(NetworkClient client, CombatReportData data)
        {
            this.client = client;
            this.data = data;
        }

        internal PlayerWithData(NetworkClient client)
        {
            this.client = client;
            data = new CombatReportData
            {
                reliquiasColetadas = 0,
                itensColetados = 0,
                dinheiroRecebido = 0,
                vezesPreso = 0
            };
        }
    }

    internal struct CombatReportData
    {
        internal int reliquiasColetadas;
        internal int itensColetados;
        internal int dinheiroRecebido;
        internal int vezesPreso;
        public override string ToString()
        {
            return $"Reliquias coletadas: {reliquiasColetadas}, Itens coletados: {itensColetados}, Dinheiro recebido: {dinheiroRecebido}, Vezes preso: {vezesPreso}";
        }
    }
}