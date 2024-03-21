using System.Collections.Generic;
using System.Linq;
using Mecanicas.PortaComCodigo;
using Unity.Netcode;
using UnityEngine;

public class CodigoFactory : NetworkBehaviour
{
    System.Random random = new System.Random();

    [SerializeField]
    private short[] digitos = new short[4];
    List<CodigoSpawnItem> possibleItemToSpawn = new List<CodigoSpawnItem>();
    List<CodigoSpawnItem> spawnedItems = new List<CodigoSpawnItem>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            Debug.Log("Gerando código.");
            for (int i = 0; i < digitos.Length; i++)
            {
                digitos[i] = (short)random.Next(0, 10);
            }
            Debug.Log("Código gerado: " + digitos[0] + digitos[1] + digitos[2] + digitos[3]);
            possibleItemToSpawn.AddRange(FindObjectsOfType<CodigoSpawnItem>());
            PickFourObjectsAtRandom();
        }
        else
        {
            AskServerForCodeServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AskServerForCodeServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("Pedindo código para o servidor.");
        SendCodeToClientRpc(digitos, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SendCodeToClientRpc(short[] code, ulong clientId = 0)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        digitos = code;
        Debug.Log("Código recebido: " + code[0] + code[1] + code[2] + code[3]);
    }

    public short[] GetCodigo()
    {
        return digitos;
    }
    private void PickFourObjectsAtRandom()
    {
        if(possibleItemToSpawn.Count == 0)
        {
            Debug.LogError("Não há objetos para spawnar os itens.");
            return;
        }
        if (possibleItemToSpawn.Count < digitos.Length)
        {
            Debug.LogWarning("Não há objetos suficientes para spawnar os itens.");
            int index = Random.Range(0, possibleItemToSpawn.Count);
            CodigoSpawnItem item = possibleItemToSpawn[index];
            item.setText(0, digitos[0]);
            item.setText(1, digitos[1]);
            item.setText(2, digitos[2]);
            item.setText(3, digitos[3]);

            spawnedItems.AddRange(new []{item, item, item, item});
            possibleItemToSpawn.RemoveAt(index);
            return;
        }
        for (int i = 0; i < digitos.Length; i++)
        {
            int index = Random.Range(0, possibleItemToSpawn.Count);
            CodigoSpawnItem item = possibleItemToSpawn[index];
            item.setText(i, digitos[i]);
            spawnedItems.Add(item);
            possibleItemToSpawn.RemoveAt(index);
        }
    }
    public bool CheckCodigo(short[] codigo)
    {
        //essa linha evita que o usuário envie um código vazio
        if (codigo.Length == 0) return false;
        return !digitos.Where((t, i) => t != codigo[i]).Any();
    }

    public void CheckCodigo(string codigo)
    {
        bool isCode = CheckCodigo(codigo.Select(c => short.Parse(c.ToString())).ToArray());

        Debug.Log(isCode ? "Código correto" : "Código incorreto");
    }

    public void ChangeCodigo()
    {
        for (int i = 0; i < digitos.Length; i++)
        {
            digitos[i] = (short)random.Next(0, 10);
        }
    }
}