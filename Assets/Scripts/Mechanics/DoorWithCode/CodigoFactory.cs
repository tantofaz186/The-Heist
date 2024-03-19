using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CodigoFactory : NetworkBehaviour
{
    System.Random random = new System.Random();

    [SerializeField]
    private short[] digitos = new short[4];

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