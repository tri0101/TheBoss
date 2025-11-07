using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolNav : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public Transform cameraMainPlayer;
    public Transform cameraHeadPlayer;
    [SerializeField] private Transform currentTarget;
    private Rigidbody[] _ragdollRigidbodies;
    private Collider[] _ragdollColliders;
    [SerializeField] private int currentIndex;
    [SerializeField] private int patrolCounter = 0;
    [SerializeField] private Transform head;
    private bool isMoving = false;

    private NavMeshAgent agent;
    private Animator animator;
    [SerializeField]  Collider mainCollider;
    private float footstepTimer;
    private const float footstepInterval = 0.75f; // 1 giây
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform garageDoor;
    [SerializeField] private Transform floor3Door;
    [SerializeField] private Transform sleepRoomDoor;
    [SerializeField] private Transform balconyDoor;

    [SerializeField] private Transform patrolGarage;
    [SerializeField] private Transform patrolFloor3;
    [SerializeField] private Transform patrolSleep;
    [SerializeField] private Transform patrolDefault;
    [SerializeField] private Transform patrolBalcony;
    private FieldOfView fov;

    [SerializeField] private Transform priorityPosition;
    private Coroutine recoverCoroutine;
    private List<Transform> points = new List<Transform>();

    // cờ kiểm tra đã load chưa
    private bool garageLoaded = false;
    private bool floor3Loaded = false;
    private bool basementLoaded = false;

    private bool isWaiting = false;
    private bool isChasing = false;
    public bool IsChasing => isChasing;
    private bool isDead = false;
    private bool isGoToPriority = false;
    public bool IsDead => isDead;
    private bool justWokeUp = false;
    private bool isLookingAround = false;
    private bool shouldLookNextIdle = false;

    private float timeSinceLastSeen = Mathf.Infinity;
    private float memoryDuration = 5f;

    [SerializeField] private bool hasUpperCutTriggered = false; // ✅ cờ chặn UpperCut nhiều lần

    private bool AgentReady => agent != null && agent.enabled && agent.isOnNavMesh;

    [SerializeField] private bool hasSpottedPlayer = false; // 🔹 Đánh dấu đã gọi log “Đã thấy player”

    [SerializeField] private bool isSpeaking = false;
    private void Awake()
    {
        // chỉ load mặc định
        if (patrolDefault != null)
        {
            foreach (Transform child in patrolDefault)
            {
                points.Add(child);
            }
        }

        patrolPoints = points.ToArray();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = 0.3f;
        }
    }

   private void LoadPointCode()
    {
        // check garageDoor
        if (!garageLoaded && garageDoor != null)
        {
            opencloseDoor script = garageDoor.GetComponent<opencloseDoor>();
            if (script != null && script.isCode)
            {
                LoadPatrolGroup(patrolGarage);
                garageLoaded = true;
            }
        }

        // check floor3Door
        if (!floor3Loaded && floor3Door != null)
        {
            opencloseDoor script = floor3Door.GetComponent<opencloseDoor>();
            if (script != null && script.isCode)
            {
                LoadPatrolGroup(patrolFloor3);
                floor3Loaded = true;
            }
        }

        // check basementDoor
        if (!basementLoaded && sleepRoomDoor != null)
        {
            opencloseDoor script = sleepRoomDoor.GetComponent<opencloseDoor>();
            if (script != null && script.isCode)
            {
                LoadPatrolGroup(patrolSleep);
                basementLoaded = true;
            }
        }
        if (!basementLoaded && balconyDoor != null)
        {
            opencloseDoor script = balconyDoor.GetComponent<opencloseDoor>();
            if (script != null && script.isCode)
            {
                LoadPatrolGroup(patrolBalcony);
                basementLoaded = true;
            }
        }
    }
    private void LoadPatrolGroup(Transform patrolGroup)
    {
        if (patrolGroup == null) return;

        foreach (Transform child in patrolGroup)
        {
            points.Add(child);
        }

        patrolPoints = points.ToArray();
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        fov = transform.GetComponent<FieldOfView>();

       
           
            mainCollider = GetComponent<Collider>();
        
            _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        _ragdollColliders = GetComponentsInChildren<Collider>();
        //.Where(c => c != GetComponent<Collider>())
        //.ToArray();

        EnsureOnNavMesh(5f);

        DisableRagdoll();
        //// ✅ Nếu có vị trí ưu tiên → đi tới đó trước
        //if (priorityPosition != null)
        //{
        //    GoToPriorityPosition();
        //}
        //else
        //{
        //    GoToNextPoint();
        //}
        GoToNextPoint();
    }
    private void GoToPriorityPosition()
    {
        if (!AgentReady || priorityPosition == null) return;

        agent.isStopped = false;
        agent.SetDestination(priorityPosition.position);
        SetWalking();

        // Sau khi đến nơi → quay lại patrol bình thường
        StartCoroutine(WaitAndThenPatrol(2f));
    }

    private IEnumerator WaitAndThenPatrol(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Xoá priority sau khi tới nơi để không bị lặp lại
        priorityPosition = null;

        if (!isDead && AgentReady)
        {
            GoToNextPoint();
        }
    }
    private void SetLooking()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsLooking", true);
        }
        isLookingAround = true;
    }

    private void SetWalking()
    {
        if (animator != null)
        {
            animator.SetBool("IsLooking", false);
            animator.SetBool("IsWalking", true);
        }
        isLookingAround = false;
    }

    private void SetIdle()
    {
        if (animator != null)
        {
            animator.SetBool("IsLooking", false);
            animator.SetBool("IsWalking", false);
        }
        isLookingAround = false;
    }

    void Update()
    {
        //Debug.Log("Priority = " + (priorityPosition != null ? priorityPosition.name : "null"));
        //LoadPointCode();
        if (isDead || hasUpperCutTriggered) return;
        if (animator != null)
        {
            isMoving = animator.GetBool("IsWalking");
        }
        if (isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f; // Đặt lại bộ đếm
            }
        }
        if (priorityPosition != null && !isChasing)
        {
            HandlePriority();
            HandleJustWokeUp();
            HandleLookingAround();
            HandleChasePlayer();
            HandlePatrolMovement();
            DetectAndOpenDoor();
            return;
        }
        HandleJustWokeUp();
        HandleLookingAround();
        HandleChasePlayer();
        HandlePatrolMovement();
        DetectAndOpenDoor();
    }
    public void PlayFootstepSound()
    {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float rayLength = 0.25f; // ✅ chỉ bắn xuống 0.5 mét

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength, groundLayer))
        {
            int layer = hit.collider.gameObject.layer;

            if (layer == LayerMask.NameToLayer("WoodFloor"))
            {
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.walkOnWood, transform.position);
            }
            else if (layer == LayerMask.NameToLayer("Grass") || layer == LayerMask.NameToLayer("GardenBake"))
            {
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.walkOnGrass, transform.position);
            }
        }
    }
    public void SetPriorityPoint(Transform pp)
    {
        priorityPosition = pp;
    }
    private void HandlePriority()
    {
        //if (isGoToPriority) return;
        //agent.isStopped = true;
        //agent.ResetPath();
        //agent.velocity = Vector3.zero;

        //agent.SetDestination(priorityPosition.position);
        //agent.isStopped = false;
        //isGoToPriority = true;

        if (isGoToPriority) return;

        // 🛑 Dừng agent trước khi tính toán
       

        // 🧭 Kiểm tra xem có đường đi hợp lệ không
        //NavMeshPath path = new NavMeshPath();
        //if (NavMesh.CalculatePath(transform.position, priorityPosition.position, NavMesh.AllAreas, path)
        //    && path.status == NavMeshPathStatus.PathComplete)
        //{
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            // ✅ Có đường đi → di chuyển
            agent.SetDestination(priorityPosition.position);
            agent.isStopped = false;
            isGoToPriority = true;
        //}
        //else
        //{
        //    isGoToPriority = false;
        //    priorityPosition = null;
        //}
    }

    private void HandleJustWokeUp()
    {
        if (!justWokeUp || animator == null) return;

        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);

        if (fov.CanSeePlayer())
        {
            justWokeUp = false;
            //AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.angryManSound, player.transform.position);
            if (!hasSpottedPlayer)
            {
                hasSpottedPlayer = true;
                AudioManager.instance.CallTensionMusic(AudioManager.instance.tensionMusic, 0.7f);
                //AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.angryManSound, player.transform.position);
                //AudioManager.instance.PlayOldManVoice(player.transform.position, 0.7f);
                AudioClip ranClip = AudioManager.instance.GetAudioOldMan();
                Speaking(ranClip, transform.position, 2, 15f, 1f);
              
            }
          
            SetWalking();
            ChasePlayer();
            return;
        }

        if (st.IsName("Looking") && st.normalizedTime >= 1f)
        {
            justWokeUp = false;
            SetWalking();
            GoToNextPoint();
        }
    }

    private void HandleLookingAround()
    {
        if (!isLookingAround || animator == null) return;

        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);

        if (st.IsName("Looking") && st.normalizedTime >= 1f)
        {
            patrolCounter = 0;
            shouldLookNextIdle = false;
            SetWalking();
            GoToNextPoint();
        }
    }

   

    private void HandleChasePlayer()
    {
        if (player == null) return;

        bool canSee = fov.CanSeePlayer();

        if (canSee)
        {
            // 🟢 Nếu mới thấy lần đầu (chưa từng thấy trước đó)
            if (!hasSpottedPlayer)
            {
                hasSpottedPlayer = true;
                AudioManager.instance.CallTensionMusic(AudioManager.instance.tensionMusic, 0.7f);
                //AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.angryManSound, player.transform.position);
                //AudioManager.instance.PlayOldManVoice(player.transform.position, 0.7f);
                AudioClip ranClip = AudioManager.instance.GetAudioOldMan();
                Speaking(ranClip, transform.position, 2, 15f, 1f);
               
            }

            // Tiếp tục hành vi đuổi theo
            ChasePlayer();
            timeSinceLastSeen = 0f;
            patrolCounter = 0;
            shouldLookNextIdle = false;
        }
        else if (isChasing)
        {
            // 🔵 Không còn thấy player nhưng vẫn trong thời gian nhớ
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen > memoryDuration)
            {
                // 🔴 Quên vị trí player → reset trạng thái
                isChasing = false;
                currentTarget = null;
                GoToNextPoint();

                // Khi đã quên hoàn toàn → cho phép gọi lại khi thấy lần sau
                hasSpottedPlayer = false;
                AudioManager.instance.StopTensionMusicSmooth(1f);
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    private void HandlePatrolMovement()
    {
        if (!isChasing && AgentReady && !agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
        {
            if (priorityPosition != null)
            {
                isGoToPriority = false;
                priorityPosition = null;
            }
            StartCoroutine(WaitBeforeNextPoint(3f));
        }

        if (animator != null && !isChasing && !isLookingAround)
        {
            animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
        }
    }

    void GoToNextPoint()
    {
        if (isDead || patrolPoints == null || patrolPoints.Length == 0 || !AgentReady) return;

       
        // Nếu không có priority thì mới patrol
        agent.SetDestination(patrolPoints[currentIndex].position);

        currentIndex = (currentIndex + 1) % patrolPoints.Length;
        patrolCounter++;

        if (patrolCounter >= 5)
        {
            shouldLookNextIdle = true;
        }
    }
 
    void GoToRandomOtherPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || !AgentReady) return;

        int newIndex = currentIndex;

        // random cho tới khi khác currentIndex
        while (newIndex == currentIndex && patrolPoints.Length > 1)
        {
            newIndex = Random.Range(0, patrolPoints.Length);
        }

        currentIndex = newIndex;
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentIndex].position);
        patrolCounter = 0;
        shouldLookNextIdle = false;
        SetWalking();

        Debug.Log($"🚷 Enemy bị chặn → đi sang patrol point khác: {currentIndex}");
    }
    IEnumerator WaitBeforeNextPoint(float waitTime)
    {
        isWaiting = true;

        if (AgentReady)
        {
            agent.isStopped = true;
            agent.ResetPath();

            if (shouldLookNextIdle)
            {
                SetLooking();
            }
            else
            {
                SetIdle();
            }
        }

        yield return new WaitForSeconds(waitTime);

        if (!isDead && AgentReady && !isLookingAround)
        {
            GoToNextPoint();
            agent.isStopped = false;
            SetWalking();
        }

        isWaiting = false;
    }

    void ChasePlayer()
    {
        if (!AgentReady || player == null) return;

        isChasing = true;
        currentTarget = player;

        Vector3 direction = player.position - transform.position;
        //direction.y = 0;
        float distance = direction.magnitude;
        float attackDistance = 2f;

        if (distance > attackDistance)
        {
            // 🟢 Enemy di chuyển về phía player (cao hơn 1.6f)
            Vector3 targetPos = player.position + Vector3.up * 1.6f;
            Vector3 stopPos = targetPos - direction.normalized * attackDistance;
            //stopPos.y = transform.position.y; // Giữ nguyên độ cao hiện tại của enemy nếu muốn di chuyển ngang mặt đất

            agent.isStopped = false;
            agent.speed = 0.3f;
            agent.SetDestination(targetPos);

            SetWalking();
        }
        else
        {
          
            // 🔴 Enemy vào phạm vi tấn công → đứng im
            agent.isStopped = true;
            agent.speed = 0f;
            agent.angularSpeed = 0;
            SetIdle();

            if (!hasUpperCutTriggered)
            {
                hasUpperCutTriggered = true;
                AudioManager.instance.StopTensionMusicSmooth(0.5f);
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null) pc.enabled = false;

                if (animator != null)
                {
                    // Player xoay mặt về phía Enemy trong 1 giây
                    StartCoroutine(pc.RotateToEnemy(head, 0.5f));
                    StartCoroutine(RotateToPlayer(player.transform, 0.5f));
                    // Enemy delay 2 giây trước khi UpperCut
                    StartCoroutine(DoUpperCut(pc));
                }
            }
        }
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
    private IEnumerator DoUpperCut(PlayerController pc)
    {
        //yield return new WaitForSeconds(1f);

        // UpperCut animation
        animator.SetTrigger("UpperCut");
        yield return new WaitForSeconds(1f);
        // Player chết
        pc.SetDie();

        // Enemy quay lại Idle
        yield return StartCoroutine(UpperCutToIdle());
    }
    public void CallPunchSound()
    {
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.punchSound, transform.position,2f,15f, 0.2f);
    }

    private IEnumerator UpperCutToIdle()
    {
        yield return new WaitForSeconds(1f);

        SetIdle();

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
        }
        currentTarget = null;

        yield return new WaitForSeconds(5f);

        if (agent != null && agent.enabled)
        {
            agent.Warp(new Vector3(-10.12f, 4.1f, 11.08f));
        }
        transform.rotation = Quaternion.Euler(0f, -155f, 0f);

        // 🔑 Reset trạng thái chase
        isChasing = false;
        currentTarget = null;
        timeSinceLastSeen = Mathf.Infinity;
        hasUpperCutTriggered = false;

        // ✅ Đảm bảo agent đã Warp xong trước khi đi patrol
        yield return null; // 1 frame delay để NavMeshAgent cập nhật
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = 0.3f;
            agent.angularSpeed = 180000;
            GoToNextPoint();
            SetWalking();
        }
        hasSpottedPlayer = false;
    }


    //bool CanSeePlayer()
    //{
    //    //if (player == null) return false;

    //    //Vector3 localCenter = new Vector3(0f, 2f, 5f);
    //    //Vector3 worldCenter = transform.position + transform.rotation * localCenter;
    //    //Vector3 halfExtents = new Vector3(5f, 2.5f, 5f);

    //    //Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, transform.rotation);
    //    //foreach (var hit in hits)
    //    //{
    //    //    if (hit.transform == player)
    //    //    {
    //    //        Vector3 eyePos = transform.position + Vector3.up * 1.5f;
    //    //        Vector3 targetPos = player.position + Vector3.up * 1.0f;
    //    //        Vector3 dir = targetPos - eyePos;

    //    //        if (Physics.Raycast(eyePos, dir.normalized, out RaycastHit rayHit, dir.magnitude))
    //    //        {
    //    //            if (rayHit.transform == player)
    //    //                return true;
    //    //        }
    //    //    }
    //    //}
    //    //return false;
    //    if (player == null) return false;

    //    // 🟢 1. Kiểm tra player có trong bán kính tầm nhìn không
    //    Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
    //    if (rangeChecks.Length == 0)
    //        return false;

    //    Transform target = rangeChecks[0].transform;
    //    Vector3 directionToTarget = (target.position - transform.position).normalized;

    //    // 🟡 2. Kiểm tra góc nhìn (FOV)
    //    if (Vector3.Angle(transform.forward, directionToTarget) > angle / 2)
    //        return false;

    //    // 🔵 3. Raycast kiểm tra vật cản (tường, vật thể)
    //    float distanceToTarget = Vector3.Distance(transform.position, target.position);
    //    if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
    //        return false;

    //    // ✅ 4. Nếu qua được hết các bước → thấy player
    //    return true;
    //}
    void DetectAndOpenDoor()
    {
        if (isDead || !AgentReady) return;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 dir = transform.forward;

        if (Physics.Raycast(origin, dir, out hit, 3f))
        {
            // 🚪 Cửa thường
            if (hit.collider.name.StartsWith("_Door"))
            {
                opencloseDoor doorScript = hit.collider.GetComponent<opencloseDoor>();
                if (doorScript != null)
                {
                    //if (doorScript.isCode)
                    //{
                        if (!doorScript.open)
                        {
                            doorScript.OpenBoss();
                            StartCoroutine(WaitAndThenMove(1f, false));
                            StartCoroutine(AutoCloseDoor(doorScript, 2f)); // 🔒 đóng sau 2s
                        }
                    //}
                    //else
                    //{
                    //    StartCoroutine(WaitAndThenMove(1f, true));
                    //}
                }
            }

            // 🚪 Cửa trượt
            if (hit.collider.name.StartsWith("_SlidingDoor"))
            {
                SlidingDoor doorScript = hit.collider.GetComponent<SlidingDoor>();
                if (doorScript != null)
                {
                    doorScript.OpenBoss();
                    StartCoroutine(WaitAndThenMove(2f, false));
                    StartCoroutine(AutoCloseDoor(doorScript, 4f)); // 🔒 đóng sau 2s
                }
            }
        }
    }

    private IEnumerator AutoCloseDoor(opencloseDoor door, float delay)
    {
        yield return new WaitForSeconds(delay);
        door.CloseBoss(); // 👈 gọi hàm đóng
    }

    private IEnumerator AutoCloseDoor(SlidingDoor door, float delay)
    {
        yield return new WaitForSeconds(delay);
        door.CloseBoss(); // 👈 gọi hàm đóng
    }

    private IEnumerator WaitAndThenMove(float waitTime, bool goRandom)
    {
        if (!AgentReady) yield break;

        // 🚫 Dừng di chuyển
        agent.isStopped = true;
        SetIdle();

        yield return new WaitForSeconds(waitTime);

        if (!isDead && AgentReady)
        {
            agent.isStopped = false;
            if (goRandom)
            {
                GoToRandomOtherPoint();  // ❌ không mở được cửa → đổi patrol point
            }
            else
            {
                SetWalking();            // ✅ mở được → đi tiếp
            }
        }
    }

    public void DisableRagdoll()
    {
        mainCollider.isTrigger = false;
        foreach (var rb in _ragdollRigidbodies)
        {
            if (rb != GetComponent<Rigidbody>())
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }
        foreach (var col in _ragdollColliders)
        {
            if (col != mainCollider)
                col.enabled = false;
        }

        if (animator != null) animator.enabled = true;
        if (agent != null)
        {
            agent.enabled = true;
            EnsureOnNavMesh(2f);
        }
        if (mainCollider != null) mainCollider.enabled = true;

        isDead = false;
    }

    public void EnableRagdoll()
    {
        isDead = true;
        mainCollider.isTrigger = true;

        StopAllCoroutines();

        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            agent.enabled = false;
        }

        if (animator != null) animator.enabled = false;

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;         // reset vận tốc
            rb.angularVelocity = Vector3.zero;  // reset quay
        }

        foreach (var col in _ragdollColliders)
        {
           //if(col != mainCollider) col.enabled = false;
            col.enabled = true;
        }

        if (recoverCoroutine != null)
        {
            StopCoroutine(recoverCoroutine);
        }
        //recoverCoroutine = StartCoroutine(RecoverFromRagdoll(60f));
        recoverCoroutine = StartCoroutine(RecoverFromRagdoll(60f));
    }

    private IEnumerator RecoverFromRagdoll(float delay)
    {
        yield return new WaitForSeconds(delay);

        DisableRagdoll();

        if (animator != null)
        {
            SetLooking();
            justWokeUp = true;
        }

        recoverCoroutine = null; // ✅ reset sau khi xong
    }
    public void ForceRecoverNow()
    {
        if (recoverCoroutine != null)
        {
            StopCoroutine(recoverCoroutine);
            recoverCoroutine = null;
        }

        DisableRagdoll();
        if (animator != null)
        {
            SetLooking();
            justWokeUp = true;
        }
    }
    private bool EnsureOnNavMesh(float maxDistance)
    {
        if (agent == null || !agent.enabled) return false;
        if (agent.isOnNavMesh) return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, maxDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            return agent.isOnNavMesh;
        }
        return false;
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Vector3 localCenter = new Vector3(0f, 2f, 5f);
    //    Vector3 worldCenter = transform.position + transform.rotation * localCenter;
    //    Vector3 boxSize = new Vector3(10f, 5f, 10f);

    //    Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
    //    Gizmos.DrawWireCube(Vector3.zero, boxSize);
    //}
   
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        if (collision != null && collision.transform.name == "Tranquilizer dart")

        {
            DartControl dc = collision.transform.GetComponent<DartControl>();
            if (!dc.GetShot()) return;
            isChasing = false;
            hasSpottedPlayer = false;
            AudioManager.instance.StopTensionMusicSmooth(0.5f);
            EnableRagdoll();
            PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();

            pond.ShowMessage(3f, "The boss has been tranquilized for 60 seconds") ;
            PickUpConfig dartConfig = collision.transform.GetComponent<PickUpConfig>();
            if (dartConfig != null)
            {
                dartConfig.ChangeNameObject("Used tranquilizer dart");
                collision.transform.name = "Used tranquilizer dart";
                
                Destroy(dartConfig.gameObject, 0.5f);

            }
        }
    }

    //private void OnTriggerEnter(Collider collision)
    //{
    //    if (isDead) return;
    //    if (collision != null && collision.transform.name == "Dart")
    //    {


    //        EnableRagdoll();
    //        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();
    //        pond.ShowMessage(5f, "The boss has been tranquilized for 45 seconds");
    //        PickUpConfig dartConfig = collision.transform.GetComponent<PickUpConfig>();
    //        if (dartConfig != null)
    //        {
    //            dartConfig.ChangeNameObject("Used Dart");
    //            collision.transform.name = "Used Dart";
    //        }
    //    }
    //}

    public void Speaking(AudioClip audio, Vector3 pos, float minDist = 2f, float maxDist = 15f, float volume = 1f)
    {
        if (audio == null || isSpeaking || isDead || hasUpperCutTriggered) return;

        isSpeaking = true;

        // Gọi AudioManager để phát âm thanh tại vị trí đó
        AudioManager.instance.PlaySFXAtPosition(audio, pos, minDist, maxDist, volume);

        // Sau 3 giây cho phép nói lại
        StartCoroutine(ResetSpeakingAfterDelay(1.5f));
    }

    private IEnumerator ResetSpeakingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isSpeaking = false;
    }



}
