using UnityEngine;

public class DartControl : MonoBehaviour
{
    [SerializeField] private bool isFlying = false;      // Phi tiêu đang bay hay không
    private Rigidbody rb;
    [SerializeField] private bool hasBeenShot = false;  // Đã được bắn ra chưa
    public bool GetShot()
    {
        return hasBeenShot;
    }
    public void  SetShot()
    {
        hasBeenShot = false;
    }
    void Start()
    {
        
    }

    void Update()
    {
        // Nếu phi tiêu đã bắn mà vận tốc gần bằng 0 (bị cắm, dừng lại)
        if (hasBeenShot && rb != null)
        {
            if (rb.linearVelocity.magnitude < 0.2f)
                isFlying = false;
        }
    }

    // 🟢 Gọi hàm này khi bắn (từ TryShootDart)
    public void StartFlying()
    {
        //if (rb == null)
        //    rb = GetComponent<Rigidbody>();

        //rb.isKinematic = false;
        //rb.useGravity = true;
        //rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //rb.AddForce(dir * force, ForceMode.Impulse);
        rb = GetComponent<Rigidbody>();
        hasBeenShot = true;
        isFlying = true;
    }

    // 🔴 Khi va chạm với tường, mục tiêu, v.v.
    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenShot) return; // tránh nhầm va chạm khi chưa bắn

        isFlying = false;
        if(collision.transform.name == "The Boss" || collision.transform.name == "Wolf_URP")
        {
            rb = GetComponent<Rigidbody>();
            // Giữ nguyên phi tiêu dính lại
            rb.isKinematic = true;
            rb.useGravity = false;

            // Ghim phi tiêu vào vị trí va chạm (tùy chọn)
            ContactPoint contact = collision.contacts[0];
            transform.position = contact.point;
            transform.rotation = Quaternion.LookRotation(contact.normal * -1f);

            // Tùy chọn: Gắn vào vật thể bị bắn trúng
            transform.SetParent(collision.transform);
            
        }
        else
        {
            hasBeenShot = false;
        }
        
    }
}
