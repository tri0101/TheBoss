using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarRun : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraCar;
    [SerializeField] private Transform carConvex;
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private Transform canvasWin;
    [SerializeField] private Transform theBoss;

    private Rigidbody rb;
    private Vector3 startPos;
    private bool isRunning = false;
    public bool isStartRun = false;
    private bool doorOpened = false; // cờ để không xoay nhiều lần
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 🔹 Bỏ qua va chạm giữa xe và cửa
        if (leftDoor != null)
        {
            Collider[] carCols = GetComponentsInChildren<Collider>();
            Collider[] leftCols = leftDoor.GetComponentsInChildren<Collider>();
            foreach (var c1 in carCols)
                foreach (var c2 in leftCols)
                    Physics.IgnoreCollision(c1, c2, true);
        }

        if (rightDoor != null)
        {
            Collider[] carCols = GetComponentsInChildren<Collider>();
            Collider[] rightCols = rightDoor.GetComponentsInChildren<Collider>();
            foreach (var c1 in carCols)
                foreach (var c2 in rightCols)
                    Physics.IgnoreCollision(c1, c2, true);
        }
    }

    public void Run()
    {
        if (theBoss != null)
        {
            Collider[] carCols = GetComponentsInChildren<Collider>();
            Collider[] bossCols = theBoss.GetComponentsInChildren<Collider>();

            foreach (var c1 in carCols)
                foreach (var c2 in bossCols)
                    Physics.IgnoreCollision(c1, c2, true);

            Debug.Log("✅ Đã bỏ qua va chạm giữa xe và theBoss");
        }
        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();
        pond.SetClearText();
        pond.SetFalseTab();
        player.gameObject.SetActive(false);
        cameraCar.gameObject.SetActive(true);

        // 🔹 Ép convex
        if (carConvex != null)
        {
            MeshCollider mc = carConvex.GetComponent<MeshCollider>();
            if (mc != null)
            {
                mc.convex = true;
                Debug.Log("✅ Đã bật Convex cho carConvex");
            }
        }

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        startPos = transform.position;

        GameObject gb = AudioManager.instance.PlaySFXAtPositionObject(AudioManager.instance.carRunning, transform.position);
        gb.transform.SetParent(transform);

        StartCoroutine(StartCarAfterDelay(4f));
    }

    private IEnumerator StartCarAfterDelay(float delay)
    {
        Debug.Log("🚗 Đang khởi động xe... (chờ " + delay + " giây)");
        yield return new WaitForSeconds(delay);
        isRunning = true;
        Debug.Log("🏁 Xe bắt đầu chạy!");
    }

    private void Update()
    {
        if (isStartRun)
        {
            Run();
            isStartRun = false;
        }

        // 🔹 Kiểm tra khi đến vị trí mở cửa
        if (!doorOpened && transform.localPosition.x >= 22.8f)
        {
            doorOpened = true;
            if (leftDoor != null && rightDoor != null)
                StartCoroutine(OpenDoors());
        }
        

    }

    private void FixedUpdate()
    {
        if (!isRunning || rb == null) return;

        float distance = Mathf.Abs(transform.position.x - startPos.x);
        if (distance >= 50f)
        {
            
            isRunning = false;
            rb.linearVelocity = Vector3.zero;
            WinGame();
            
            return;
        }

        rb.MovePosition(rb.position + Vector3.right * speed * Time.fixedDeltaTime);
    }
    public void WinGame()
    {
        //StartCoroutine(GameOverCoroutine());
        
        if (canvasWin != null)
        {
            canvasWin.gameObject.SetActive(true);
            Transform parentCanvas = canvasWin.parent.parent;
            Transform parentCanvasTrue = canvasWin.parent.parent.parent;
            canvasWin.parent.SetParent(parentCanvasTrue);
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(parentCanvasTrue, false);
            parentCanvas.gameObject.SetActive(false);
        }
            
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //private IEnumerator GameOverCoroutine()
    //{
    //    // Hiển thị canvas Over
    //    if (canvasWin != null)
    //        canvasWin.gameObject.SetActive(true);

    //    // Chờ 3 giây
    //    yield return new WaitForSeconds(3f);

    //    // Ẩn canvas Over
    //    if (canvasWin != null)
    //        canvasWin.gameObject.SetActive(false);
    //    LoadingScene.instance.LoadMenuScene();

    //}

    private IEnumerator OpenDoors()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Quaternion startLeft = leftDoor.localRotation;
        Quaternion endLeft = Quaternion.Euler(leftDoor.localEulerAngles.x, -90f, leftDoor.localEulerAngles.z);

        Quaternion startRight = rightDoor.localRotation;
        Quaternion endRight = Quaternion.Euler(rightDoor.localEulerAngles.x, 90f, rightDoor.localEulerAngles.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            leftDoor.localRotation = Quaternion.Slerp(startLeft, endLeft, t);
            rightDoor.localRotation = Quaternion.Slerp(startRight, endRight, t);
            yield return null;
        }

        leftDoor.localRotation = endLeft;
        rightDoor.localRotation = endRight;
    }
    public bool IsBossInRange()
    {
        if (theBoss == null) return false;
        if (isBossDead()) return false;
        // Lấy vị trí 2D của xe và boss, bỏ qua trục y
        Vector2 carPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 bossPos2D = new Vector2(theBoss.position.x, theBoss.position.z);

        // Tính khoảng cách
        float distance = Vector2.Distance(carPos2D, bossPos2D);

        // Kiểm tra nếu nhỏ hơn hoặc bằng 5 mét
        return distance <= 10f;
    }
    public bool isBossDead()
    {
        EnemyPatrolNav epn = theBoss.GetComponent<EnemyPatrolNav>();
        if(epn = null)
        {
            return epn.IsDead;
        }
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        if (theBoss == null) return;

        // Màu cho vòng tròn
        Gizmos.color = Color.red;

        // Vẽ vòng tròn bán kính 5m quanh xe (trên mặt phẳng XZ)
        Vector3 center = transform.position;
        center.y = theBoss.position.y; // đặt cùng y của boss để dễ nhìn

        // Vẽ 50 đoạn nối thành vòng tròn
        int segments = 50;
        float radius = 10f;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // Tùy chọn: vẽ đường nối đến boss nếu boss nằm trong vòng
        Vector2 carPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 bossPos2D = new Vector2(theBoss.position.x, theBoss.position.z);
        if (Vector2.Distance(carPos2D, bossPos2D) <= radius)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, theBoss.position);
        }
    }
}
