using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SetBook : MonoBehaviour
{
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Transform player;
    [SerializeField] private bool isTrue;
    [SerializeField] private bool isSetted;

    [SerializeField] private Transform selectedBook = null;

    public void Run()
    {
        if (holdContainer == null || player == null || selectedBook != null)
        {
            Debug.LogWarning("Thiếu reference (holdContainer, posStatus, player).");
            return;
        }

        // Tìm con vật hợp lệ
        foreach (Transform child in holdContainer)
        {
            if (child.name == "Shadows of the Alley" ||
                child.name == "The Fallen Kingdom" ||
                child.name == "Soups and Stews" || 
                child.name == "Advanced Physics Concepts")
            {
                Debug.Log("Is Set ");
                selectedBook = child;
                break;
            }

        }

        if (selectedBook == null)
        {
            Debug.Log("Không tìm thấy Buffalo, Dog, Lion hoặc Rihno trong holdContainer.");
            return;
        }
        isSetted = true;
        selectedBook.gameObject.layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(selectedBook, "Default");
        // Gỡ khỏi container
        selectedBook.SetParent(null);
        Rigidbody rb = selectedBook.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = selectedBook.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            Debug.Log($"✅ Đã thêm Rigidbody cho {selectedBook.name}");
        }
        Transform newParent = transform.parent;
        if (newParent != null)
        {
            selectedBook.SetParent(newParent,false);
        }
        // Set transform
        selectedBook.localPosition = Vector3.zero;
        selectedBook.localRotation = Quaternion.identity;
        selectedBook.localScale = Vector3.one;
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);
        // Nếu tên trùng tên cha thì true
        isTrue = (newParent != null && selectedBook.name == newParent.name);

        // Gọi ReleaseHeldObject
        PickUpSystem pickUp = player.GetComponent<PickUpSystem>();
        if (pickUp != null)
        {
            pickUp.ReleaseHeldObject();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy PickUpSystem trên player.");
        }

        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();

        pond.setFalseLMB();
    }
    void SetLayerRecursively(Transform target, string newLayer)
    {

        target.gameObject.layer = LayerMask.NameToLayer(newLayer);

        foreach (Transform child in target)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
    private void Update()
    {
        // Nếu từng chọn rồi, thì kiểm tra tính hợp lệ liên tục
        if (selectedBook != null)
        {

            bool isStillValid = selectedBook != null
                                && selectedBook.parent == transform.parent
                                && selectedBook.name == transform.parent.name;
            if (transform.parent.childCount == 1)
            {
                isTrue = false;
                selectedBook = null;

            }


            if (!isStillValid)
            {
                isTrue = false;
                //selectedAnimal = null;
            }
        }
        else
        {
            isTrue = false;
            isSetted = false;
        }
    }

    public bool IsTrue()
    {
        return isTrue;
    }
    public bool IsSetted()
    {
        return isSetted;
    }
}
