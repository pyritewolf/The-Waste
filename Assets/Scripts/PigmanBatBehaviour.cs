using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PigmanBatBehaviour : MonoBehaviour {
    
    private Animator anim;
    private NavMeshAgent nma;
    private int currentWaypoint;
    private Vector3 previousPosition;
    private bool isIdle;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private List<Transform> waypoints;
    [SerializeField]
    private float chaseSpeed;
    [SerializeField]
    private float patrolSpeed;
    [SerializeField]
    private float patrolDelay;

    [SerializeField]
    private float hearingRange;


    // Use this for initialization
    void Awake()
    {
        nma = GetComponent<NavMeshAgent>();
        nma.SetDestination(waypoints[currentWaypoint].position);
        nma.Move(nma.desiredVelocity);
        anim = GetComponent<Animator>();
        isIdle = false;
    }


    // Update is called once per frame
    void Update () {

        LookAround();
        if (anim.GetBool("isAggressive"))
        {
            ChaseTarget();
        } else {
            Patrol();
        }
        
        VerifySpeed();
    }

    void LookAround()
    {
        //Here the enemy tries to pick up the player with his piggy senses
        //Some basic variables that the pig will need
        float distanceToTarget = (target.position - transform.position).magnitude;
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        //Piggy hearing - the pig listens in a circular radius
        if (distanceToTarget <= hearingRange) {
            anim.SetBool("isAggressive", true);
        }

        //Piggy eyesight - this piggy can't see much, so he relies on hearing!

    }


    void ChaseTarget()
    {

        nma.SetDestination(target.position);
        int tooClose = -1;
        if (nma.remainingDistance > nma.stoppingDistance) {
            tooClose = 1;
        } else
        {
            AttackTarget();
        }
        nma.speed = chaseSpeed;
        Vector3 mov = Vector3.Normalize(target.position - transform.position) * nma.speed * Time.deltaTime * tooClose;
        //nma.Move(mov);
    }

    void AttackTarget()
    {
        //TODO; attack logic should go here but i'm too sleepy, sorry not sorry
        // zzzzleepy pigs
    }

    void Patrol()
    {
        nma.speed = patrolSpeed;

        if (nma.remainingDistance <= nma.stoppingDistance && !isIdle)
        {
            nma.Move(Vector3.zero);
            nma.velocity = Vector3.zero;
            nma.destination = transform.position;
            nma.Stop();
            nma.ResetPath();
            //nma.enabled = false;
            Invoke("NextWaypoint", patrolDelay);
            isIdle = true;
        }
    }

    void VerifySpeed()
    {
        Vector3 curMove = transform.position - previousPosition;
        float curSpeed = (curMove.magnitude / Time.deltaTime) / nma.speed;
        previousPosition = transform.position;
        anim.SetFloat("speed", curSpeed);
    }

    void NextWaypoint()
    {
        //nma.enabled = true;
        currentWaypoint++;
        if (!nma.pathPending && currentWaypoint >= waypoints.Count)
            currentWaypoint = 0;

        nma.destination = waypoints[currentWaypoint].position;
        nma.Resume();
        isIdle = false;
    }
}
