using UnityEngine;

public class GasCanRefuel : MonoBehaviour
{
    [SerializeField] private Transform gasCan;
    [SerializeField] private PlayerObjectNameDisplay pond;

    public void Run()
    {
        if (gasCan == null)
        {
            Debug.LogWarning("❌ Chưa gán gasCan trong Inspector!");
            return;
        }

        // Lấy PickUpConfig trên gasCan
        PickUpConfig pickUpConfig = gasCan.GetComponent<PickUpConfig>();
        if (pickUpConfig != null)
        {
            // Gọi hàm đổi tên
            pickUpConfig.ChangeNameObject("Full gas can");
            gasCan.name = "FullGasCan";
            Debug.Log("✅ Gas can đã được đổi thành Full gas can!");
            GetComponent<InteractableObject>().enabled = false;
            pond.ShowMessage(5f, "You filled the gas can");
        }
        else
        {
            Debug.LogWarning("❌ Không tìm thấy PickUpConfig trên gasCan!");
        }
    }
}
