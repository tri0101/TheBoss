using UnityEngine;

public class MotorBlockSetUp : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform holdContainer;
    public bool isSetted = false;
    PlayerObjectNameDisplay pond;
    private void Start()
    {
         pond = player.GetComponent<PlayerObjectNameDisplay>();
    }
    public void Run()
    {
        if (holdContainer == null)
        {
            Debug.LogError("❌ Chưa gán HoldContainer!");
            return;
        }

        if (player == null)
        {
            Debug.LogError("❌ Chưa gán Player!");
            return;
        }

        // Lấy tất cả con của holdContainer
        foreach (Transform child in holdContainer)
        {
            // Set parent mới là cha của object chứa script này
            child.SetParent(this.transform.parent);

            // Gán vị trí, xoay và scale mong muốn
            child.localPosition = new Vector3(0.05999935f, 0.3695f, 1.2645f);
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);
            SetLayerRecursively(child, "Default");
        }

        // Gọi ReleaseHeldObject() trong PickUpSystem của player
        PickUpSystem pickUpSystem = player.GetComponent<PickUpSystem>();
        if (pickUpSystem != null)
        {
            pickUpSystem.ReleaseHeldObject();
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy PickUpSystem trong Player!");
        }
        

        isSetted = true;
        pond.CompleteTask("motorBlockReady");
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
