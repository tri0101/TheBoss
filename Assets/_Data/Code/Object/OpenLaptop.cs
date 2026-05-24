using UnityEngine;

public class OpenLaptop : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Texture2D displayImage;
    [SerializeField] private Texture2D displayImage1;
    [SerializeField] private Texture2D displayImage2;
    [SerializeField] private Texture2D displayImage3;
    [SerializeField] private Texture2D displayImage4;
    PlayerController playerController;
    private bool isLaptopOpen = false;
    public bool IsLaptopOpen => isLaptopOpen;
    [SerializeField]private bool isSetBattery = false;
    public bool IsSetBattery => isSetBattery;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();     
        
    }
    public void Run()
    {
        // Chỉ thực hiện một lần
        if (!isSetBattery)
        {
            // Xoá con trong holdContainer
            foreach (Transform child in holdContainer)
            {
                Destroy(child.gameObject);
            }

            // Tìm con tên "LX Laptop Display Assembly"
            Transform displayAssembly = transform.Find("LX Laptop Display Assembly");
            if (displayAssembly != null)
            {
                // Lấy MeshRenderer từ con đó
                MeshRenderer meshRenderer = displayAssembly.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Duyệt từng material và tìm theo tên
                    foreach (Material mat in meshRenderer.materials)
                    {
                        if (mat.name.Contains("LX Laptop Display"))
                        {
                            // Gán BaseMap = ảnh PNG
                            mat.SetTexture("_BaseMap", displayImage);

                            // Gán màu trắng RGBA = 255
                            mat.color = Color.white;
                            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy MeshRenderer trong LX Laptop Display Assembly.");
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy con 'LX Laptop Display Assembly'.");
            }
            transform.name = "LapTopWithBattery";
            isSetBattery = true;
        }
        else
        {
            // Mở laptop UI
            player.GetComponent<PlayerController>().enabled = false;
            canvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerObjectNameDisplay pond=  player.GetComponent<PlayerObjectNameDisplay>();
            pond.CanvasButtonQCloseLaptop.gameObject.SetActive(true);
            isLaptopOpen = true;
        }
            
    }
    public void CloseLaptop()
    {
        canvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<PlayerController>().enabled = true;
        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();
        pond.CanvasButtonQCloseLaptop.gameObject.SetActive(false);
        isLaptopOpen = false;
    }
    void Update()
    {
        
        if (isLaptopOpen && Input.GetKeyDown(KeyCode.Q))
        {
            CloseLaptop();
        }
    }

    void OnMouseOver()
    {
        if (isSetBattery && Input.GetKeyDown(KeyCode.E))
        {
            Run();
        }
    }
}
