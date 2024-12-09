using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FOV))]
public class Enemy : NetworkBehaviour
{
    public NavMeshAgent agent;
    public AudioPlay tazerSound;
    public Animator anim;
    public bool gameStarted = true;

    [SerializeField]
    bool walkPointSet;

    public float attackRange;

    private bool shooting;
    public GameObject playerFound;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public FOV fov;
    public float radiusToPickRandomLocation = 10f;
    public NetworkVariable<float> stamina = new NetworkVariable<float>(100f);
    public float staminaSpeed;
    public float heightOffset = 1.5f;

    public AudioListPlay screamSound;

    private List<GameObject> waypoints;

    public Transform bulletSpawn;

    [SerializeField]
    Vector3 patrolLocation;

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
        waypoints = GetWaypoints();
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
            if (Tired()) StartCoroutine(RegenerateStamina());
            yield return new WaitWhile(Tired);

            yield return new WaitUntil(() => Arrived() || playerFound != null);
            if (playerFound)
            {
                Transform target = playerFound.transform;
                if (IsServer) StartCoroutine(ConsumeStamina());
                screamSound.PlayAudioClientRpc();
                yield return new WaitUntil(() =>
                {
                    Chase(target);
                    return playerFound == null || Tired() || CanAttackTarget(target);
                });
                if (CanAttackTarget(target))
                {
                    if (IsServer) StopCoroutine(ConsumeStamina());
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
        patrolLocation = PickRandomNavmeshLocation();
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
        StartCoroutine(Shoot(targetTransform));
    }

    IEnumerator Shoot(Transform target)
    {
        StopAgent();
        Vector3 targetPosition = target.position + Vector3.up * heightOffset;
        transform.LookAt(target);
        bulletSpawn.LookAt(targetPosition);
        SetAnimationShootRpc();
        yield return new WaitForSeconds(1f);
        transform.LookAt(target);
        bulletSpawn.LookAt(targetPosition);
        yield return new WaitForSeconds(.5f);
        
        //float time = 1.5f;

       /* do
        {
            time -= Time.deltaTime;
            targetPosition = target.position + Vector3.up * heightOffset;
            transform.LookAt(target);
            bulletSpawn.LookAt(targetPosition);
            yield return null;
        } while (time > 0);*/

        if (IsServer)
        {
            tazerSound.PlayAudioClientRpc();
            InstantiateBulletRpc();
        }

        yield return new WaitForSeconds(2f);
        shooting = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("SafeZone"))
        {
            ChangePatolLocation(PickRandomNavmeshLocation());
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

    bool Tired()
    {
        return stamina.Value <= 0;
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
        GameObject bullet = BulletPool.instance.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = bulletSpawn.position;
            bullet.transform.forward = bulletSpawn.forward;
            bullet.SetActive(true);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = Vector3.zero;
            bulletRb.Sleep();
            bulletRb.AddForce(bullet.transform.forward * 12f, ForceMode.Impulse);
        }
    }

    #endregion

    #region Navmesh

    public List<GameObject> GetWaypoints()
    {
        return GameObject.FindGameObjectsWithTag("Waypoints").ToList();
    }

    private Vector3 PickRandomNavmeshLocation( /*float radius*/)
    {
        /*Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }*/

        int rnd = Random.Range(0, waypoints.Count);

        return waypoints[rnd].transform.position;
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

    #region Stamina

    IEnumerator ConsumeStamina()
    {
        yield return new WaitUntil(() =>
        {
            stamina.Value -= staminaSpeed * Time.deltaTime;
            return stamina.Value <= 0;
        });
    }

    IEnumerator RegenerateStamina()
    {
        StopAgent();
        SetAnimationTiredTrueRpc();
        yield return new WaitForSeconds(3f);
        if (IsServer) stamina.Value = 100f;
        SetAnimationTiredFalseRpc();
    }

    #endregion

    #region Animations

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAnimationTiredTrueRpc()
    {
        anim.SetBool("tired", true);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAnimationTiredFalseRpc()
    {
        anim.SetBool("tired", false);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAnimationShootRpc()
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