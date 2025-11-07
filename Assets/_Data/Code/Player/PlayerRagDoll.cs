using UnityEngine;

public class PlayerRagDoll : MonoBehaviour
{
    private Rigidbody[] _ragdollRigidbodies;
    private Rigidbody _mainRigidbody;

    private void Awake()
    {
        _mainRigidbody = GetComponent<Rigidbody>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        DisableRagdoll();
    }

    private void DisableRagdoll()
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            if (rb != _mainRigidbody)
            {
                rb.isKinematic = true;
                rb.useGravity = false;   // 🔑 tắt gravity của bone
            }
        }
        // main rigidbody để di chuyển
        _mainRigidbody.isKinematic = false;
        _mainRigidbody.useGravity = true;
    }

    public void EnableRagdoll()
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            if (rb != _mainRigidbody) // loại trừ Rigidbody chính
                rb.isKinematic = false;
        }

        // main Rigidbody nên set Kinematic để nó không điều khiển nữa
        _mainRigidbody.isKinematic = true;
    }
}
