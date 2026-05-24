using UnityEngine;

public class FireTrigger : MonoBehaviour
{
    public string targetName = "FireEx";   // Object muốn kiểm tra
    public float collisionDuration = 3f; // Thời gian va chạm liên tục

    private float timer = 0f;
    private bool isColliding = false;

    void Update()
    {
        if (isColliding)
        {
            timer += Time.deltaTime;
            if (timer >= collisionDuration)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            timer = 0f;
        }

        // Reset trạng thái mỗi frame
        isColliding = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.name == targetName)
        {
            gameObject.SetActive(false);
            isColliding = true;
        }
    }
}
