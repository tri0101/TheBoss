using UnityEngine;
using System.IO;

public class TextureExporter : MonoBehaviour
{
    public Texture2D texture;

    void Start()
    {
        if (texture != null)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/ExportedTexture.png", bytes);
            Debug.Log("Saved to " + Application.dataPath + "/ExportedTexture.png");
        }
    }
}
