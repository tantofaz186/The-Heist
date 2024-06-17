using System;
using System.Collections;
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
    private bool walkPointSet;
    public float walkPointRange;
    public float attackRange;
    public float timeAttack;
    private bool attacked;
   // public event Action<GameObject> OnAttack;
    private float tempSpeed;
    public GameObject playerFound;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    
    public FOV fov;
    
    public GameObject hitCollider;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        fov = GetComponent<FOV>();
    }

    GameObject FindPlayer(FOV sensor)
    {
        return sensor.Objects.First();
    }

    private void Start()
    {
        StartCoroutine(FovScan());
        hitCollider.SetActive(false);
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
    {   
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
        float randomz = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        var position = transform.position;
        walkPoint = new Vector3(position.x + randomX, position.y, position.z + randomz);

        if (Physics.Raycast(walkPoint, -transform.up, 2f))
        {
            walkPointSet = true;
        }
    }

    void Chase(Transform _playerPos)
    {
        agent.SetDestination(_playerPos.transform.position);
        agent.speed = chaseSpeed;
        anim.SetBool("run", true);
        anim.SetBool("walk", false);
    }

    void Attack(Transform targetTransform)
    {   
        var position = targetTransform.position;
        //agent.SetDestination(position);
        transform.LookAt(position);
        if (!attacked)
        {   Invoke(nameof(ColliderActivate), 0.5f);
            anim.SetTrigger("attack");
            agent.speed = 0f;
            attacked = true;
            //OnAttack?.Invoke(targetTransform.gameObject);
            Invoke(nameof(ResetAttack), timeAttack);
        }
    }

    void ColliderActivate()
    {
        hitCollider.SetActive(true);
    }

    void ResetAttack()
    {
        attacked = false;
        hitCollider.SetActive(false);
        agent.speed = tempSpeed;
    }
}