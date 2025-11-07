using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    private bool isRotated = false;
    private bool isMouseOver = false;

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private Quaternion currentTarget;

    public float rotationSpeed = 2f; // tốc độ xoay

    void Start()
    {
        // Lấy rotation ban đầu của object cha
        originalRotation = transform.parent.localRotation;
        targetRotation = originalRotation * Quaternion.Euler(0f, 0f, -90f); // xoay tương đối
        currentTarget = originalRotation;
    }

    void Update()
    {
        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            if (!isRotated)
            {
                currentTarget = targetRotation;
                isRotated = true;
            }
            else
            {
                currentTarget = originalRotation;
                isRotated = false;
            }
        }

        // Xoay mượt tới góc mong muốn
        transform.parent.localRotation = Quaternion.Slerp(
            transform.parent.localRotation,
            currentTarget,
            Time.deltaTime * rotationSpeed
        );
    }

    void OnMouseOver()
    {
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }
}
