using UnityEngine;

public class SecretDoor : MonoBehaviour
{
    [SerializeField] private Transform status1;
    [SerializeField] private Transform status2;
    [SerializeField] private Transform status3;
    [SerializeField] private Transform status4;
    [SerializeField] private Transform player;
    private PlayerObjectNameDisplay pond;

    private bool hasStartedMoving = false;
    private bool hasLowered = false;
    private bool isCall = false;

    private float moveDuration = 1.5f;
    private float moveTimer = 0f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private void Start()
    {
        pond = player.GetComponent<PlayerObjectNameDisplay>();
    }
    void Update()
    {
        if (hasStartedMoving)
        {
            MoveDoor();
        }
        else
        {
            CheckStatus();
        }
    }

    void CheckStatus()
    {
        if (status1 == null || status2 == null || status3 == null || status4 == null)
        {
            Debug.LogWarning("Thiếu Transform SetStatus.");
            return;
        }

        SetStatus s1 = status1.GetComponent<SetStatus>();
        SetStatus s2 = status2.GetComponent<SetStatus>();
        SetStatus s3 = status3.GetComponent<SetStatus>();
        SetStatus s4 = status4.GetComponent<SetStatus>();

        if (s1 == null || s2 == null || s3 == null || s4 == null)
        {
            Debug.LogWarning("Một trong các Transform không có SetStatus.");
            return;
        }
        if (s1.IsSetted() && s2.IsSetted() && s3.IsSetted() && s4.IsSetted())
        {
            if (s1.IsTrue() && s2.IsTrue() && s3.IsTrue() && s4.IsTrue())
            {
                hasStartedMoving = true;
                DisableInteractable(status1);
                DisableInteractable(status2);
                DisableInteractable(status3);
                DisableInteractable(status4);

                // 🔹 Đổi tag các object con có tên chứa "status"
                UntagStatusChildren(status1);
                UntagStatusChildren(status2);
                UntagStatusChildren(status3);
                UntagStatusChildren(status4);
            }
            else
            {
                if (isCall) return;
                pond.ShowMessage(2f, "Nothing happens");
                isCall = true;
            }
        }
        else
        {
            isCall = false;
        }
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
            if (child.name.ToLower().Contains("status"))
            {
                child.tag = "Untagged";
                child.gameObject.layer = LayerMask.NameToLayer("Default");
                SetTagAndLayerRecursively(child, "Untagged", "Default");
            }
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
    void MoveDoor()
    {
        if (!hasLowered)
        {
            // Hạ Y liền 1 lần
            transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z);
            hasLowered = true;

            // Lưu vị trí bắt đầu và kết thúc
            startPos = transform.position;
            targetPos = new Vector3(-15f, transform.position.y, transform.position.z);
            moveTimer = 0f;
        }
        else
        {
            // Di chuyển mượt theo X trong 1.5 giây
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            if (t >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
