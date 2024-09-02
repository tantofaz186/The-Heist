using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FOV))]
public class Enemy : NetworkBehaviour
{
    public NavMeshAgent agent;

    public Animator anim;
    public bool gameStarted = true;
    public Vector3 walkPoint;
    [SerializeField] bool walkPointSet;
    public float attackRange;
    public float timeAttack;
    private bool shooting = false;
   // public event Action<GameObject> OnAttack;
    private float tempSpeed;
    public GameObject playerFound;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public List<GameObject> waypoints = new List<GameObject>();
    public FOV fov;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        fov = GetComponent<FOV>();
        waypoints = GetWaypoints();
    }

    GameObject FindPlayer(FOV sensor)
    {
        return sensor.Objects.First();
    }

    private void Start()
    {
        StartCoroutine(FovScan());
        tempSpeed = agent.speed;
    }

    private IEnumerator FovScan()
    {
        while (gameStarted)
        {
            fov.Scan();
            if (fov.Objects.Count > 0)
            {
                playerFound = FindPlayer(fov);
                if (playerFound)
                {
                    
                    if (Vector3.Distance(transform.position, playerFound.transform.position) < attackRange)
                    {   
                        Attack(playerFound.transform);
                    }
                    else
                    {
                        Chase(playerFound.transform);
                    }
                }
            }
            else
            {
                Patrol();
            }
            yield return new WaitForSeconds(fov.scanInterval);
        }
    }
    
    
    public void CheckLocation(Vector3 target)
    {
        walkPointSet = true;
        walkPoint = target;
    }

    void Patrol()
    {   if (shooting)return;
        if (!walkPointSet) SearchWalk();
        if (walkPointSet) agent.SetDestination(walkPoint);
        agent.speed = patrolSpeed;
        anim.SetBool("walk", true);
        anim.SetBool("run", false);
        Vector3 distance = transform.position - walkPoint;

        if (distance.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void SearchWalk()
    {   
        int rnd = Random.Range(0, waypoints.Count);;

        
        walkPoint = waypoints[rnd].transform.position;

        if (Physics.Raycast(walkPoint, -transform.up, 2f))
        {
            walkPointSet = true;
        }
    }

    void Chase(Transform _playerPos)
    {   if (shooting)return;
        agent.SetDestination(_playerPos.transform.position);
        agent.speed = chaseSpeed;
        anim.SetBool("run", true);
        anim.SetBool("walk", false);
    }

    void Attack(Transform targetTransform)
    {   
        var position = targetTransform.position;
       
        transform.LookAt(position);
        if (!shooting)
        {  
            shooting = true;
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {   
       anim.SetTrigger("shoot");
       yield return new WaitForSeconds(.5f);
       
       InstantiateBulletRpc();
       yield return new WaitForSeconds(1f);
       shooting = false;
      
       
    }

    [Rpc(SendTo.Server)]
    public void InstantiateBulletRpc()
    {  Debug.Log("Shoot");
        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        var bulletNetworkObject = bullet.GetComponent<NetworkObject>();
        bulletNetworkObject.SpawnWithOwnership(OwnerClientId);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 5f, ForceMode.Impulse);
        
    }

  
    
    List<GameObject> GetWaypoints()
    {
        return GameObject.FindGameObjectsWithTag("Waypoints").ToList();
      
    }
}