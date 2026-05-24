using UnityEngine;

public class TireSetUp : MonoBehaviour
{
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Transform player;
    public bool isSetted = false;
    public void Run()
    {
        if (holdContainer == null || holdContainer.childCount == 0)
        {
            Debug.LogWarning("❌ Không có child trong holdContainer!");
            return;
        }

        // 🔹 Gọi hàm ReleaseHeldObject() trong PickUpSystem của player
        PickUpSystem pickUpSystem = player.GetComponent<PickUpSystem>();
        if (pickUpSystem != null)
        {
            pickUpSystem.ReleaseHeldObject();
        }
        else
        {
            Debug.LogWarning("❌ Không tìm thấy PickUpSystem trong player!");
        }

        // Lấy object con đầu tiên của holdContainer
        Transform childObj = holdContainer.GetChild(0);

        // Đưa nó thành con của cha của cha của object chứa script này
        if (transform.parent != null && transform.parent.parent != null)
        {
            childObj.SetParent(transform.parent.parent);
        }
        else
        {
            Debug.LogWarning("❌ Object này không có đủ cấp cha để set!");
            return;
        }

        // Set localPosition mới
        childObj.localPosition = new Vector3(0.6462312f, 0.2816171f, 1.157501f);

        // ✅ Reset localRotation về (0,0,0)
        childObj.localRotation = Quaternion.Euler(Vector3.zero);

        // Đổi tag object đó và toàn bộ con của nó thành "Untagged"
        SetTagRecursively(childObj.gameObject, "Untagged");
        SetLayerRecursively(childObj, "Default");
        isSetted = true;
    }

    private void SetTagRecursively(GameObject obj, string newTag)
    {
        obj.tag = newTag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, newTag);
        }
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
