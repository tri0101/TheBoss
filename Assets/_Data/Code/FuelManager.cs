using UnityEngine;

public class FuelManager : MonoBehaviour
{
    // Biến public để kiểm tra trạng thái nhiên liệu
    public bool isFuel = false;
    [SerializeField] private PlayerObjectNameDisplay pond;

    // Hàm chạy để bật isFuel
    public void Run()
    {
        pond.CompleteTask("fuelReady");
        isFuel = true;
        GetComponent<InteractableObject>().enabled = false;
        pond.ShowMessage(5f, "You refueled the vehicle");
        Debug.Log("✅ Fuel is now active!");
    }
}
