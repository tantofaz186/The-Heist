using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CodigoFactory : SingletonNetwork<CodigoFactory>
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
            AskServerForCode();
        }
    }

    public void AskServerForCode()
    {
        if (IsServer) return;
        Debug.Log("Pedindo código para o servidor.");
        ServerGiveCodeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerGiveCodeServerRpc(ServerRpcParams rpcParams = default)
    {
        if (IsServer) ClientReceiveCodeClientRpc(digitos);
    }

    [ClientRpc]
    private void ClientReceiveCodeClientRpc(short[] code)
    {
        if (!IsServer)
        {
            digitos = code;
            Debug.Log("Código recebido: " + code[0] + code[1] + code[2] + code[3]);
        }
    }

    public short[] GetCodigo()
    {
        return digitos;
    }

    public bool CheckCodigo(short[] codigo)
    {
        if (codigo.Length != digitos.Length) return false;
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