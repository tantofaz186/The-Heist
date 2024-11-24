using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FOV))]
public class Anubis : NetworkBehaviour
{
    public NavMeshAgent agent;
    public AudioPlay attackSound;
    public Animator anim;
    public bool gameStarted = true;

    [SerializeField]
    bool walkPointSet;

    public float attackRange;

    private bool shooting;
    public GameObject playerFound;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 3.5f;
    public FOV fov;
    public float radiusToPickRandomLocation = 10f;
    

    [SerializeField] private Transform playerPos;

    [SerializeField] Vector3 patrolLocation;

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
        if (IsServer)
        {
            StopAgent();
            GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(FovScan());
            StartCoroutine(StateMachineBehaviour());
        }
        else
            enabled = false;
    }

    private IEnumerator FovScan()
    {
        while (gameStarted)
        {
            fov.Scan();
            playerFound = fov.Objects.Count > 0 ? FindPlayer(fov) : null;

            yield return new WaitForSeconds(fov.scanInterval);
        }
    }

    private IEnumerator StateMachineBehaviour()
    {
        while (gameStarted)
        {
            
            yield return new WaitUntil(() => Arrived() || playerFound != null);
            if (playerFound)
            {
                Transform target = playerFound.transform;
               
                yield return new WaitUntil(() =>
                {
                    Chase(target);
                    return playerFound == null  || CanAttackTarget(target);
                });
               
                if (CanAttackTarget(target))
                {
                    shooting = true;
                    Attack(target);
                    yield return new WaitUntil(AttackEnd);
                }
            }
            else
            {
                Patrol();
            }
        }
    }

    #region State Machine

    void Patrol()
    {
        patrolLocation = PickRandomNavmeshLocation(radiusToPickRandomLocation);
        agent.SetDestination(patrolLocation);
        agent.speed = patrolSpeed;
        SetAnimationWalkRpc();
    }

    void Chase(Transform target)
    {
        agent.speed = chaseSpeed;
        SetAnimationRunRpc();
        agent.SetDestination(target.position);
    }

    void Attack(Transform targetTransform)
    {
        StopAgent();
        StartCoroutine(SlowAttack(targetTransform));
    }

    IEnumerator SlowAttack(Transform target)
    {
        StopAgent();
        transform.LookAt(target);
        playerPos = target;
        SetAnimationAttackRpc();
        yield return new WaitForSeconds(1f);
        
        if (IsServer)
        {
            attackSound.PlayAudioClientRpc();
            InstantiateBulletRpc();
        }

        yield return new WaitForSeconds(3f);
        shooting = false;
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("SafeZone"))
        {
            ChangePatolLocation(PickRandomNavmeshLocation(radiusToPickRandomLocation));
        }
    }

    bool CanAttackTarget(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= attackRange && !shooting;
    }

    bool AttackEnd()
    {
        return !shooting;
    }

    
    
    public void ChangePatolLocation(Vector3 newPatrolLocation)
    {
        fov.ClearObjects();
        patrolLocation = newPatrolLocation;
        agent.SetDestination(patrolLocation);
    }

    [Rpc(SendTo.Server)]
    void InstantiateBulletRpc() 
    {
        GameObject slowArea = BulletPool.instance.GetSlowArea();
        if (slowArea != null)
        {
            slowArea.transform.position = playerPos.position;
            slowArea.SetActive(true);
        }
    }

    #endregion

    #region Navmesh

    private Vector3 PickRandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    bool Arrived()
    {
        return agent.remainingDistance <= 1f;
    }

    void StopAgent()
    {
        agent.SetDestination(transform.position);
        agent.velocity = Vector3.zero;
    }

    #endregion

    

    #region Animations

    
    public void SetAnimationAttackRpc()
    {
        anim.SetTrigger("shoot");
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAnimationRunRpc()
    {
        anim.SetBool("run", true);
        anim.SetBool("walk", false);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAnimationWalkRpc()
    {
        anim.SetBool("walk", true);
        anim.SetBool("run", false);
    }

    #endregion
}