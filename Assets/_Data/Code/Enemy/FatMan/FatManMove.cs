using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FatManMove : MonoBehaviour
{
    public Transform[] patrolPoints;
    [SerializeField] private Transform player;
    public float stopDistanceFromPlayer = 3f;
    public float detectionRadius = 10f;

    private NavMeshAgent agent;
    private Animator animator;

    private enum State { Idle, Walking }
    private State currentState = State.Walking;

    private Transform currentTarget;
    private bool isWaiting = false;
    private float chaseTimer = 0f;
    private float chaseDuration = 5f; // thời gian vẫn chase khi player ra ngoài

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GoToRandomPoint();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer <= detectionRadius;

        // Chasing logic
        if (playerInRange)
        {
            currentState = State.Walking;
            chaseTimer = chaseDuration; // reset timer
            ChasePlayer();
        }
        else if (chaseTimer > 0f)
        {
            // vẫn đi theo player trong khoảng 5 giây
            currentState = State.Walking;
            ChasePlayer();
            chaseTimer -= Time.deltaTime;
        }
        else
        {
            // Patrol
            if (currentState != State.Walking) return;
            Patrol();
        }
    }

    // ================= Patrol =================
    private void Patrol()
    {
        if (isWaiting) return;

        if (currentTarget == null)
        {
            GoToRandomPoint();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private void GoToRandomPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentTarget = patrolPoints[Random.Range(0, patrolPoints.Length)];
        agent.SetDestination(currentTarget.position);
        SetWalking();
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        currentState = State.Idle;
        SetIdle();
        yield return new WaitForSeconds(5f);
        currentState = State.Walking;
        GoToRandomPoint();
        isWaiting = false;
    }

    // ================= Chase Player =================
    private Vector3 lastStopPos;
    private void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 stopPos = player.position - dir * stopDistanceFromPlayer;

        if (Vector3.Distance(lastStopPos, stopPos) > 0.1f)
        {
            agent.SetDestination(stopPos);
            lastStopPos = stopPos;
        }

        SetWalking();
    }

    // ================= Animation =================
    private void SetWalking()
    {
        animator.SetBool("IsLooking", false);
        animator.SetBool("IsWalking", true);
    }

    private void SetIdle()
    {
        animator.SetBool("IsLooking", false);
        animator.SetBool("IsWalking", false);
    }

    // ================= Debug =================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
