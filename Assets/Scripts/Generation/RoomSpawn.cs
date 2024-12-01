using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Generation;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSpawn : NetworkBehaviour
{
    [SerializeField] List<GameObject> hallways = new List<GameObject>();

    [SerializeField] Transform mapCenter;

    [SerializeField] List<GameObject> roomSpawnpoints = new();

    [SerializeField] public NetworkVariable<bool> finished = new NetworkVariable<bool>(false);

    [SerializeField] private List<RoomTypes> themeRooms = new();

    [SerializeField] private List<int> themeRoomsCheck = new();

    [SerializeField] private List<RoomTypes> normalRooms = new();

    [SerializeField] private List<int> normalRoomsCheck = new();

    [SerializeField] private GameObject vaultRoom;

    [SerializeField] private GameObject prison;

    [SerializeField] public List<GameObject> playerSpawnPoints = new();

    public NavMeshBake bake;
    public EnemySpawn enemySpawn;
    public ItemSpawn itemSpawn;
    public DoorSpawn doorSpawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            SpawnSceneRpc();
        }

        StartCoroutine(WaitToBake());
    }

    private IEnumerator WaitToBake()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => bake.surface.navMeshData != null || bake.surface != null);
        bake.surface.BuildNavMesh();
        Prison.instance.FindPrison();
        if (IsServer)
        {
            SpawnDoorRpc();
            SpawnEnemyRpc();
            SpawnItemRpc();
            finished.Value = true;
        }
    }

    [Rpc(SendTo.Server)]
    void SpawnSceneRpc()
    {
        SortHallway();
        roomSpawnpoints = GetRoomSpawnPoints();
        SortRooms();
    }

    [Rpc(SendTo.Server)]
    void SpawnEnemyRpc()
    {
        enemySpawn.SpawnEnemyRpc();
    }

    [Rpc(SendTo.Server)]
    void SpawnDoorRpc()
    {
        doorSpawn.SpawnDoorsRpc();
    }

    [Rpc(SendTo.Server)]
    void SpawnItemRpc()
    {
        itemSpawn.SpawnItemsRpc();
    }

    void SortHallway()
    {
        int rnd = Random.Range(0, hallways.Count);
        var instance = Instantiate(hallways[rnd], mapCenter);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
    }

    List<GameObject> GetRoomSpawnPoints()
    {
        return GameObject.FindGameObjectsWithTag("RoomSpawnPoints").ToList();
    }

    List<GameObject> GetSpawnPoints()
    {
        return GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
    }

    void SortRooms()
    {
        SpawnVault();
        SpawnPrison();
        for (int n = 0; n < roomSpawnpoints.Count; n++)
        {
            int type = roomSpawnpoints[n].GetComponent<SpawnInfo>().spawnType;
            switch (type)
            {
                case 0:
                    SpawnThemeRoom(n);

                    break;
                case 1:
                    SpawnNormalRoom(n);

                    break;
            }
        }
    }

    void SpawnThemeRoom(int x)
    {
        int rnd = Random.Range(0, themeRooms.Count);

        bool inList = false;
        if (themeRoomsCheck.Count <= 0)
        {
            for (int i = 0; i < themeRooms.Count; i++)
            {
                themeRoomsCheck.Add(i);
            }
        }


        for (int i = 0; i < themeRoomsCheck.Count; i++)
        {
            if (rnd == themeRoomsCheck[i])
            {
                inList = true;
            }
        }

        if (inList)
        {
            themeRoomsCheck.Remove(rnd);

            int rnd2 = Random.Range(0, themeRooms[rnd].Rooms.Count);
            var instance = Instantiate(themeRooms[rnd].Rooms[rnd2], roomSpawnpoints[x].transform.position,
                roomSpawnpoints[x].transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
        }
        else
        {
            SpawnThemeRoom(x);
        }
    }

    void SpawnNormalRoom(int x)
    {
        int rnd = Random.Range(0, normalRooms.Count);

        bool inList = false;
        if (normalRoomsCheck.Count <= 0)
        {
            for (int i = 0; i < normalRooms.Count; i++)
            {
                normalRoomsCheck.Add(i);
            }
        }


        for (int i = 0; i < normalRoomsCheck.Count; i++)
        {
            if (rnd == normalRoomsCheck[i])
            {
                inList = true;
            }
        }

        if (inList)
        {
            normalRoomsCheck.Remove(rnd);

            int rnd2 = Random.Range(0, normalRooms[rnd].Rooms.Count);
            var instance = Instantiate(normalRooms[rnd].Rooms[rnd2], roomSpawnpoints[x].transform.position,
                roomSpawnpoints[x].transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
        }
        else
        {
            SpawnNormalRoom(x);
        }
    }

    void SpawnVault()
    {
        int rnd = Random.Range(0, roomSpawnpoints.Count);
        int roomType = roomSpawnpoints[rnd].GetComponent<SpawnInfo>().spawnType;
        if (roomType == 1)
        {
            SpawnVault();
        }
        else
        {
            var instance = Instantiate(vaultRoom, roomSpawnpoints[rnd].transform.position, roomSpawnpoints[rnd].transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);

            roomSpawnpoints.Remove(roomSpawnpoints[rnd]);
        }
    }

    void SpawnPrison()
    {
        int rnd = Random.Range(0, roomSpawnpoints.Count);
        int roomType = roomSpawnpoints[rnd].GetComponent<SpawnInfo>().spawnType;
        if (roomType == 0)
        {
            SpawnPrison();
        }
        else
        {
            var instance = Instantiate(prison, roomSpawnpoints[rnd].transform.position, roomSpawnpoints[rnd].transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);

            roomSpawnpoints.Remove(roomSpawnpoints[rnd]);
        }
    }
}

[Serializable]
public class RoomTypes
{
    public List<GameObject> Rooms = new();
}