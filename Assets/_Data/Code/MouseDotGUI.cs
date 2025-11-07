using UnityEngine;

public class MouseDotGUI : MonoBehaviour
{
    public float dotSize = 8f;

    private void Start()
    {
        // Ẩn con trỏ mặc định
        Cursor.visible = false;

        // Khóa con trỏ trong màn hình (tùy chọn, không nên nếu bạn muốn di chuyển tự do)
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnGUI()
    {
        // Lấy vị trí chuột
        Vector2 mousePos = Event.current.mousePosition;

        // Tạo khung vẽ dấu chấm
        Rect dotRect = new Rect(mousePos.x - dotSize / 2, mousePos.y - dotSize / 2, dotSize, dotSize);

        // Vẽ dấu chấm trắng
        GUI.color = Color.white;
        GUI.DrawTexture(dotRect, Texture2D.whiteTexture);
    }
}
