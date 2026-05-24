using UnityEngine;
using System.Collections;

public class VentRemove : MonoBehaviour
{
    [SerializeField] private Transform nail1;
    [SerializeField] private Transform nail2;

    private NailRemove nailRemove1;
    private NailRemove nailRemove2;
    private Rigidbody rb;
    private bool triggered = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (nail1 != null) nailRemove1 = nail1.GetComponent<NailRemove>();
        if (nail2 != null) nailRemove2 = nail2.GetComponent<NailRemove>();
    }

    private void Update()
    {
        // Kiểm tra nếu cả 2 đinh đều đã gỡ và chưa chạy logic
        if (!triggered && nailRemove1 != null && nailRemove2 != null)
        {
            if (nailRemove1.isRemoved && nailRemove2.isRemoved)
            {
                StartCoroutine(ToggleKinematic());
                triggered = true;
            }
        }
    }

    private IEnumerator ToggleKinematic()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // tắt kinematic
            Debug.Log("Vent Rigidbody isKinematic OFF");
            yield return new WaitForSeconds(1.5f);
            rb.isKinematic = true;  // bật lại
            Debug.Log("Vent Rigidbody isKinematic ON");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WoodFloor"))
        {
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.tireSound,transform.position);
        }
    }
}
