using System.Collections;
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

    public Transform bulletSpawn;

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
        StopAgent();
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(FovScan());
        StartCoroutine(StateMachineBehaviour());
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
                yield return new WaitUntil(() =>
                {
                    Chase(target);
                    return playerFound == null || Tired() || CanAttackTarget(target);
                });
                if (IsServer) StopCoroutine(ConsumeStamina());
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
        agent.SetDestination(PickRandomNavmeshLocation(radiusToPickRandomLocation));
        agent.speed = patrolSpeed;
        anim.SetBool("walk", true);
        anim.SetBool("run", false);
    }

    void Chase(Transform target)
    {
        agent.speed = chaseSpeed;
        anim.SetBool("run", true);
        anim.SetBool("walk", false);
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
        transform.LookAt(target.position);
        bulletSpawn.LookAt(target.position);
        anim.SetTrigger("shoot");
        yield return new WaitForSeconds(1f);

        if (IsServer)
        {
            tazerSound.PlayAudioClientRpc();
            InstantiateBulletRpc();
        }

        yield return new WaitForSeconds(2f);
        shooting = false;
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
            bulletRb.AddForce(bullet.transform.forward * 10f, ForceMode.Impulse);
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
        anim.SetBool("tired", true);
        yield return new WaitForSeconds(3f);
        if (IsServer) stamina.Value = 100f;
        anim.SetBool("tired", false);
    }

    #endregion
}