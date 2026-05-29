using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SetCassette : MonoBehaviour
{
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Transform player;
    [SerializeField] private bool isTrue;
    [SerializeField] private bool isSetted;
    [SerializeField] private Transform doorCassette;
    [SerializeField] private Transform selectedObject = null;
    [SerializeField] private DoorCassette doorCassetteScript;
    [Header("Door Movement")]
    [SerializeField] private float doorCloseLocalYFrom = 0f;
    [SerializeField] private float doorCloseLocalYTo = -40f;
    [SerializeField] private float doorCloseDuration = 0.35f;

    [SerializeField] private bool isOk = false;// hoàn tất hết rồi
    public bool IsOk => isOk;
    private Coroutine closeDoorCoroutine;

    public void Run()
    {
        if (holdContainer == null || player == null || selectedObject != null)
        {
            Debug.LogWarning("Thiếu reference (holdContainer, player).");
            return;
        }

        // Tìm con vật hợp lệ
        foreach (Transform child in holdContainer)
        {
            if (child.name == "Baby Lullaby Cassette")
            {
                Debug.Log("Is Set ");
                selectedObject = child;
                break;
            }
        }

        if (selectedObject == null)
        {
            Debug.Log("ko thấy cát sét.");
            return;
        }

        isSetted = true;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(selectedObject, "Default");

        // Gỡ khỏi container
        selectedObject.SetParent(null);
        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = selectedObject.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            Debug.Log($"✅ Đã thêm Rigidbody cho {selectedObject.name}");
        }

        Transform newParent = transform.parent;
        if (newParent != null)
        {
            selectedObject.SetParent(newParent, false);
        }

        selectedObject.localPosition = Vector3.zero;
        selectedObject.localRotation = Quaternion.identity;
        selectedObject.localScale = Vector3.one;

        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);

        // Nếu tên trùng tên cha thì true
        isTrue = (newParent != null && selectedObject.name == newParent.name);

        DisableInteractable(selectedObject);
        UntagStatusChildren(selectedObject);

        if (closeDoorCoroutine != null) StopCoroutine(closeDoorCoroutine);
        closeDoorCoroutine = StartCoroutine(CloseCassetteDoor());

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

    private IEnumerator CloseCassetteDoor()
    {
        if (!isTrue || doorCassette == null) yield break;

        Vector3 fixedPos = doorCassette.localPosition;

        Quaternion startRot = Quaternion.Euler(
            doorCassette.localEulerAngles.x,
            0f,
            doorCassette.localEulerAngles.z
        );

        Quaternion endRot = Quaternion.Euler(
            doorCassette.localEulerAngles.x,
            -38f,
            doorCassette.localEulerAngles.z
        );

        doorCassette.localPosition = fixedPos;
        doorCassette.localRotation = startRot;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, doorCloseDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            doorCassette.localRotation = Quaternion.Lerp(startRot, endRot, t);
            doorCassette.localPosition = fixedPos;

            yield return null;
        }

        doorCassette.localRotation = endRot;
        doorCassette.localPosition = fixedPos;

        closeDoorCoroutine = null;

        RotationOnClick rotationOnClick =
            doorCassette.GetChild(0).GetComponent<RotationOnClick>();

        doorCassette.GetChild(0).name = "Door";
        rotationOnClick.enabled = false;
        isOk = true;
        doorCassetteScript.SetIsOk(true);
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
    }

    public bool IsTrue()
    {
        return isTrue;
    }

    public bool IsSetted()
    {
        return isSetted;
    }

    void DisableInteractable(Transform obj)
    {
        InteractableObject interactable = obj.GetComponent<InteractableObject>();
        if (interactable != null)
        {
            interactable.enabled = false;
        }
    }

    void UntagStatusChildren(Transform obj)
    {
        Transform parent = obj.parent;
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            child.tag = "Untagged";
            child.gameObject.layer = LayerMask.NameToLayer("Default");
            SetTagAndLayerRecursively(child, "Untagged", "Default");
        }
    }

    void SetTagAndLayerRecursively(Transform target, string newTag, string newLayer)
    {
        target.tag = newTag;
        target.gameObject.layer = LayerMask.NameToLayer(newLayer);

        foreach (Transform child in target)
        {
            SetTagAndLayerRecursively(child, newTag, newLayer);
        }
    }
}