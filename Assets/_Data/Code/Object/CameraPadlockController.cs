
using UnityEngine;

public class CameraPadlockController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera padlockCamera;
    [SerializeField] private Transform cursorFollower;
    [SerializeField] private Transform canvasQBack;
    [SerializeField] private Transform canvasQ;
    [SerializeField] private Transform canvasLMB;

    public float dotSize = 8f;

    // Vùng giới hạn trong world space
    private readonly Vector3 worldMin = new Vector3(-12.20f, -4.18f, 9.20f);
    private readonly Vector3 worldMax = new Vector3(-12.01f, -3.88f, 9.35f);

    private Vector3 clampedMousePos;

    private void OnEnable()
    {
        if (padlockCamera == null)
            padlockCamera = GetComponentInChildren<Camera>();

        if (cursorFollower != null)
            cursorFollower.gameObject.SetActive(true);
        canvasQBack.gameObject.SetActive(true);
        canvasQ.gameObject.SetActive(false);
        canvasLMB.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (player != null) player.SetActive(true);
            gameObject.SetActive(false);
            if(cursorFollower != null) cursorFollower.gameObject.SetActive(false);
            canvasQBack.gameObject.SetActive(false);
            canvasQ.gameObject.SetActive(true);
          

        }

        if (cursorFollower != null && padlockCamera != null)
        {
            // Convert world bounds sang screen space
            Vector3 screenMin = padlockCamera.WorldToScreenPoint(worldMin);
            Vector3 screenMax = padlockCamera.WorldToScreenPoint(worldMax);

            // Clamp mouse position
            clampedMousePos = Input.mousePosition;
            clampedMousePos.x = Mathf.Clamp(clampedMousePos.x, screenMin.x, screenMax.x);
            clampedMousePos.y = Mathf.Clamp(clampedMousePos.y, screenMin.y, screenMax.y);
            clampedMousePos.z = padlockCamera.WorldToScreenPoint(cursorFollower.position).z;

            // Chuyển sang world space và cập nhật vị trí object
            Vector3 worldPos = padlockCamera.ScreenToWorldPoint(clampedMousePos);
            cursorFollower.position = worldPos;

            // Debug (nếu cần)
            Debug.Log($"ClampedMouse: {clampedMousePos} → World: {worldPos}");
        }
    }

    private void OnGUI()
    {
        // Đảo Y để dùng trong GUI
        Vector2 guiMousePos = new Vector2(clampedMousePos.x, Screen.height - clampedMousePos.y);

        // Vẽ chấm trắng
        Rect dotRect = new Rect(guiMousePos.x - dotSize / 2, guiMousePos.y - dotSize / 2, dotSize, dotSize);
        GUI.color = Color.white;
        GUI.DrawTexture(dotRect, Texture2D.whiteTexture);
    }
}
