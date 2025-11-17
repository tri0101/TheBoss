using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SetStatus : MonoBehaviour
{
    [SerializeField] private Transform holdContainer;
    [SerializeField] private PosStatus posStatus;
    [SerializeField] private Transform player;
    [SerializeField] private bool isTrue;
    [SerializeField] private bool isSetted;

    [SerializeField] private Transform selectedAnimal = null;

    public void Run()
    {
        if (holdContainer == null || posStatus == null || player == null || selectedAnimal != null)
        {
            Debug.LogWarning("Thiếu reference (holdContainer, posStatus, player).");
            return;
        }
        
        // Tìm con vật hợp lệ
        foreach (Transform child in holdContainer)
        {
            if (child.name == "BuffaloStatus" || child.name == "WolfStatus" || child.name == "LionStatus" || child.name == "RihnoStatus")
            {
                Debug.Log("Is Set ");
                selectedAnimal = child;
                break;
            }
            
        }

        if (selectedAnimal == null)
        {
            Debug.Log("Không tìm thấy Buffalo, Dog, Lion hoặc Rihno trong holdContainer.");
            return;
        }
        isSetted = true;
        selectedAnimal.gameObject.layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(selectedAnimal, "Default");
        // Gỡ khỏi container
        selectedAnimal.SetParent(null);
        Rigidbody rb = selectedAnimal.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = selectedAnimal.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            Debug.Log($"✅ Đã thêm Rigidbody cho {selectedAnimal.name}");
        }

        // Set transform
        selectedAnimal.position = posStatus.position;
        selectedAnimal.rotation = Quaternion.Euler(0f, 97.018f, 0f);
        
        selectedAnimal.localScale = Vector3.one * 0.1f;
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);

        // Gán về cha của object chứa script này
        Transform newParent = transform.parent;
        if (newParent != null)
        {
            selectedAnimal.SetParent(newParent);
        }

        // Nếu tên trùng tên cha thì true
        isTrue = (newParent != null && selectedAnimal.name == newParent.name);

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
        if (selectedAnimal != null)
        {

            bool isStillValid = selectedAnimal != null
                                && selectedAnimal.parent == transform.parent
                                && selectedAnimal.name == transform.parent.name;
            if(transform.parent.childCount == 1)
            {
                isTrue = false;
                selectedAnimal = null;
              
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
