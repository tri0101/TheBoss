using UnityEngine;

public class SetElectricHandle : MonoBehaviour
{
    [SerializeField] private Transform handle;
    [SerializeField] private Transform targetChild;   // object con cần set vị trí/rotation/scale
    [SerializeField] private Transform rotateTransform; // object cần có script RotateElectricHandle
    [SerializeField] private Transform player; // object cần có script RotateElectricHandle
    [SerializeField] private Transform gameObjectBox;

    public void Run()
    {
        if (handle == null)
        {
            Debug.LogWarning("Chưa gán handle trong Inspector!");
            return;
        }
        gameObjectBox.gameObject.SetActive(true);
        // Cho handle cùng cha với object hiện tại
        handle.SetParent(transform.parent);

        // Đặt transform mặc định cho handle
        handle.localPosition = Vector3.zero;
        handle.localRotation = Quaternion.identity;
        handle.localScale = Vector3.one;

        // Đổi tag của handle
        handle.tag = "Untagged";
        rotateTransform.name = "Sphere001";
        transform.name = "ElectricBox";
        // Nếu có object con được chỉ định thì set transform
        if (targetChild != null)
        {
            targetChild.localPosition = new Vector3(-0.5151965f, 0.06496462f, 0.2216314f);
            targetChild.localRotation = Quaternion.Euler(-100f, 0f, 0f);
            targetChild.localScale = Vector3.one;
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);
            // Đổi tag của targetChild
            targetChild.tag = "Untagged";

            // 🔑 Set tất cả MeshCollider của targetChild và con cháu
            MeshCollider[] colliders = targetChild.GetComponentsInChildren<MeshCollider>(true);
            foreach (MeshCollider col in colliders)
            {
                col.isTrigger = false;
            }
        }

        // Liên kết với script RotateElectricHandle
        RotateElectricHandle rotateScript = rotateTransform.GetComponent<RotateElectricHandle>();
        if (rotateScript != null)
        {
            rotateScript.SetIsSetUp();
        }
        else
        {
            Debug.LogWarning("Handle không có script RotateElectricHandle!");
        }

        // Thêm Rigidbody cho rotateTransform (nếu chưa có)
        Rigidbody rb = rotateTransform.GetComponent<Rigidbody>();
        if (rb == null) rb = rotateTransform.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.freezeRotation = true;
        SetLayerRecursively(handle, "Default");
        // Gọi release object từ player
        PickUpSystem pickUp = player.GetComponent<PickUpSystem>();
        if (pickUp != null)
            pickUp.ReleaseHeldObject();
    }
    void SetLayerRecursively(Transform target, string newLayer)
    {

        target.gameObject.layer = LayerMask.NameToLayer(newLayer);

        foreach (Transform child in target)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
}
