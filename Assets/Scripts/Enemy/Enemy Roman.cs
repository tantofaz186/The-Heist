using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FOV))]
public class EnemyRoman : NetworkBehaviour
{
    public NavMeshAgent agent;
    public AudioPlay attackSound;
    public AudioPlay chargeSound;
    public Animator anim;
    public bool gameStarted = true;

    [SerializeField]
    bool walkPointSet;

    public float attackRange;
    public float collideRange;

    private bool shooting;
    public GameObject playerFound;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float chargeSpeed = 20f;
    public FOV fov;
    public float radiusToPickRandomLocation = 10f;
    public NetworkVariable<float> stamina = new NetworkVariable<float>(100f);
    public float staminaSpeed;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Collider hitCollider;

    [SerializeField]
    Vector3 patrolLocation;

    private List<GameObject> waypoints;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        fov = GetComponent<FOV>();
        rb = GetComponent<Rigidbody>();
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
            GetComponent<Rigidbody>().isKinematic = false;
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
                yield return new WaitUntil(() =>
                {   
                    
                    Chase(target);
                    return playerFound == null || Tired() || CanAttackTarget(target);
                });
                if (IsServer) StopCoroutine(ConsumeStamina());
                yield return new WaitUntil(() =>
                {   
                    Debug.Log("StartAttack");
                    shooting = true;
                    Attack(target);
                    return playerFound == null || AttackEnd() || Tired();
                });
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
        agent.speed = chargeSpeed;
        SetAnimationRunRpc();
        agent.SetDestination(target.position);
    }

    void Attack(Transform targetTransform)
    {   
       
        agent.speed = chargeSpeed;
        SetAnimationDashRpc();
        agent.SetDestination(targetTransform.position);
    }
    
    public void EndAttack(Collider other, float force)
    {
        Vector3 direction = transform.forward * force;
        rb.AddForce(-direction, ForceMode.Impulse);
        other.GetComponent<Movement>().Stun(direction);
        agent.ResetPath();
        stamina.Value = 0f;
        shooting = false;
    }

    bool CanAttackTarget(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= attackRange ;
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

    #endregion

    #region Navmesh

    public List<GameObject> GetWaypoints()
    {
        return GameObject.FindGameObjectsWithTag("Waypoints").ToList();
    }

    private Vector3 PickRandomNavmeshLocation()
    {
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
        agent.speed = 0;
        SetAnimationTiredTrueRpc();
        yield return new WaitForSeconds(3f);
        agent.speed = patrolSpeed;
        if (IsServer) stamina.Value = 100f;
        SetAnimationTiredFalseRpc();
        Patrol();
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
        anim.SetBool("attack", false);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAnimationDashRpc()
    {
        anim.SetBool("attack", true);
        anim.SetBool("run", false);
        anim.SetBool("walk", false);
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