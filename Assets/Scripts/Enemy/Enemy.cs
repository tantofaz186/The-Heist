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

    public NetworkVariable<float> stamina = new NetworkVariable<float>(100f);
    public float staminaSpeed;
    
    private bool isRunning;
   [SerializeField] private bool isRegenerating;

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
            
            if (isRunning)
            {
                ConsumeStamina();
            }
            yield return new WaitUntil(()=>!isRegenerating);
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

    void ConsumeStamina()
    {
        stamina.Value -= staminaSpeed * Time.deltaTime;
        if (stamina.Value <= 0)
        {
            
            StartCoroutine(RegenerateStamina());
        }
    }

    IEnumerator RegenerateStamina()
    {   isRegenerating = true;
        agent.SetDestination(transform.position);
        anim.SetBool("tired", true);
        yield return new WaitForSeconds(3f);
        stamina.Value = 100f;
        anim.SetBool("tired", false);
        isRegenerating = false;
        
        
    }
    
    
    
    public void CheckLocation(Vector3 target)
    {
        walkPointSet = true;
        walkPoint = target;
    }

    void Patrol()
    {   if (shooting)return;
        isRunning = false;
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
        isRunning = true;
        agent.SetDestination(_playerPos.transform.position);
        agent.speed = chaseSpeed;
        anim.SetBool("run", true);
        anim.SetBool("walk", false);
    }

    void Attack(Transform targetTransform)
    {   isRunning = false;
        Transform position = targetTransform;
        agent.SetDestination(transform.position);
        transform.LookAt(position);
        Vector3 direction = position.transform.position - bulletSpawn.transform.position;
        direction.Normalize();
        float angleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        bulletSpawn.transform.rotation = Quaternion.Euler(0, angleY, 0);
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
       yield return new WaitForSeconds(2.5f);
       shooting = false;
       
    }

    [Rpc(SendTo.Server)]
    public void InstantiateBulletRpc()
    {  Debug.Log("Shoot");
        
        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        var bulletNetworkObject = bullet.GetComponent<NetworkObject>();
        bulletNetworkObject.SpawnWithOwnership(OwnerClientId);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
        
    }

  
    
    List<GameObject> GetWaypoints()
    {
        return GameObject.FindGameObjectsWithTag("Waypoints").ToList();
      
    }
}