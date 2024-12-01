using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Generation
{
    [RequireComponent(typeof(RoomSpawn))]
    public class NetworkObjectSpawnController : NetworkBehaviour
    {
        private RoomSpawn roomSpawn;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            roomSpawn = GetComponent<RoomSpawn>();
            if (IsServer)
            {
                StartCoroutine(SpawnObjects());
            }
        }

        private IEnumerator SpawnObjects()
        {
            yield return new WaitUntil(() => roomSpawn.finished.Value);


            foreach (NetworkObjectSpawner spawn in GetSpawns())
            {
                NetworkObject networkObject = spawn.networkObjectToSpawn;
                NetworkObject instance = Instantiate(networkObject, spawn.transform.position, spawn.transform.rotation);
                instance.SpawnWithOwnership(OwnerClientId);
            }
        }

        public List<NetworkObjectSpawner> GetSpawns()
        {
            return FindObjectsOfType<NetworkObjectSpawner>(true).ToList();
        }
    }
}