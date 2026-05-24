//using UnityEngine;

//public class OpenDoorCar : MonoBehaviour
//{
//    [SerializeField] private Transform fuelForm;
//    [SerializeField] private Transform motorForm;
//    [SerializeField] private Transform garageForm;
//    [SerializeField] private Transform tireForm;  // Thêm TireForm
//    [SerializeField] private Transform car;

//    public bool isOpen = false;

//    public void Run()
//    {
//        // Kiểm tra FuelManager
//        FuelManager fuelManager = fuelForm.GetComponent<FuelManager>();
//        if (fuelManager == null || !fuelManager.isFuel)
//        {
//            Debug.Log("Fuel chưa đầy hoặc script FuelManager không được gán!");
//            return;
//        }

//        // Kiểm tra ButtonGarage
//        ButtonGarage buttonGarage = garageForm.GetComponent<ButtonGarage>();
//        if (buttonGarage == null || !buttonGarage.isSetted)
//        {
//            Debug.Log("Garage chưa set hoặc script ButtonGarage không được gán!");
//            return;
//        }

//        // Kiểm tra MotorBatterySetUp
//        MotorBatterySetUp motorBattery = motorForm.GetComponent<MotorBatterySetUp>();
//        if (motorBattery == null || !motorBattery.isSetted)
//        {
//            Debug.Log("MotorBatterySetUp chưa set hoặc script không được gán!");
//            return;
//        }

//        // Kiểm tra MotorBlockSetUp
//        MotorBlockSetUp motorBlock = motorForm.GetComponent<MotorBlockSetUp>();
//        if (motorBlock == null || !motorBlock.isSetted)
//        {
//            Debug.Log("MotorBlockSetUp chưa set hoặc script không được gán!");
//            return;
//        }

//        // Kiểm tra TireSetUp
//        TireSetUp tireSetup = tireForm.GetComponent<TireSetUp>();
//        if (tireSetup == null || !tireSetup.isSetted)
//        {
//            Debug.Log("TireSetUp chưa set hoặc script không được gán!");
//            return;
//        }

//        // Nếu tất cả đều đúng
//        isOpen = true;
//        CarRun carrun = car.GetComponent<CarRun>();
//        carrun.Run();
//        Debug.Log("Cửa xe đã mở!");
//    }
//}
using UnityEngine;

public class OpenDoorCar : MonoBehaviour
{
    [SerializeField] private Transform fuelForm;
    [SerializeField] private Transform motorForm;
    [SerializeField] private Transform garageForm;
    //[SerializeField] private Transform tireForm;
    [SerializeField] private Transform car;
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Transform carHood;

    private bool fuelReady = false;
    private bool garageReady = false;
    private bool motorBatteryReady = false;
    private bool motorBlockReady = false;
    //private bool tireReady = false;
    private bool carKeyReady = false;

    public bool isOpen = false;

    public void Run()
    {
        // 🔹 Kiểm tra Fuel
        FuelManager fuelManager = fuelForm.GetComponent<FuelManager>();
        fuelReady = (fuelManager != null && fuelManager.isFuel);

        // 🔹 Kiểm tra Garage
        ButtonGarage buttonGarage = garageForm.GetComponent<ButtonGarage>();
        garageReady = (buttonGarage != null && buttonGarage.isSetted);

        // 🔹 Kiểm tra Motor (Battery + Block)
        MotorBatterySetUp motorBattery = motorForm.GetComponent<MotorBatterySetUp>();
        MotorBlockSetUp motorBlock = motorForm.GetComponent<MotorBlockSetUp>();
        
        if(holdContainer.childCount > 0)
        {
            if(holdContainer.GetChild(0).name == "CarKey")
            {
                carKeyReady = true;
            }
        }

        motorBlockReady  = motorBlock != null && motorBlock.isSetted;
        motorBatteryReady = motorBattery != null && motorBattery.isSetted;
        // 🔹 Kiểm tra Tire
        //TireSetUp tireSetup = tireForm.GetComponent<TireSetUp>();
        //tireReady = (tireSetup != null && tireSetup.isSetted);

        // ✅ Nếu tất cả đều true → mở cửa
        if (fuelReady && garageReady && motorBatteryReady && motorBlockReady  && carKeyReady)
        {
            if (carHood.transform.localRotation.x != 0) return;
            isOpen = true;
            CarRun carRun = car.GetComponent<CarRun>();
            if (carRun != null)
            {
                PlayerObjectNameDisplay pc = holdContainer.parent.parent.GetComponent<PlayerObjectNameDisplay>();
                pc.setFalseLMB();
                carRun.Run();
                Debug.Log("✅ Cửa xe đã mở và xe sẵn sàng chạy!");
            }
        }
       
    }
}
