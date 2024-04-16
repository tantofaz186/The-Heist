using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Enemy : NetworkBehaviour
{
    public NavMeshAgent agent;
   
    public Animator anim;
    public Rigidbody rb;

    public Transform playerPos;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    public float timeAttack;
    private bool attacked;


    public float sightRange, attackRange;
    public bool playerInSight, playerInAttack;


    public float radius;
    public float angle;
    
    
    
    
    private void Awake()
    {
       
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

   

   
   
    
    List<GameObject> GetPlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player").ToList();
      
    }

    
    void Update()
    {
        Collider[] col=  Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        if (col.Length > 0)
        {
            Attack(col[0].transform);
            return;
        }
        Collider[] col2=  Physics.OverlapSphere(transform.position, sightRange, whatIsPlayer);
        if (col2.Length > 0)
        {
            Chase(col2[0].transform);
            return;
        }
        
        Patrol();
        
        
        
        
            
        
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

    void Chase(Transform playerPos)
    {
        agent.SetDestination(playerPos.position);
        agent.speed = 3f;
        anim.SetBool("run",true);
        anim.SetBool("walk",false);
        
    }

    void Attack(Transform playerPos)
    {
        agent.SetDestination(playerPos.position);
        transform.LookAt(playerPos.position);
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
