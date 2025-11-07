using UnityEngine;

public class DisableChildColliders : MonoBehaviour
{
    //void Start()
    //{
    //    DisableAllChildColliders();
    //}

    public void DisableAllChildColliders()
    {
        // Lấy collider của chính object cha (nơi script này được gắn)
        Collider parentCollider = GetComponent<Collider>();

        // Lấy tất cả collider trong cha + con
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in allColliders)
        {
            // Bỏ qua collider của object cha
            if (col == parentCollider) continue;

            // Tắt collider con
            col.enabled = false;
        }
    }
    public void EnableAllChildColliders()
    {
        // Lấy collider của chính object cha (nơi script này được gắn)
        Collider parentCollider = GetComponent<Collider>();

        // Lấy tất cả collider trong cha + con
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in allColliders)
        {
            // Bỏ qua collider của object cha
            if (col == parentCollider) continue;

            // Tắt collider con
            col.enabled = true;
        }
    }
}
