﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class AI : MonoBehaviour
{
    enum State
    {
        Patrolling,
        Chasing,
        Searching,
        Attacking,
        Retreating,
        Listening
    }

    private State enemyState;
    //public Player player;

    public Animator animator;
    public NavMeshAgent enemy;

    public Player player;
    public Transform[] points;

    public TextMeshProUGUI currentStateTxt;

    public int patrolDestinationPoint;
    public int patrolDestinationAmount;
    public int viewDistance = 20;
    public int hearingDistance = 10;
    public int attackDistance = 3;
    public float distance;
    public float chasingTime = 4.0f;
    public float searchTime = 8.0f;

    private Vector3 lastPlayerLocation;
    private Vector3 playerLocation;
    private Vector3 enemyLocation;

    public Ray enemySight;
    public RaycastHit hitInfo;

    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        patrolDestinationPoint = 0;
        SwitchState(State.Patrolling);
    }

    

    void Patrolling()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Searching", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", true);

        playerLocation = player.gameObject.transform.position;
        enemy.SetDestination(points[patrolDestinationPoint].position);

        if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
        {
            if (hitInfo.collider.tag == "Player")
            {
                Debug.Log("triggered!!!!");
                SwitchState(State.Chasing);
            }
        }


    }

    void Chasing()
    {
        animator.SetBool("Running", true);
        animator.SetBool("Searching", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", false);

        enemy.SetDestination(playerLocation);

        if (distance <= attackDistance)
        {
            SwitchState(State.Attacking);
        }

        if (distance >= viewDistance)
        {
            lastPlayerLocation = player.gameObject.transform.position;
            searchTime = 8.0f;
            SwitchState(State.Searching);
        }


    }

    void Searching()
    {

        animator.SetBool("Running", false);
        animator.SetBool("Searching", true);
        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", false);

        enemy.SetDestination(enemyLocation);

        searchTime -= Time.deltaTime;

        if (searchTime <= 0.0f)
        {
            SwitchState(State.Retreating);
        }



    }

    void Listening()
    {
        //transform.LookAt(player.transform);
        enemy.SetDestination(playerLocation);

        if (distance >= hearingDistance)
        {
            SwitchState(State.Chasing);
        }

    }

    void Retreating()
    {
        animator.SetBool("Running", true);
        animator.SetBool("Searching", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", false);

        enemy.SetDestination(points[patrolDestinationPoint].position);

        if ((enemyLocation.x == points[patrolDestinationPoint].position.x) && (enemyLocation.z == points[patrolDestinationPoint].position.z))
        {
            SwitchState(State.Patrolling);
        }
    }

    void Attacking()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Searching", false);
        animator.SetBool("Attacking", true);
        animator.SetBool("Walking", false);

        enemy.SetDestination(enemyLocation);

        if (distance > attackDistance)
        {
            SwitchState(State.Chasing);
        }
    }

    void SwitchState(State newState)
    {
        enemyState = newState;

        switch (enemyState)
        {
            case State.Patrolling:
                Debug.Log("State: Patrolling");
                Patrolling();
                break;

            case State.Chasing:
                Debug.Log("State: Chasing");
                Chasing();
                break;

            case State.Searching:
                Debug.Log("State: Searching");
                Searching();
                break;

            case State.Listening:
                Debug.Log("State: Listening");
                Listening();
                break;

            case State.Retreating:
                Debug.Log("State: Retreating");
                Retreating();
                break;

            case State.Attacking:
                Debug.Log("State: Attacking");
                Attacking();
                break;
        }
    }

    void Update()
    {
        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

        currentStateTxt.text = "Current State: " + enemyState;
        enemyLocation = enemy.transform.position;
        playerLocation = player.gameObject.transform.position;
        distance = Vector3.Distance(playerLocation, enemyLocation);

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.white);
        
       
        /*if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
        {
            if (hitInfo.collider.tag == "Player")
            { 
                Debug.Log("triggered!!!!");
                //chasingTime = 4.0f;
                //chasingTime -= Time.deltaTime;
                SwitchState(State.Chasing);
            }
        }*/
        if (distance <= hearingDistance && player.running == true && distance > attackDistance)
        {
            SwitchState(State.Listening);
        }

        

        /*if (enemyState == State.Chasing)
        {
            SwitchState(State.Chasing);
            if (distance >= viewDistance)
            {
                lastPlayerLocation = player.gameObject.transform.position;
                searchTime = 8.0f;
                SwitchState(State.Searching);
            }
        }*/

        /*if (enemyState == State.Searching)
        {
            searchTime -= Time.deltaTime;

            if (searchTime <= 0.0f)
            {
                SwitchState(State.Retreating);
            }
        }*/

        /*if (enemyState == State.Listening)
        {
            if (distance >= hearingDistance)
            {
                SwitchState(State.Chasing);
            }
        }*/
        

        if ((enemyLocation.x == points[patrolDestinationPoint].position.x) && (enemyLocation.z == points[patrolDestinationPoint].position.z))
        {
            Debug.Log("triggered");

            patrolDestinationPoint = patrolDestinationPoint + 1;
            
            if (patrolDestinationPoint >= patrolDestinationAmount)
            {
                patrolDestinationPoint = 0;
            }

            SwitchState(State.Patrolling);
        }


    }

}
