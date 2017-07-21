using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PigmanCudgelBehaviour : MonoBehaviour
{

    private Animator anim;
    private NavMeshAgent nma;
    private int currentWaypoint;
    private Vector3 previousPosition;
    private bool isIdle;
    private int currentAttack;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private List<Transform> waypoints;
    [SerializeField]
    private float patrolSpeed;
    [SerializeField]
    private float patrolDelay;
    [SerializeField]
    private float chaseSpeed;
    [SerializeField]
    private float basicAttackDamage;
    [SerializeField]
    private float strongAttackDamage;
    [SerializeField]
    private int basicAttackNumber;
    
    [SerializeField]
    private float sightRange;
    [SerializeField]
    private float sightAngle;
    [SerializeField]
    private LayerMask sightObstacles;


    // Use this for initialization
    void Awake()
    {
        nma = GetComponent<NavMeshAgent>();
        nma.SetDestination(waypoints[currentWaypoint].position);
        nma.Move(nma.desiredVelocity);
        anim = GetComponent<Animator>();
        isIdle = false;
        currentAttack = 0;
    }


    // Update is called once per frame
    void Update()
    {

        LookAround();
        if (anim.GetBool("isAggressive"))
        {
            ChaseTarget();
        }
        else
        {
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

        //This piggy doesn't have piggy hearing bc he has a hood over his head and can't hear a thing
        // poor piggy

        //Piggy eyesight - the pig looks forward in a cone-shaped space
        if (Vector3.Dot(directionToTarget, transform.forward) > Mathf.Abs(Mathf.Cos(sightAngle * Mathf.PI / 180))
            && distanceToTarget < sightRange)
        {
            //check for obstacles cause this piggy can't see through walls
            bool hit = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, sightObstacles);
            if (!hit)
            {
                anim.SetBool("isAggressive", true);
            }
        }

    }


    void ChaseTarget()
    {

        nma.SetDestination(target.position);
        int tooClose = -1;
        if (nma.remainingDistance > nma.stoppingDistance)
        {
            tooClose = 1;
        } else
        {
            AttackTarget();
        }
        nma.speed = chaseSpeed;
        Vector3 mov = Vector3.Normalize(target.position - transform.position) * nma.speed * Time.deltaTime * tooClose;
        //nma.Move(mov);
    }


    //TODO; this attack logic will be performed EVERY FRAME that the player is within hitting distance
    // check out how to fit this with the animation's timing
    void AttackTarget()
    {
        //Here the enemy inflicts damage to the player with his piggy weapon
        //Some basic variables that the pig will need
        float distanceToTarget = (target.position - transform.position).magnitude;
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        //get that goddamned pig-slaying player
        RaycastHit targetGO;
        bool hit = Physics.Raycast(transform.position, directionToTarget, out targetGO, distanceToTarget, sightObstacles);
        if (hit)
        {
            currentAttack++;
            float currentAttackDamage;
            //if the piggy did the basic attack too many times, do the heavy attack
            if(currentAttack >= basicAttackNumber)
            {
                anim.SetBool("isAttackingStrong", true);
                currentAttackDamage = strongAttackDamage;
                currentAttack = 0;
            }
            //otherwise do the basic attack
            else
            {
                anim.SetBool("isAttackingStrong", true);
                currentAttackDamage = basicAttackDamage;
            }

            //perform the actual attack
            Attributes targetLife = targetGO.collider.gameObject.GetComponent<Attributes>();
            targetLife.RecieveDamage(currentAttackDamage);
        }
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