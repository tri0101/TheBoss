using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("👁️ Tầm nhìn")]
    public float radius = 20f;     // bán kính tầm nhìn
    [Range(0, 360)] public float angle = 90f; // góc nhìn
    [SerializeField] LayerMask targetMask;   // layer Player
    [SerializeField] LayerMask obstructionMask; // layer tường, chướng ngại vật
    [SerializeField] private Transform doorCheck;


    [Header("👂 Tầm nghe")]
    public float hearingRadius = 10f; // bán kính tầm nghe (âm thanh)
    public float hearingThreshold = 5f; // độ nhạy nghe (có thể tinh chỉnh)
    public Transform player; // layer tường, chướng ngại vật
    public bool cannSeePlayer;
    public bool canHearPlayer;
    private EnemyPatrolNav epn;


    //private void Start()
    //{
    //    StartCoroutine(FOVRoutine());
    //}
    //private IEnumerator FOVRoutine()
    //{
    //    WaitForSeconds wait = new WaitForSeconds(0.2f);

    //    while (true)
    //    {
    //        yield return wait;
    //        CanSeePlayer();
    //    }
    //}
    private void Awake()
    {
        epn = transform.GetComponent<EnemyPatrolNav>();
    }


    //public bool CanSeePlayer()
    //{
    //    if (player == null) return false;

    //    // 📍 Dời vị trí mắt của enemy và player lên cao 1.6f (pivot ở chân)
    //    Vector3 enemyEyePos = transform.position + new Vector3(0f, 1.6f, 0f);
    //    Vector3 playerEyePos = player.position + new Vector3(0f, 1.6f, 0f);

    //    // 🟢 1. Kiểm tra player có trong bán kính tầm nhìn không
    //    Collider[] rangeChecks = Physics.OverlapSphere(enemyEyePos, radius, targetMask);
    //    if (rangeChecks.Length == 0)
    //        return false;

    //    Transform target = rangeChecks[0].transform;
    //    Vector3 directionToTarget = (playerEyePos - enemyEyePos).normalized;

    //    // 🟡 2. Kiểm tra góc nhìn (FOV)
    //    if (Vector3.Angle(transform.forward, directionToTarget) > angle / 2)
    //        return false;

    //    // 🔵 3. Raycast kiểm tra vật cản (tường, vật thể)
    //    float distanceToTarget = Vector3.Distance(enemyEyePos, playerEyePos);
    //    if (Physics.Raycast(enemyEyePos, directionToTarget, distanceToTarget, obstructionMask))
    //    {
    //        cannSeePlayer = false;
    //        return false;

    //    }


    //    // ✅ 4. Nếu qua được hết các bước → thấy player
    //    cannSeePlayer = true;
    //    return true;

    //}

    //public bool CanSeePlayer()
    //{
    //    if (player == null) return false;

    //    // 📍 Dời vị trí mắt của enemy và player lên cao 1.6f (pivot ở chân)
    //    Vector3 enemyEyePos = transform.position + new Vector3(0f, 1.6f, 0f);
    //    Vector3 playerEyePos = player.position + new Vector3(0f, 1.6f, 0f);

    //    // 🟢 0. Kiểm tra khoảng cách gần (2D - trên mặt phẳng XZ)
    //    // Tạo Vector2 chỉ chứa X và Z của vị trí Enemy và Player
    //    Vector2 enemyPosXZ = new Vector2(transform.position.x, transform.position.z);
    //    Vector2 playerPosXZ = new Vector2(player.position.x, player.position.z);

    //    // Tính khoảng cách 2D
    //    float flatDistance = Vector2.Distance(enemyPosXZ, playerPosXZ);
    //    float closeRangeThreshold = 2.0f; // Bán kính 2 mét

    //    if (flatDistance <= closeRangeThreshold)
    //    {
    //        if (doorCheck.transform.localPosition.x == 0) return false;
    //        // Nếu Player ở quá gần (trong vòng 2m trên mặt phẳng XZ)
    //        cannSeePlayer = true;
    //        return true;
    //    }
    //    // --- Kết thúc kiểm tra khoảng cách gần ---


    //    // 🟢 1. Kiểm tra player có trong bán kính tầm nhìn tổng thể không
    //    Collider[] rangeChecks = Physics.OverlapSphere(enemyEyePos, radius, targetMask);
    //    if (rangeChecks.Length == 0)
    //        return false;

    //    Transform target = rangeChecks[0].transform;
    //    Vector3 directionToTarget = (playerEyePos - enemyEyePos).normalized;

    //    // 🟡 2. Kiểm tra góc nhìn (FOV)
    //    if (Vector3.Angle(transform.forward, directionToTarget) > angle / 2)
    //        return false;

    //    // 🔵 3. Raycast kiểm tra vật cản (tường, vật thể)
    //    float distanceToTarget = Vector3.Distance(enemyEyePos, playerEyePos);
    //    if (Physics.Raycast(enemyEyePos, directionToTarget, distanceToTarget, obstructionMask))
    //    {
    //        cannSeePlayer = false;
    //        return false;

    //    }


    //    // ✅ 4. Nếu qua được hết các bước → thấy player
    //    cannSeePlayer = true;
    //    return true;
    //}
    public bool CanSeePlayer()
    {
        if (player == null) return false;

        // Vị trí mắt Enemy và Player
        Vector3 enemyEyePos = transform.position + new Vector3(0f, 1.6f, 0f);
        Vector3 playerEyePos = player.position + new Vector3(0f, 1.6f, 0f);

        // Dùng lại direction + distance cho tất cả
        Vector3 directionToPlayer = (playerEyePos - enemyEyePos).normalized;
        float distanceToPlayer = Vector3.Distance(enemyEyePos, playerEyePos);

        // ===============================
        // 0. KIỂM TRA KHOẢNG CÁCH GẦN (<= 2 mét XZ)
        // ===============================
        Vector2 enemyPosXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPosXZ = new Vector2(player.position.x, player.position.z);
        float flatDistance = Vector2.Distance(enemyPosXZ, playerPosXZ);

        if (flatDistance <= 2f)
        {
            // Nếu cửa đóng → không thể nhìn xuyên
            if (doorCheck.transform.localPosition.x == 0)
                return false;

            // Check tường trong cự ly gần
            if (Physics.Raycast(enemyEyePos, directionToPlayer, distanceToPlayer, obstructionMask))
            {
                cannSeePlayer = false;
                return false;
            }

            // Không bị che → thấy Player
            cannSeePlayer = true;
            return true;
        }

        // ===============================
        // 1. Kiểm tra trong bán kính tầm nhìn
        // ===============================
        Collider[] rangeChecks = Physics.OverlapSphere(enemyEyePos, radius, targetMask);
        if (rangeChecks.Length == 0)
            return false;

        // ===============================
        // 2. Kiểm tra trong góc nhìn (FOV)
        // ===============================
        if (Vector3.Angle(transform.forward, directionToPlayer) > angle / 2f)
            return false;

        // ===============================
        // 3. Raycast kiểm tra tường ở tầm xa
        // ===============================
        if (Physics.Raycast(enemyEyePos, directionToPlayer, distanceToPlayer, obstructionMask))
        {
            cannSeePlayer = false;
            return false;
        }

        // ===============================
        // 4. Qua hết tất cả → thấy Player
        // ===============================
        cannSeePlayer = true;
        return true;
    }

    public void CanHearSound(Transform soundSource)
    {
        if (soundSource == null) return;

        Vector3 enemyPos = transform.position + Vector3.up * 1.6f;
        float distance = Vector3.Distance(enemyPos, soundSource.position);

        if (distance <= hearingRadius)
        {
            if (!epn.IsChasing)
            {
                AudioClip ad = AudioManager.instance.whatthenoise;
                epn.Speaking(ad, transform.position, 2, 15, 1f);
            }
            
            // 🟢 Gọi xử lý khi nghe thấy âm thanh
            epn.SetPriorityPoint(soundSource);
            //Debug.Log($"{name} nghe thấy âm thanh từ {soundSource.name}");
        }
    }
    public void AutoHearSound(Transform soundSource)
    {
        if (soundSource == null) return;
        //AudioClip ad = AudioManager.instance.whocoming;
        //epn.Speaking(ad, transform.position, 2, 15, 0.5f);
        epn.SetPriorityPoint(soundSource);
         
    }

    //private void OnDrawGizmos()
    //{
    //    // vị trí "mắt"
    //    Vector3 enemyEyePos = transform.position + Vector3.up * 1.6f;

    //    // bán kính + biên góc
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(enemyEyePos, radius);

    //    Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2f);
    //    Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, angle / 2f);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(enemyEyePos, enemyEyePos + viewAngle01 * radius);
    //    Gizmos.DrawLine(enemyEyePos, enemyEyePos + viewAngle02 * radius);

    //    // Nếu thấy player → vẽ đường xanh
    //    if (cannSeePlayer && player != null)
    //    {
    //        Vector3 playerEyePos = player.position + Vector3.up * 1.6f;
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawLine(enemyEyePos, playerEyePos);
    //    }
    //}

    //// helper giống Editor.DirectionFromAngle
    //private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    //{
    //    float rad = (eulerY + angleInDegrees) * Mathf.Deg2Rad;
    //    return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    //}
    private void OnDrawGizmos()
    {
        Vector3 enemyEyePos = transform.position + Vector3.up * 1.6f;

        // 👁️ Vẽ tầm nhìn
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(enemyEyePos, radius);

        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2f);
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, angle / 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(enemyEyePos, enemyEyePos + viewAngle01 * radius);
        Gizmos.DrawLine(enemyEyePos, enemyEyePos + viewAngle02 * radius);

        if (cannSeePlayer && player != null)
        {
            Vector3 playerEyePos = player.position + Vector3.up * 1.6f;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(enemyEyePos, playerEyePos);
        }

        // 👂 Vẽ tầm nghe (vòng tròn màu cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }

    // Helper
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        float rad = (eulerY + angleInDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}
