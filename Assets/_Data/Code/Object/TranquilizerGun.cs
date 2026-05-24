using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class TranquilizerGun : MonoBehaviour
{
    public Transform dartTransform;
    public float shootForce = 5f;

    [Header("Camera reference")]
    public Transform fpsCam;
    [SerializeField] private Transform gunBody;
    [SerializeField] private Transform player;
    [SerializeField] private PickUpSystem pcs;
    [SerializeField] private PlayerObjectNameDisplay pond;
    private Collider gunBodyCollider;

    

    private void Awake()
    {
        pond = player.gameObject.GetComponent<PlayerObjectNameDisplay>();
        gunBodyCollider = gunBody.GetComponent<Collider>();
    }
    private void Start()
    {
        pcs = player.GetComponent<PickUpSystem>();
    }

    private void Update()
    {
        if (dartTransform == null)
        {
            Transform found = transform.Find("Tranquilizer dart");
            if (found == null)
            {
                found = transform.Find("Used tranquilizer dart");
            }

            if (found != null) dartTransform = found;
        }

        if (Input.GetMouseButtonDown(1) && transform.parent != null && transform.parent.name == "holdContainer")
        {
            TryShootDart();
        }
    }

    void TryShootDart()
    {
        if (dartTransform == null)
        {
            pond.ShowMessage(2f, "It's missing tranquilizer dart");
            Debug.LogWarning("Không tìm thấy Dart để bắn.");
            return;
        }

        dartTransform.SetParent(null);
        DartControl dc = dartTransform.GetComponent<DartControl>();
        dc.StartFlying();
        Rigidbody rb;
        rb = dartTransform.gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.mass = 0.05f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 1f;
        // Bật lại MeshCollider
        MeshCollider meshCol = dartTransform.GetComponentInChildren<MeshCollider>();
        //if (meshCol != null) meshCol.enabled = true;

        // Bật lại tất cả collider và tắt trigger
        foreach (Collider col in dartTransform.GetComponentsInChildren<Collider>())
            col.isTrigger = false;

        // Tạm thời bỏ qua va chạm giữa Dart và thân súng
        if (meshCol != null && gunBodyCollider != null)
        {
            Physics.IgnoreCollision(meshCol, gunBodyCollider, true);
           
        }

        //// Chuẩn bị Rigidbody để bay
        //Rigidbody rb = dartTransform.GetComponent<Rigidbody>();
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.tranquilizerGunShot, transform.position); 
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Vector3 shootDir = fpsCam != null ? fpsCam.forward : transform.forward;
            rb.AddForce(shootDir * shootForce, ForceMode.Impulse);
        }
        StartCoroutine(ReenableCollisionAfterDelay(rb, meshCol, gunBodyCollider, 0.5f));
        dartTransform = null;
        pcs.MinusCountGun();
    }

    // 👇 Coroutine để bật lại va chạm sau delay
    private IEnumerator ReenableCollisionAfterDelay(Rigidbody rb, Collider dartCol, Collider gunCol, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (dartCol != null && gunCol != null)
        {
            Physics.IgnoreCollision(dartCol, gunCol, false);
        }
        
        rb.mass = 1f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
    }
}
