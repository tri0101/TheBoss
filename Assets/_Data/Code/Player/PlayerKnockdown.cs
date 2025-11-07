using UnityEngine;

public class PlayerKnockout : MonoBehaviour
{
    private Rigidbody rb;
    private bool isKnockedOut = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ban đầu player đứng yên
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isKnockedOut)
        {
            KnockOut();
        }
    }

    void KnockOut()
    {
        isKnockedOut = true;
        rb.isKinematic = false;

        // reset vận tốc để force chính xác
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // cú uppercut: hất lên + ra sau
        Vector3 forceDir = (transform.up * 6f) + (-transform.forward * 3f);
        rb.AddForce(forceDir, ForceMode.Impulse);
    }
}
