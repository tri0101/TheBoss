using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class WolfController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    public Transform cameraMainPlayer;
    public Transform cameraHeadPlayer;
    private Transform meatTarget;
    private PoisonMeatTest currentMeatScript;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private Animator animator;

    private bool isDead = false;
    private bool waitingForDeath = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isRunning = false;
    private enum WolfState { Patrol, Chase, Eat }
    private WolfState currentState = WolfState.Patrol;

    [Header("Distance Settings")]
    [SerializeField] private float stopDistancePlayer = 0.5f;
    [SerializeField] private float stopDistanceMeat = 0.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Scan Settings")]
    [SerializeField] private float scanInterval = 0.5f;
    private float scanTimer = 0f;

    [Header("Speed Settings")]
    [SerializeField] private float normalSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private bool isAttacked = false;
    [SerializeField] private AudioSource audioS;
    [SerializeField] private LayerMask groundLayer; // gán trong Inspector
    private float footstepTimer;
    private const float footstepInterval = 0.75f;
    private const float runstepInterval = 0.5f;
 
    private bool isEatingProcess = false;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.speed = normalSpeed;
    }

    private void Update()
    {
        if (isDead || isAttacked) return;

        // quét định kỳ
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            FindMeatTarget();
            scanTimer = 0f;
        }

        bool playerInNav = player != null && IsInNavMesh(player.position, true);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Nếu đang ăn (Idle_New (1)) thì không được chase
        bool isEatingAnim = stateInfo.IsName("Idle_New (1)");

        // ✅ Ưu tiên đuổi Player trước, chỉ ăn thịt khi player đã ra khỏi vùng NavMesh
        if (playerInNav && !isEatingAnim)
        {
            Debug.Log("player muc tieu");
            SetState(WolfState.Chase);
            HandleChase();
        }
        else if (meatTarget != null)
        {
            SetState(WolfState.Eat);
            HandleMeat();
        }
        else
        {
            SetState(WolfState.Patrol);
            HandlePatrol();
        }
        isRunning = (currentState == WolfState.Chase);
        agent.speed = isRunning ? runSpeed : normalSpeed;
        HandleRotation();
        isMoving = agent.velocity.sqrMagnitude > 0.1f && !agent.isStopped;
        if (isMoving)
        {
            float currentStepInterval = isRunning ? runstepInterval : footstepInterval;
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= currentStepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }

    }
    public void PlayFootstepSound()
    {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float rayLength = 0.5f; // chĩa xuống 1 mét

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength, groundLayer))
        {
            int layer = hit.collider.gameObject.layer;

            if (layer == LayerMask.NameToLayer("Grass") || layer == LayerMask.NameToLayer("GardenBake"))
            {
                //Debug.Log("da chay nhac");
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.walkOnGrass, transform.position);
            }
        }
    }
    private void SetState(WolfState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (newState == WolfState.Patrol)
            agent.ResetPath();
    }

    private void HandleMeat()
    {
        if (meatTarget == null) return;

        Vector3 adjustedTarget = meatTarget.position;
        agent.stoppingDistance = stopDistanceMeat;
        agent.SetDestination(adjustedTarget);

        float distanceToMeat = Vector3.Distance(transform.position, adjustedTarget);
        if (distanceToMeat <= stopDistanceMeat + 0.1f)
        {
            if (!isEatingProcess)
            {
                StartCoroutine(EatNormalMeat());
            }

            animator.SetBool("isEating", true);
            animator.SetBool("isRunning", false);
            agent.isStopped = true;
            agent.speed = normalSpeed;

            if (currentMeatScript != null && currentMeatScript.GetPoisoned() && !waitingForDeath)
            {
                waitingForDeath = true;
                Invoke(nameof(Die), 10f);
            }
        }
        else
        {
            animator.SetBool("isEating", false);
            agent.isStopped = false;
        }
    }

    private IEnumerator EatNormalMeat()
    {
        isEatingProcess = true;
        animator.SetBool("isEating", true);

        float eatDuration = 10f;
        float timer = 0f;

        while (timer < eatDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (meatTarget != null)
        {
            Destroy(meatTarget.gameObject);
            meatTarget = null;
            currentMeatScript = null;
        }

        isEatingProcess = false;
        animator.SetBool("isEating", false);
        agent.isStopped = false;
        SetState(WolfState.Patrol);
    }
    private void HandleChase()
    {
        animator.SetBool("isEating", false);
        animator.SetBool("isRunning", true);

        if (!isAttacked)
        {
            agent.isStopped = false;
            agent.speed = runSpeed;
        }

        //GoToTarget(player, stopDistancePlayer);
        GoToTarget(player, 4f);
    }

    private void HandlePatrol()
    {
        Patrol();
        animator.SetBool("isEating", false);
        animator.SetBool("isRunning", false);
        agent.isStopped = false;
        agent.speed = normalSpeed;
    }

    private void GoToTarget(Transform target, float stopDistance)
    {
        if (agent == null) return;

        agent.stoppingDistance = stopDistance;
        agent.SetDestination(target.position);

        if (target == player)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= stopDistance + 0.1f)
            {
                animator.SetTrigger("isPrepare");
                animator.SetBool("isRunning", false);
                isAttacked = true;
                PlayerController pc = player.GetComponent<PlayerController>();
                
                if (pc != null) pc.enabled = false;

                agent.isStopped = true;
                agent.speed = 0f;
                agent.angularSpeed = 0f;
                StartCoroutine(PrepareAndAttack(pc));
                //RotateToPlayer(player.transform, 1f);
                //StartCoroutine(pc.RotateToEnemy(transform, 1f));
                ////dừng 5 giây trước khi attack
                //StartCoroutine(Attack(pc));
                //StartCoroutine(ResumeAfterAttack());
            }
        }
    }
    private IEnumerator PrepareAndAttack(PlayerController pc)
    {
      
        // 2. Cho player quay mặt lại
        yield return StartCoroutine(pc.RotateToEnemy(transform, 0.1f));


        yield return StartCoroutine(RotateToPlayer(player.transform, 0.1f));

        //// 3. Dừng 5 giây trước khi tấn công
        //yield return new WaitForSeconds(0.5f);

        // 4. Bắt đầu attack
        yield return StartCoroutine(Attack(pc));

        // 5. Sau khi tấn công xong
        yield return StartCoroutine(ResumeAfterAttack());
    }

    private IEnumerator RotateToTarget(Transform target, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
    }
    public IEnumerator RotateToPlayer(Transform player, float duration)
    {
        Quaternion startRot = transform.rotation;

        // hướng về player, nhưng giữ y = 0 để chỉ xoay ngang
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.rotation = targetRot;
    }
    private IEnumerator Attack(PlayerController pc)
    {
        AudioManager.instance.StopTensionMusicSmooth(0.5f);
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger("Attack");
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.dogSound, transform.position, 2f, 15f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        pc.SetDie();
    }

    private IEnumerator ResumeAfterAttack()
    {
        yield return new WaitForSeconds(5f);

        if (!isDead)
        {
            isAttacked = false;
            agent.isStopped = false;
            agent.speed = normalSpeed;
            agent.angularSpeed = 50f;
            SetState(WolfState.Patrol);
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0 || agent == null) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.stoppingDistance = 0f;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void HandleRotation()
    {
        if (isDead) return;

        Vector3 direction = Vector3.zero;

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            direction = agent.velocity.normalized;
        }
        else if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 targetDir = agent.steeringTarget - transform.position;
            targetDir.y = 0;
            if (targetDir.magnitude > 0.1f)
                direction = targetDir.normalized;
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    private bool IsInNavMesh(Vector3 position, bool checkHeightDiff)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 1f, agent.areaMask))
        {
            float heightDiff = Mathf.Abs(position.y - hit.position.y);
            if (checkHeightDiff)
                return heightDiff > 0.6f; // player còn trên mặt navmesh
            return heightDiff < 0.1f;    // meat thì sát mặt navmesh hơn
        }
        return false;
    }

    private void FindMeatTarget()
    {
        GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp")
            .Concat(GameObject.FindGameObjectsWithTag("isEating"))
            .ToArray();

        var meats = pickUps
            .Where(go => go.name.Contains("Meat") && IsInNavMesh(go.transform.position, false))
            .OrderBy(go => Vector3.Distance(transform.position, go.transform.position))
            .ToArray();

        if (meats.Length > 0)
        {
            meatTarget = meats[0].transform;
            currentMeatScript = meatTarget.GetComponent<PoisonMeatTest>();
        }
        else
        {
            meatTarget = null;
            currentMeatScript = null;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        waitingForDeath = false;
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isEating", false);
        animator.SetTrigger("Dead");
        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();

        pond.ShowMessage(3f, "The wolf has been tranquilized ");
    }
    public void SetTrueWaitingForDead()
    {
        waitingForDeath = true;
    }
    public bool getDead()
    {
        return isDead;
    }
    public void CallAfterDie(Transform collision)
    {
        //PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();

        //pond.ShowMessage(3f, "The wolf has been tranquilized ");
        PickUpConfig dartConfig = collision.transform.GetComponent<PickUpConfig>();
        if (dartConfig != null)
        {
            dartConfig.ChangeNameObject("Used tranquilizer dart");
            collision.transform.name = "Used tranquilizer dart";
            Destroy(dartConfig.gameObject, 0.5f);

        }
    }

}
