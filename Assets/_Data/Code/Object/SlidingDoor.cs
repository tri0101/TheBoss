using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    private bool isMoving = false;
    private Vector3 targetPosition;
    private float moveSpeed = 200f; // đơn vị mỗi giây

    // Vị trí đóng và mở, chỉ thay đổi X
    private float closedX = -196f;
    private float openedX = -23f;

    private float fixedY = -56f;
    private float fixedZ = -2.751159f;

    // Trạng thái cửa
    public bool open { get; private set; } = false;

    // Tham chiếu tới AudioManager

    [SerializeField] private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    

    private void OnMouseOver()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (Input.GetMouseButtonDown(0) && !isMoving)
            {
                ToggleDoor();
            }
        }
            
    }

    public void OpenBoss()
    {
        if (!isMoving && !open)   // 👉 chỉ mở khi đang đóng
            ToggleDoor();
    }

    public void CloseBoss()
    {
        if (!isMoving && open)   // 👉 chỉ đóng khi đang mở
            ToggleDoor();
    }

    private void ToggleDoor()
    {
        Transform parent = transform.parent;
        float currentX = parent.localPosition.x;

        if (Mathf.Abs(currentX - closedX) < 1f)
        {
            // 👉 Đang đóng → mở
            targetPosition = new Vector3(openedX, fixedY, fixedZ);
            open = true;
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.slidingDoorOpen, transform.position);
        }
        else
        {
            // 👉 Đang mở → đóng
            targetPosition = new Vector3(closedX, fixedY, fixedZ);
            open = false;
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.slidingDoorClose, transform.position);
        }

        isMoving = true;
    }
    public void CloseAfterRescene()
    {
        targetPosition = new Vector3(closedX, fixedY, fixedZ);
        open = false;
        isMoving = true;
    }
    private void Update()
    {
        if (isMoving)
        {
            Transform parent = transform.parent;
            Vector3 currentPos = parent.localPosition;
            Vector3 newPos = Vector3.MoveTowards(currentPos, targetPosition, moveSpeed * Time.deltaTime);

            // Giữ nguyên Y và Z trong suốt quá trình di chuyển
            newPos.y = fixedY;
            newPos.z = fixedZ;

            parent.localPosition = newPos;

            if (Vector3.Distance(parent.localPosition, targetPosition) < 0.01f)
            {
                parent.localPosition = targetPosition;
                isMoving = false;
            }
        }
    }
}
