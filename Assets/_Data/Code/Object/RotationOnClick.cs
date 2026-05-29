using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class RotationOnClick : MonoBehaviour
{
    private bool isRotated = false;
    private bool isMouseOver = false;
    private bool isRotating = false; // cờ cho biết đang xoay
    [SerializeField] private DoorSoundSO doorsound;

    [Header("Setup từ ScriptableObject")]
    [SerializeField] private SetUpRotationClick rotationData;
    //[SerializeField] private Transform player;
   

    [Header("Tốc độ xoay (độ/giây)")]
    public float rotationSpeed = 90f;

    // Góc hiện tại và target riêng cho từng trục
    private float currentX, currentY, currentZ;
    private float targetX, targetY, targetZ;
    private float checkRange = 3f;
    public Vector3 GetFromRotationEuler()
    {
        return rotationData.fromRotationEuler;
    }
    public Vector3 GetToRotationEuler()
    {
        return rotationData.toRotationEuler;
    }
    [SerializeField] private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    void Start()
    {

        // Khởi tạo rotation hiện tại từ fromRotationEuler
        currentX = rotationData.fromRotationEuler.x;
        currentY = rotationData.fromRotationEuler.y;
        currentZ = rotationData.fromRotationEuler.z;

        // Ban đầu target = current → object không xoay
        targetX = currentX;
        targetY = currentY;
        targetZ = currentZ;

        transform.parent.localRotation = Quaternion.Euler(currentX, currentY, currentZ);
    }

    void Update()

    {
        
        
        // Click chuột khi hover → chỉ khi không xoay
        if (isMouseOver && Input.GetMouseButtonDown(0) && !isRotating)
        {
            Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);


            if (Physics.Raycast(ray, out RaycastHit hit, checkRange))
            {
                targetX = isRotated ? rotationData.fromRotationEuler.x : rotationData.toRotationEuler.x;
                targetY = isRotated ? rotationData.fromRotationEuler.y : rotationData.toRotationEuler.y;
                targetZ = isRotated ? rotationData.fromRotationEuler.z : rotationData.toRotationEuler.z;
                if (!isRotated && doorsound.openSound != null)
                {
                    AudioManager.instance.PlaySFXAtPosition(doorsound.openSound, transform.position);
                }
                else if (isRotated && doorsound.closeSound != null)
                {
                    AudioManager.instance.PlaySFXAtPosition(doorsound.closeSound, transform.position);
                }
                isRotated = !isRotated;
                isRotating = true; // bắt đầu xoay
            }
                
        }

        // MoveTowards từng trục theo target với tốc độ rotationSpeed
        float prevX = currentX, prevY = currentY, prevZ = currentZ;

        currentX = rotationData.rotateX ? Mathf.MoveTowards(currentX, targetX, rotationSpeed * Time.deltaTime) : currentX;
        currentY = rotationData.rotateY ? Mathf.MoveTowards(currentY, targetY, rotationSpeed * Time.deltaTime) : currentY;
        currentZ = rotationData.rotateZ ? Mathf.MoveTowards(currentZ, targetZ, rotationSpeed * Time.deltaTime) : currentZ;

        // Áp dụng rotation mới
        transform.parent.localRotation = Quaternion.Euler(currentX, currentY, currentZ);

        // Nếu xoay xong (đạt target trên tất cả trục) → cho phép click lại
        if (isRotating && Mathf.Approximately(currentX, targetX)
                       && Mathf.Approximately(currentY, targetY)
                       && Mathf.Approximately(currentZ, targetZ))
        {
            isRotating = false;
        }
    }

    void OnMouseOver() => isMouseOver = true;
    void OnMouseExit() => isMouseOver = false;
       
    
}
