using UnityEngine;
using System.Collections;

public class GateBreak : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Car") // kiểm tra xe
        {
            // Gọi coroutine xoay parent.parent
            Transform target = transform.parent?.parent;
            if (target != null)
            {
                StartCoroutine(RotateGate(target, 0f, -90f, 0.5f)); // 2 giây để xoay
            }
        }
    }

    private IEnumerator RotateGate(Transform gate, float startY, float endY, float duration)
    {
        float elapsed = 0f;
        Quaternion startRot = Quaternion.Euler(gate.localEulerAngles.x, startY, gate.localEulerAngles.z);
        Quaternion endRot = Quaternion.Euler(gate.localEulerAngles.x, endY, gate.localEulerAngles.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            gate.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        gate.localRotation = endRot; // đảm bảo dừng đúng góc cuối
    }
}
