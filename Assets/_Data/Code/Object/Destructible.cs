using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion; // Prefab khi bị phá
    [SerializeField] private Transform holdContainer; // Chỗ chứa vật người chơi đang cầm
    [SerializeField] private GameObject clockHand;


    
   
    //void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (holdContainer.childCount > 0 && holdContainer.GetChild(0).name.StartsWith("Axe"))
    //        {
    //            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.crateBreak, transform.position);
    //        }
    //    }
    //}
    public void Run()
    {
        if (holdContainer.childCount > 0 && holdContainer.GetChild(0).name.StartsWith("Axe"))
        {
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.crateBreak, transform.position);
            CallDestruct();
        }
    }
    public void CallDestruct()
    {
        // Tạo phiên bản bị phá
        GameObject destroyedObj = Instantiate(destroyedVersion, transform.position, transform.rotation);

        // Cho biến mất sau 5 giây
        Destroy(destroyedObj, 5f);

        if (gameObject.name == "Whisky_Bottle")
        {
            SpawnClockHand();

            // Ẩn mesh để coi như biến mất
            MeshRenderer mesh = GetComponent<MeshRenderer>();
            if (mesh != null) mesh.enabled = false;

            // Nếu có collider thì tắt luôn để không còn va chạm
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Sau 2 giây mới xóa hẳn object
            Destroy(gameObject, 2f);
        }
        else
        {
            // Các object khác thì xóa ngay
            Destroy(gameObject);
        }
    }

    private void SpawnClockHand()
    {
        if (clockHand != null)
        {
            Transform spawnPoint = transform.Find("positionSpawn");

            if (spawnPoint != null)
            {
                // Di chuyển object gốc clockHand tới vị trí spawnPoint
                clockHand.transform.position = spawnPoint.position;
                clockHand.transform.rotation = spawnPoint.rotation;

                // Bật object gốc lên
                clockHand.name = "ClockHandtwo";
                clockHand.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy child 'positionSpawn' trong " + gameObject.name);
            }
        }
    }

}
