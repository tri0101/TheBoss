using UnityEngine;
using System.Collections;

public class DoorClock : MonoBehaviour
{
    private bool isHovered = false;
    private bool rotatedNegative = false;
    private bool isRotating = false;

    [SerializeField] private float rotateDuration = 0.5f;

    void Update()
    {
        if (isHovered && Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            StartCoroutine(RotateDoorSmooth());
        }
    }

    private IEnumerator RotateDoorSmooth()
    {
        isRotating = true;

        // Lưu góc bắt đầu
        Quaternion startRotation = transform.localRotation;

        // Xác định góc đích
        float angleDelta = rotatedNegative ? 90f : -90f;
        Quaternion endRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + angleDelta,
            transform.localEulerAngles.z
        );

        float elapsed = 0f;

        while (elapsed < rotateDuration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRotation;
        rotatedNegative = !rotatedNegative;
        isRotating = false;
    }

    private void OnMouseEnter()
    {
        isHovered = true;
    }

    private void OnMouseExit()
    {
        isHovered = false;
    }
}
