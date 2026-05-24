using UnityEngine;

public class PoisonMeatTest : MonoBehaviour
{
    [SerializeField] private bool isPoisoned = false;
    [SerializeField] private Transform holdContainer;
    [SerializeField] private PlayerObjectNameDisplay pond;

    // Hàm public có thể gọi từ script khác, hoặc gán trong UnityEvent (Button, Trigger...)
    private PickUpConfig config;

    
    private void Awake()
    {
        config = transform.GetComponent<PickUpConfig>();
        isPoisoned = false;
    }
    public void Run()
    {
        if (isPoisoned)
        {
            return;
        }
        isPoisoned = true;
        config.ChangeNameObject("Poisioned Meat");

        // Kiểm tra các object con trong holdContainer
        if (holdContainer != null)
        {
            foreach (Transform child in holdContainer)
            {
                if (child.name == "Tranquilizer dart")
                {
                    PickUpConfig dartConfig = child.GetComponent<PickUpConfig>();
                    if (dartConfig != null)
                    {
                        dartConfig.ChangeNameObject("Used Tranquilizer Dart");
                        child.name = "Used Tranquilizer Dart";
                    }
                    GetComponent<InteractableObject>().enabled = false;
                    pond.ShowMessage(5f, "You injected the tranquilizer into the meat");
                    break; // đã tìm thấy thì thoát vòng lặp
                }
            }
        }
    }
    public bool GetPoisoned()
    {
        return isPoisoned;
    }

}
