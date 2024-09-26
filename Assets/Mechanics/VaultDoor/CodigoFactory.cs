using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace Mechanics.VaultDoor
{
    public class CodigoFactory : NetworkBehaviour
    {
        Random random = new ();

        [SerializeField]
        private short[] digitos = new short[4];
        [SerializeField]List<CodigoSpawnItem> possibleItemToSpawn = new List<CodigoSpawnItem>();
        [SerializeField]List<CodigoSpawnItem> spawnedItems = new List<CodigoSpawnItem>();

        [SerializeField]
        private int[] itemsCheck;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            StartCoroutine(WaitToSpawn());
        }
        const float timeout = 5.0f;
        private IEnumerator WaitToSpawn()
        {
            float elapsedTime = 0.0f;
            yield return new WaitUntil(() =>  FindObjectsOfType<CodigoSpawnItem>().Length > 0);
            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;
                return (FindObjectsOfType<CodigoSpawnItem>().Length >= 4 && elapsedTime >= timeout/2f)|| elapsedTime >= timeout;
            });
            possibleItemToSpawn.AddRange(FindObjectsByType<CodigoSpawnItem>(FindObjectsSortMode.InstanceID));
            if (IsServer)
            {
                Debug.Log("Gerando código.");
                for (int i = 0; i < digitos.Length; i++)
                {
                    digitos[i] = (short)random.Next(0, 10);
                }
                Debug.Log("Código gerado: " + digitos[0] + digitos[1] + digitos[2] + digitos[3]);
                StartCoroutine(PickFourObjectsAtRandom());
            }
            else
            {
                AskServerForCodeServerRpc();
            }
        }

        private void SetItemText()
        {
            for (int i = 0; i < digitos.Length; i++)
            {
                spawnedItems[i].setText(i, digitos[i]);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskServerForCodeServerRpc(ServerRpcParams rpcParams = default)
        {
            Debug.Log("Pedindo código para o servidor.");
            SendCodeToClientRpc(digitos, itemsCheck.ToArray(), rpcParams.Receive.SenderClientId);
        }

        [Rpc(SendTo.Everyone,RequireOwnership = false)]
        private void SendCodeToClientRpc(short[] code, int[] intList, ulong clientId = 0)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                return;
            }
            digitos = code;
            Debug.Log("Código recebido: " + code[0] + code[1] + code[2] + code[3]);
            itemsCheck = intList;
            for (int i = 0; i < digitos.Length; i++)
            {
                int index = itemsCheck[i];
                CodigoSpawnItem item = possibleItemToSpawn[index];
                item.setText(i, digitos[i]);
                spawnedItems.Add(item);
            }
            
            Debug.Log("cheguei");
            EndSetupRpc();
        }

        [Rpc(SendTo.Server,RequireOwnership = false)]
        private void EndSetupRpc()
        {
            SetupCodigoFactoryRpc();
        }
        
        [Rpc(SendTo.Everyone)]
        private void SetupCodigoFactoryRpc()
        {
            FindObjectOfType<VaultDoorBehaviour>().Initialize(this);
            Debug.Log("Código recebido e objetos spawnados.");
        }

        public short[] GetCodigo()
        {
            return digitos;
        }
        private IEnumerator PickFourObjectsAtRandom()
        {
            if(possibleItemToSpawn.Count == 0)
            {
                Debug.LogError("Não há objetos para spawnar os itens.");
                yield break;
            }
            if (possibleItemToSpawn.Count < digitos.Length)
            {
                Debug.LogWarning("Não há objetos suficientes para spawnar os itens.");
                int index = random.Next(0, possibleItemToSpawn.Count);
                CodigoSpawnItem item = possibleItemToSpawn[index];
                item.setText(0, digitos[0]);
                item.setText(1, digitos[1]);
                item.setText(2, digitos[2]);
                item.setText(3, digitos[3]);

                spawnedItems.AddRange(new []{item, item, item, item});
                possibleItemToSpawn.RemoveAt(index);
                yield break;
            }

            itemsCheck = new int[possibleItemToSpawn.Count];
            for(int i = 0; i < itemsCheck.Length; i++)
            {
                itemsCheck[i] = i;
            }
            itemsCheck = itemsCheck.OrderBy((item) => random.Next()).ToArray();
            for (int i = 0; i < digitos.Length; i++)
            {
                int index = itemsCheck[i];
                CodigoSpawnItem item = possibleItemToSpawn[index];
                item.setText(i, digitos[i]);
                spawnedItems.Add(item);
            }
        }
        public bool CheckCodigo(short[] codigo)
        {
            //essa linha evita que o usuário envie um código vazio
            if (codigo.Length == 0) return false;
            var isCode = !digitos.Where((t, i) => t != codigo[i]).Any();
            SendEventToClientsRpc(isCode);
            return isCode;
        }

        [Rpc(SendTo.Everyone)]
        public void SendEventToClientsRpc(bool isCorrectCode)
        {
            Debug.Log("Evento enviado");
            FindObjectOfType<VaultDoorBehaviour>().CodeCheck(isCorrectCode);
        }
        public void ChangeCodigo()
        {
            for (int i = 0; i < digitos.Length; i++)
            {
                digitos[i] = (short)random.Next(0, 10);
            }
        }
    }
}