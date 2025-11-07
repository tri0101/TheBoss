using UnityEngine;

public class OpenLaptop : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform holdContainer;
    [SerializeField] private Texture2D displayImage; // Ảnh PNG kéo vào

    private bool isLaptopOpen = false;
    private bool isSetBattery = false;

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

        //// Mở laptop UI
        //player.GetComponent<PlayerController>().enabled = false;
        //canvas.gameObject.SetActive(true);
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        
        isLaptopOpen = true;
    }

    //void Update()
    //{
    //    if (isLaptopOpen && Input.GetKeyDown(KeyCode.Q))
    //    {
    //        // Đóng laptop UI
    //        canvas.gameObject.SetActive(false);
    //        Cursor.lockState = CursorLockMode.Locked;
    //        Cursor.visible = false;
    //        player.GetComponent<PlayerController>().enabled = true;

    //        isLaptopOpen = false;
    //    }
    //}

    //void OnMouseOver()
    //{
    //    if (isSetBattery && Input.GetKeyDown(KeyCode.E))
    //    {
    //        Run();
    //    }
    //}
}
