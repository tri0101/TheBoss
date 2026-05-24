using UnityEngine;

public class SetHourHand : MonoBehaviour
{
    [SerializeField] private Transform rootNote;
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Transform player;

    public bool isSetting = false; // ✅ Thêm biến này
    
    public void Run()
    {
        if (rootNote == null || holdContainer == null || player == null)
        {
            Debug.LogWarning("Thiếu tham chiếu!");
            return;
        }

        PickUpSystem pickUp = player.GetComponent<PickUpSystem>();
        if (pickUp != null)
            pickUp.ReleaseHeldObject();

        if (holdContainer.childCount == 0)
        {
            Debug.LogWarning("HoldContainer không có object con.");
            return;
        }

        Transform heldObject = holdContainer.GetChild(0);
        heldObject.SetParent(rootNote);
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);
        heldObject.localPosition = new Vector3(-0.01141216f, 152.2344f, 7.58461f);
        heldObject.localRotation = Quaternion.Euler(0f, 0f, 0f);
        heldObject.localScale = Vector3.one;

        HourHandController controller = heldObject.GetComponentInChildren<HourHandController>();
        if (controller != null)
            controller.enabled = true;
        heldObject.tag = "Untagged";
        foreach (Transform child in heldObject)
        {
            child.tag = "Untagged"; // ✅ Đặt tag cho child

            SetLayerRecursively(child, "Default");
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);

            MeshCollider meshCol = child.GetComponent<MeshCollider>();
            if (meshCol != null) meshCol.isTrigger = false;
        }

        isSetting = true; // ✅ Đánh dấu đã chạy
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
