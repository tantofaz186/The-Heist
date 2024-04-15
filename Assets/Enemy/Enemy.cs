using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public List<GameObject> players = new();
    public Animator anim;

    

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    public float timeAttack;
    private bool attacked;


    public float sightRange, attackRange;
    public bool playerInSight, playerInAttack;
    
    
    
    private void Awake()
    {
       UpdatePlayerslist();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }
    
    public void UpdatePlayerslist()
    {
        players = GetPlayers();
    }
    
    List<GameObject> GetPlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player").ToList();
      
    }

    
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttack = Physics.CheckSphere(transform.position,attackRange, whatIsPlayer);

        if (!playerInSight&&!playerInAttack)Patrol();
        if (playerInSight&&!playerInAttack)Chase();
        if (playerInSight&&playerInAttack)Attack();
            
        
    }

    void Patrol()
    {
       if(!walkPointSet)SearchWalk();
       if (walkPointSet) agent.SetDestination(walkPoint);
       agent.speed = 1f;
        anim.SetBool("walk",true);
        anim.SetBool("run",false);
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

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomz);

        if (Physics.Raycast(walkPoint, -transform.up, 2f))
        {
            walkPointSet = true;
        }
    }

    void Chase()
    {
        agent.SetDestination(players[0].transform.position);
        agent.speed = 3f;
        anim.SetBool("run",true);
        anim.SetBool("walk",false);
        
    }

    void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(players[0].transform.position);
        if (!attacked)
        {
            anim.SetTrigger("attack");
            attacked = true;
            Invoke(nameof(ResetAttack),timeAttack);
        }
    }

    void ResetAttack()
    {
        attacked = false;
    }
}
