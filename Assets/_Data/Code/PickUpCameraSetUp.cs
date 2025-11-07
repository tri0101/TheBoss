using UnityEngine;

public class PickUpCameraSetup : MonoBehaviour
{
    public Camera cam; // gắn Camera trong Inspector
    public string layerName = "holdLayer";

    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        if (cam != null)
        {
            // Clear Flags = Depth only
            cam.clearFlags = CameraClearFlags.Depth;

            // Culling Mask = layer "holdLayer"
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1) // tồn tại layer
            {
                cam.cullingMask = 1 << layer;
            }

            // Clipping Planes
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 1000f;

            // Depth
            cam.depth = 1;
        }
    }
}
