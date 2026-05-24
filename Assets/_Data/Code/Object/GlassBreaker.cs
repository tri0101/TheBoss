using UnityEngine;

public class GlassBreaker : MonoBehaviour
{
    [SerializeField] private Transform brokenGlass; // Prefab kính vỡ
   
    public void Run()
    {
        if (brokenGlass != null)
        {
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.glassBreak, transform.position);
            GameObject newBroken = Instantiate(brokenGlass.gameObject, transform.position, transform.rotation);
            newBroken.SetActive(true);
            Destroy(newBroken, 1f);
        }

        // Ẩn object hiện tại (kính nguyên)
        gameObject.SetActive(false);
    }
}
