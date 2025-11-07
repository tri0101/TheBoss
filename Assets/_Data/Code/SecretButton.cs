using UnityEngine;

public class ButtonSecret : MonoBehaviour
{
    [SerializeField] private Transform secretDoor;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveToX = -10f;

    private Animator buttonAnimator;
    private bool isActivated = false;
    private bool doorMoving = false;

    private void Awake()
    {
        buttonAnimator = GetComponent<Animator>();
        if (buttonAnimator == null)
            Debug.LogWarning("Không tìm thấy Animator trên nút.");

        if (secretDoor == null)
            Debug.LogWarning("Chưa gán secretDoor trong ButtonSecret.");
    }

    private void OnMouseDown()
    {
        if (isActivated || secretDoor == null) return;

        isActivated = true; // ✅ Bấm lần đầu thì khóa luôn

        // Gửi trigger "Push"
        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Push");

        // Tăng z nhẹ và bắt đầu di chuyển
        secretDoor.position += new Vector3(0f, 0f, 0.0001f);
        doorMoving = true;
    }

    private void Update()
    {
        if (!doorMoving || secretDoor == null) return;

        Vector3 pos = secretDoor.position;
        float newX = Mathf.MoveTowards(pos.x, moveToX, moveSpeed * Time.deltaTime);
        secretDoor.position = new Vector3(newX, pos.y, pos.z);

        if (Mathf.Approximately(newX, moveToX))
        {
            doorMoving = false; // ✅ Đã tới nơi
        }
    }
}
