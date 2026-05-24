using UnityEngine;
using System.Collections;

public class MinuteHandController : MonoBehaviour
{
    [SerializeField] private float rotateDuration = 0.3f;

    private bool isHovered = false;
    private bool isRotating = false;
    [SerializeField] private bool isSet = false;
    public float CurrentAngle { get; private set; } = 0f;

    void Update()
    {
        if (isHovered && !isRotating)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(RotateHand(true));  // Quay xuôi
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(RotateHand(false)); // Quay ngược
            }
        }
    }

    public void SetIsSet()
    {
        isSet = true;
    }

    private IEnumerator RotateHand(bool clockwise)
    {
        isRotating = true;

        Vector3 currentEuler = transform.localEulerAngles;
        float currentZ = Mathf.Round(currentEuler.z / 30f) * 30f;
        float newZ;

        if (clockwise)
        {
            newZ = (currentZ + 30f) % 360f;
        }
        else
        {
            newZ = (currentZ - 30f + 360f) % 360f;  // Đảm bảo không bị âm
        }

        CurrentAngle = newZ;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(currentEuler.x, currentEuler.y, newZ);
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.tokSound, transform.position);
        float elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
        isRotating = false;
    }

    private void OnMouseEnter() => isHovered = true;
    private void OnMouseExit() => isHovered = false;
}
