using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static DontDestroyOnLoad instance;

    private void Awake()
    {
        // Nếu object cùng tên đã tồn tại, xóa cái mới để tránh trùng
        var existing = GameObject.FindObjectsOfType<DontDestroyOnLoad>();
        foreach (var obj in existing)
        {
            if (obj != this && obj.name == gameObject.name)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Giữ lại khi load scene mới
        DontDestroyOnLoad(gameObject);
    }
}
