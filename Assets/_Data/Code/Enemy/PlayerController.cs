using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Animator anim;
    public Transform cameraTransform;
    [SerializeField] Transform cameraMainPlayer;
    [SerializeField] Transform cameraHeadPlayer;
    [SerializeField] Transform holdContainer;
    [SerializeField] Transform canvasDeath;
    [SerializeField] Transform canvasMenu;
    [SerializeField] TextMeshProUGUI textDay;
    [SerializeField] private int dayCount = 1;
    [SerializeField] Transform enemy;
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float jumpForce = 5f;
    public bool isGrounded = false;
    public bool isOnStair = false;
    public bool isOnHighStair = false;
    public float downForceVolume;
    [SerializeField] private bool isMoving;
    [Header("Mouse Look")]
    public float mouseSensitivity = 4f;
    private float cameraPitch = 0f;

    public bool ShowCrosshair = true;
    private Vector3 moveWorld;
    public RaycastHit slopeHit;
    public float currentSpeed;
    [SerializeField] private LayerMask groundLayer; // gán trong Inspector
    private float footstepTimer;
    private const float footstepInterval = 0.5f; // 1 giây
    private Texture2D circleTex;

    private bool hasDied = false;
    public bool HasDied => hasDied;
    public bool isOnObject = false;
    public bool isNotCorrect = false;
    private PlayerObjectNameDisplay pond;
    private FieldOfView fov;


    [SerializeField] OpenLaptop openLaptop;
    //AudioManager audioManager;
    //private void Awake()
    //{
    //    audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    //}
    public bool GetHasDied()
    {
        return this.hasDied;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        circleTex = MakeCircleTexture(16);
        fov = enemy.gameObject.GetComponent<FieldOfView>();
        pond = transform.GetComponent<PlayerObjectNameDisplay>();
        
    }
    Texture2D MakeCircleTexture(int diameter)
    {
        Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.ARGB32, false);
        Color clear = new Color(0, 0, 0, 0); // nền trong suốt

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dx = x - diameter / 2f;
                float dy = y - diameter / 2f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= diameter / 2f)
                    tex.SetPixel(x, y, Color.white); // bên trong hình tròn
                else
                    tex.SetPixel(x, y, clear);       // bên ngoài trong suốt
            }
        }

        tex.Apply();
        return tex;
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    void Update()
    {
        CheckStairByRaycast();
        if (cameraTransform.gameObject.activeSelf)
        {
            HandleMouseLook();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        
    }

    //void HandleMovement()
    //{
    //    float inputX = Input.GetAxis("Horizontal");
    //    float inputZ = Input.GetAxis("Vertical");

    //    Vector3 movement = new Vector3(inputX, 0, inputZ);
    //    if (movement.magnitude > 1f)
    //        movement.Normalize();

    //    anim.SetBool("isMoving", movement != Vector3.zero);

    //    Vector3 moveWorld = transform.TransformDirection(movement);
    //    Vector3 velocity = moveWorld * moveSpeed;

    //    if (isOnStair)
    //        rb.linearVelocity = velocity;
    //    else
    //        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);


    //}
    void HandleMovement()
    {
        rb.useGravity = !isOnStair;
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(inputX, 0f, inputZ);
        if (movement.magnitude > 1f)
            movement.Normalize();
        bool isMoving = movement != Vector3.zero;
        anim.SetBool("isMoving", movement != Vector3.zero);

        if (isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f; // Đặt lại bộ đếm
            }
        }
       
        // Đổi hướng từ local sang thế giới (nếu cần)
        moveWorld = transform.TransformDirection(movement);
        

        //// Thêm lực cho Rigidbody
        //rb.AddForce(moveWorld.normalized * moveSpeed * 10f, ForceMode.Force);
        if (movement != Vector3.zero)
        {
            // Thêm lực khi có input
            rb.AddForce(moveWorld.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            // Khi không có input, lập tức dừng tốc độ theo phương ngang
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
        // Giới hạn tốc độ
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > 5f)
        {
            flatVelocity = flatVelocity.normalized * 5f;
            rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
        }
        Vector3 moveDir = GetSlopeMoveDirection(); // hướng di chuyển trên mặt dốc
        float verticalDot = Vector3.Dot(moveDir.normalized, Vector3.up); // xét xem hướng lên hay xuống
        
        if (isOnStair)
        {
            Vector3 slopeMoveDir = GetSlopeMoveDirection();
            float dirDot = Vector3.Dot(slopeMoveDir.normalized, Vector3.down);

            if (dirDot > 0f)
            {
                
               moveSpeed = 12.75f;
                
                
            }
                
            else
            {
                
               moveSpeed = 20f;
                
            }
                

            rb.AddForce(slopeMoveDir * moveSpeed * 2.5f, ForceMode.Force);
            downForceVolume = 80f;
             rb.AddForce(Vector3.down * downForceVolume, ForceMode.Force);
           
                
        }
        else
        {
            moveSpeed = 20f;
        }


            currentSpeed = rb.linearVelocity.magnitude;
            

    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveWorld,slopeHit.normal ).normalized;
    }
    void CheckStairByRaycast()
    {
        Vector3 origin = transform.position + Vector3.up * 2f;
        Vector3 direction = Vector3.down;
        float rayLength = 3f;

        if (Physics.Raycast(origin, direction, out slopeHit, rayLength))
        {
            if (slopeHit.collider.CompareTag("Stair"))
            {
                isOnStair = true;
                if(slopeHit.collider.name == "Stairs Main HIgh")
                {
                    isOnHighStair = true;
                }
                else
                {
                    isOnHighStair = false;
                }
            }
            else
            {
                isOnStair = false;
                isOnHighStair = false;
            }

            Debug.DrawLine(origin, slopeHit.point, Color.green);
        }
        else
        {
            isOnHighStair = false;
            isOnStair = false;
            Debug.DrawLine(origin, origin + direction * rayLength, Color.red);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }


    public void SetMouseSensitivity(float value)
    {
        float baseSensitivity = 4f; // giống bên PauseMenu
        mouseSensitivity = value * baseSensitivity;
    }

    public float GetCurrentSensitivity()
    {
        return mouseSensitivity;
    }

    void OnGUI()
    {
        if (!ShowCrosshair) return;

        float size = circleTex.width;
        float posX = (Screen.width - size) / 2;
        float posY = (Screen.height - size) / 2;

        if (isOnObject)
        {
            // Vẽ hình tròn trắng
            GUI.DrawTexture(new Rect(posX, posY, size, size), circleTex);
        }
        else if(isNotCorrect){
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.red;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(posX, posY, size, size), "X", style);
        }
        else
        {
            // Vẽ dấu X trắng
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(posX, posY, size, size), "X", style);
        }
    }
    public IEnumerator RotateToEnemy(Transform enemy, float duration)
    {
        if (openLaptop.IsLaptopOpen)
        {
            openLaptop.CloseLaptop();
        }
        // Đổi camera
        cameraMainPlayer.gameObject.SetActive(false);
        cameraHeadPlayer.gameObject.SetActive(true);

        // --- 1. Thân Player chỉ xoay ngang ---
        Quaternion startRotPlayer = transform.rotation;
        Vector3 flatDir = enemy.position - transform.position;
        flatDir.y = 0; // khóa Y
        Quaternion targetRotPlayer = Quaternion.LookRotation(flatDir);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRotPlayer, targetRotPlayer, t);

            // --- 2. Camera xoay full hướng về enemy ---
            Vector3 dirToEnemy = enemy.position - cameraHeadPlayer.position;
            Quaternion targetRotCam = Quaternion.LookRotation(dirToEnemy);
            cameraHeadPlayer.rotation = Quaternion.Slerp(cameraHeadPlayer.rotation, targetRotCam, t);

            yield return null;
        }

        // đảm bảo chính xác 100% khi kết thúc
        transform.rotation = targetRotPlayer;
        cameraHeadPlayer.rotation = Quaternion.LookRotation(enemy.position - cameraHeadPlayer.position);
    }



    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus)
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //        Cursor.visible = false;
    //    }
    //    else
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //    }
    //}
    public void SetDie()
    {
        // Nếu đã xử lý chết rồi thì không làm lại
        if (hasDied) return;
        hasDied = true;

        PauseMenu pm = canvasMenu.GetComponent<PauseMenu>();
        pm.isDisabled = true;

        if (pond.GetReturnDoorKeyBlue())
        {
            pond.SetFalseTab();
        }

        // Tăng ngày chỉ 1 lần
        dayCount++;

        // Nếu đang cầm vật phẩm thì thả ra
        if (holdContainer.childCount > 0)
        {
            PickUpSystem pickUpSystem = GetComponent<PickUpSystem>();
            if (pickUpSystem != null)
            {
                pickUpSystem.DropWhenDie();
            }
        }

        // Tắt collider của player
        Collider col = transform.GetComponent<Collider>();
        col.enabled = false;

        // Bật lại collider của con
        DisableChildColliders disableChildColliders = transform.GetComponent<DisableChildColliders>();
        disableChildColliders.EnableAllChildColliders();

        // Phát animation chết
        anim.SetTrigger("isFall");

        // Gọi enemy phục hồi lại
        EnemyPatrolNav epn = enemy.GetComponent<EnemyPatrolNav>();
        epn.ForceRecoverNow();

        // Gọi coroutine xử lý sau 5s
        StartCoroutine(HandleAfterDeath());
    }
    public void CallFalLSound()
    {
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.diePlayerSound, transform.position, 2f, 15f, 0.3f);
    }

    private IEnumerator HandleAfterDeath()
    {
        // Chờ animation isFall kết thúc
        //yield return new WaitForSeconds(0.75f);
        //AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.diePlayerSound, transform.position,2f,15f, 0.3f);

        // Chờ thêm 3 giây trước khi hiện canvas Death
        yield return new WaitForSeconds(3f);
        FindAndCloseAll();
        ResetRotations();
        // Hiển thị canvas Death
        if (canvasDeath != null)
            canvasDeath.gameObject.SetActive(true);
        if(dayCount < 5)
        {
            // Cập nhật text day
            if (textDay != null)
                textDay.text = "Day " + dayCount;
        }
        else
        {
            textDay.text = "The last day";
        }

            StartingPlayer startingPlayer = GetComponent<StartingPlayer>();
        if (dayCount >= 6)
        {

            startingPlayer.GameOver();
            yield return null; 
        }
        // Chờ 3 giây hiển thị canvas Death
        yield return new WaitForSeconds(3f);

        // Ẩn canvas Death
        if (canvasDeath != null)
            canvasDeath.gameObject.SetActive(false);
        if (pond.GetReturnDoorKeyBlue())
        {
            pond.SetTrueTab();
        }
        // Đặt lại vị trí và rotation
        //transform.position = new Vector3(-14.2f, -6.14f, 7.11f);
        transform.position = new Vector3(-14.214f, -6.241f, 7.314f);
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        hasDied = false;

        // Gọi StartingPlayer.ReturnPlay()
        
        if (startingPlayer != null)
        {
           
            startingPlayer.ReturnPlay();
        }
        PauseMenu pm = canvasMenu.GetComponent<PauseMenu>();
        pm.isDisabled = false;
    }


    public void PlayFootstepSound()
    {
        Vector3 origin = transform.position + Vector3.up * 2f;
        Vector3 direction = Vector3.down;
        float rayLength = 1.5f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength, groundLayer))
        {
            int layer = hit.collider.gameObject.layer;

            if (layer == LayerMask.NameToLayer("WoodFloor"))
            {
                //AudioManager.instance.isNotOutdoor();
                //audioManager.PlaySFX(audioManager.walkOnWood);
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.walkOnWood, transform.position);
            }
            else if (layer == LayerMask.NameToLayer("Grass") || layer == LayerMask.NameToLayer("GardenBake"))
            {
                //AudioManager.instance.isOutdoor();
                //audioManager.PlaySFX(audioManager.walkOnGrass);
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.walkOnGrass, transform.position);
            }
            else if(layer == LayerMask.NameToLayer("WoodFloorOutDoor"))
            {
                
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.walkOnWood, transform.position);
            }
            else
            {
                //AudioManager.instance.isNotOutdoor();

            }
            //else
            //{
            //    audioManager.PlaySFX(audioManager.walkOnWood); 
            //}

           
        }
       
    }
    public void FindAndCloseAll()
    {
        // Danh sách các loại script cần kiểm tra
        System.Type[] scriptTypes = new System.Type[]
        {
            typeof(opencloseDoor),
            typeof(openCabinetDoor),
            typeof(openCabinetDoor1),
            typeof(Drawer_Pull_X),
            typeof(SlidingDoor)
        };

        foreach (System.Type type in scriptTypes)
        {
            // Tìm tất cả các component trong scene thuộc loại script đó
            Object[] scripts = FindObjectsOfType(type);

            foreach (var script in scripts)
            {
                // Lấy field "open" trong script
                var openField = type.GetField("open");
                if (openField != null)
                {
                    bool isOpen = (bool)openField.GetValue(script);
                    if (isOpen)
                    {
                        // Tìm và gọi hàm CloseAfterRescene()
                        var closeMethod = type.GetMethod("CloseAfterRescene");
                        if (closeMethod != null)
                        {
                            closeMethod.Invoke(script, null);
                            Debug.Log($"Đã đóng {type.Name} trên object: {((MonoBehaviour)script).gameObject.name}");
                        }
                    }
                }
            }
        }
    }
    public void ResetRotations()
    {
        // Tìm tất cả object có script RotationOnClick
        RotationOnClick[] rotators = FindObjectsOfType<RotationOnClick>();

        foreach (var rotator in rotators)
        {
            Transform parent = rotator.transform.parent;
            if (parent == null) continue;

            // Lấy 2 phương thức GetFromRotationEuler và GetToRotationEuler
            MethodInfo getFromMethod = typeof(RotationOnClick).GetMethod("GetFromRotationEuler");
            MethodInfo getToMethod = typeof(RotationOnClick).GetMethod("GetToRotationEuler");

            if (getFromMethod == null || getToMethod == null)
            {
                Debug.LogWarning($"Script RotationOnClick trên {rotator.name} thiếu hàm GetFromRotationEuler hoặc GetToRotationEuler");
                continue;
            }

            // Lấy giá trị Vector3
            Vector3 fromRotation = (Vector3)getFromMethod.Invoke(rotator, null);
            Vector3 toRotation = (Vector3)getToMethod.Invoke(rotator, null);

            // Lấy góc hiện tại của cha
            Vector3 parentRot = parent.localRotation.eulerAngles;

            // So sánh (dùng khoảng cách nhỏ vì float không tuyệt đối)
            if (Vector3.Distance(parentRot, toRotation) < 0.01f)
            {
                parent.localRotation = Quaternion.Euler(fromRotation);
                Debug.Log($"[Rotation Reset] Cha của {rotator.name} đã được đặt lại rotation từ {toRotation} → {fromRotation}");
            }
        }
    }
    private void OnTriggerEnter(Collider hit)
    {
        // Kiểm tra xem object va chạm có tên "RingBell" không
        if (hit.transform.name == "RingBell")
        {
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.ringSound, hit.transform.position);
            fov.AutoHearSound(hit.transform);
        }
        else if(hit.transform.name == "RingBellDog")
        {
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.ringSound, hit.transform.position);
        }
    }

}
