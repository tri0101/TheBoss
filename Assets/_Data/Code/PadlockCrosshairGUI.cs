using UnityEngine;

public class PadlockCrosshairGUI : MonoBehaviour
{
    private void OnGUI()
    {
        float size = 8f;
        float posX = (Screen.width - size) / 2;
        float posY = (Screen.height - size) / 2;

        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(posX, posY, size, size), Texture2D.whiteTexture);
    }
}
