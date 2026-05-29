using System.Collections;
using UnityEngine;
public class SecretBookDoor : MonoBehaviour
{
    [SerializeField] private Transform book1;
    [SerializeField] private Transform book2;
    [SerializeField] private Transform book3;
    [SerializeField] private Transform book4;
    [SerializeField] private Transform player;
    private PlayerObjectNameDisplay pond;

    private bool hasMove = false;
    private bool isCall = false;

    public Animator openandclose;
    public bool open;
    public bool isCode = true;
    public DoorSoundSO doorSound;
    private void Start()
    {
        pond = player.GetComponent<PlayerObjectNameDisplay>();
    }
    void Update()
    {
        if(!hasMove)
        {
            CheckStatus();
        }
    }

    void CheckStatus()
    {
        if (book1 == null || book2 == null || book3 == null || book4 == null)
        {
            Debug.LogWarning("Thiếu Transform Book.");
            return;
        }

        SetBook s1 = book1.GetComponent<SetBook>();
        SetBook s2 = book2.GetComponent<SetBook>();
        SetBook s3 = book3.GetComponent<SetBook>();
        SetBook s4 = book4.GetComponent<SetBook>();

        if (s1 == null || s2 == null || s3 == null || s4 == null)
        {
            Debug.LogWarning("Một trong các Transform không có SetBook.");
            return;
        }
        if (s1.IsSetted() && s2.IsSetted() && s3.IsSetted() && s4.IsSetted())
        {
            if (s1.IsTrue() && s2.IsTrue() && s3.IsTrue() && s4.IsTrue())
            {
                hasMove = true;
                MoveDoor();
                DisableInteractable(book1);
                DisableInteractable(book2);
                DisableInteractable(book3);
                DisableInteractable(book3);

                // 🔹 Đổi tag các object con có tên chứa "status"
                UntagStatusChildren(book1);
                UntagStatusChildren(book2);
                UntagStatusChildren(book3);
                UntagStatusChildren(book4);
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
    void MoveDoor()
    {
        StartCoroutine(opening());
    }
    IEnumerator opening()
    {
        
        Debug.Log("you are opening the door");
        openandclose.Play("Opening");
        open = true;
        AudioManager.instance.PlaySFXAtPosition(doorSound.openSound, transform.position);
        yield return new WaitForSeconds(0.5f);
    }
}
