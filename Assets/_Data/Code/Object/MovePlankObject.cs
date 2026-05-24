using UnityEngine;
using System.Collections;
using SojaExiles;

public class MovePlankObject : MonoBehaviour
{
    public float moveDuration = 0.5f; // thời gian di chuyển mượt
    private bool isMoving = false;
    [SerializeField] private Transform targetDoor;
    private bool isCallSound = false;
    public void Run()
    {
        if (isMoving) return; // tránh gọi nhiều lần cùng lúc
        StartCoroutine(MoveSmoothly());
    }

    //private IEnumerator MoveSmoothly()
    //{
    //    isMoving = true;

    //    // 🔹 Tắt isKinematic nếu object có Rigidbody
    //    Rigidbody rb = GetComponent<Rigidbody>();
    //    if (rb != null)
    //    {
    //        rb.isKinematic = false;
    //    }

    //    // 🔹 Lấy vị trí ban đầu và vị trí đích
    //    Vector3 startPos = transform.localPosition;
    //    Vector3 targetPos = startPos + new Vector3(-0.4f, 0.2f, 0f);

    //    float elapsedTime = 0f;

    //    // 🔹 Di chuyển mượt bằng Lerp
    //    while (elapsedTime < moveDuration)
    //    {
    //        transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    // 🔹 Gán vị trí cuối cùng chính xác
    //    transform.localPosition = targetPos;
    //    opencloseDoor ocD = targetDoor.GetComponent<opencloseDoor>();
    //    targetDoor.name = "_Door.Blockk";
    //    ocD.isCode = true;
    //    // 🔹 Hủy object sau 1 giây
    //    Destroy(gameObject, 1f);

    //    isMoving = false;

    //}
    private IEnumerator MoveSmoothly()
    {
        isMoving = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true; // 🔹 Giữ object cố định khi di chuyển

        // 🔹 Lấy vị trí ban đầu và đích
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y, 3.4f);

        float elapsedTime = 0f;

        // 🔹 Di chuyển mượt dọc trục Z
        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 🔹 Đảm bảo vị trí cuối chính xác
        transform.localPosition = targetPos;

        // 🔹 Cho phép Rigidbody hoạt động vật lý trở lại
        if (rb != null)
            rb.isKinematic = false;
        Destroy(gameObject, 1.5f);
        opencloseDoor ocD = targetDoor.GetComponent<opencloseDoor>();
        targetDoor.name = "_Door.Blockk";
        ocD.isCode = true;
        isMoving = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCallSound) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("WoodFloor"))
        {
            isCallSound = true;
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.stickSound, transform.position);
        }
    }
}
