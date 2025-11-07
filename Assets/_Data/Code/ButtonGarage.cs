using UnityEngine;
using System.Collections;

public class ButtonGarage : MonoBehaviour
{
    [SerializeField] private Transform garageDoor;
    [SerializeField] private Transform player;
    public bool isSetted = false;

    PlayerObjectNameDisplay pond;
    private void Start()
    {
        pond = player.GetComponent<PlayerObjectNameDisplay>();
    }
   
    public void Run()
    {
        StartCoroutine(OpenGarage());
    }

    private IEnumerator OpenGarage()
    {
        // Thời gian chạy animation
        float duration = 12f; // chạy trong 2 giây, bạn có thể chỉnh
        float elapsed = 0f;
       
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.garageDoor, transform.position);

        // Lấy giá trị ban đầu
        Vector3 startLocalPos = transform.localPosition;
        Vector3 endLocalPos = new Vector3(startLocalPos.x, startLocalPos.y, -0.5f);

        Vector3 garageStartPos = garageDoor.localPosition;
        Vector3 garageEndPos = new Vector3(garageStartPos.x, 3.755f, garageStartPos.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Chạy từ từ theo t
            transform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, t);
            garageDoor.localPosition = Vector3.Lerp(garageStartPos, garageEndPos, t);

            yield return null;
        }

        // Đảm bảo kết thúc đúng giá trị cuối
        transform.localPosition = endLocalPos;
        garageDoor.localPosition = garageEndPos;
        pond.CompleteTask("garageReady");
        isSetted = true;
        Destroy(garageDoor.gameObject);
    }
}
