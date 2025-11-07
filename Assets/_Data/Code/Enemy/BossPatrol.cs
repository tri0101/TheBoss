using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 20f;
    public float minWaitTime = 6f;
    public float maxWaitTime = 10f;
    public float obstacleCheckDistance = 2f;

    [Header("Player Detection")]
    public float detectRadius = 3f;
    public float stopDistanceFromPlayer = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject player;

    private bool isWaiting = false;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        agent.stoppingDistance = stopDistanceFromPlayer;

        GoToRandomPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectRadius)
        {
            isChasing = true;
            FollowPlayer();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                GoToRandomPoint();
            }

            Patrol();

            if (!isWaiting && !agent.pathPending && IsObstacleAhead())
            {
                StopAllCoroutines();
                GoToRandomPoint();
            }
        }
    }

    void FollowPlayer()
    {
        StopAllCoroutines();
        isWaiting = false;

        animator.SetBool("IsWalking", true);

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > agent.stoppingDistance + 0.2f)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);

            Vector3 lookAtPos = player.transform.position;
            lookAtPos.y = transform.position.y;
            transform.LookAt(lookAtPos);
        }
    }

    void Patrol()
    {
        if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.3f)
        {
            StartCoroutine(WaitAndGoRandom());
        }
    }

    IEnumerator WaitAndGoRandom()
    {
        isWaiting = true;
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);

        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        GoToRandomPoint();
        isWaiting = false;
    }

    void GoToRandomPoint()
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * patrolRadius;
        randomDir.y = 0; // Giữ mặt phẳng
        randomDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            agent.isStopped = false;
            animator.SetBool("IsWalking", true);
        }
    }

    bool IsObstacleAhead()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 rayDir = transform.forward;

        Ray ray = new Ray(rayOrigin, rayDir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, obstacleCheckDistance))
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + transform.forward * obstacleCheckDistance);
    }
}
