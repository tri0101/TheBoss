using UnityEngine;

public class ObjectKinematicFalse : MonoBehaviour
{
    public void Run()
    {
        transform.tag = "Untagged";
        // Bỏ kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Nếu là Calendar thì sau 1.5 giây bật lại kinematic
        if (gameObject.name == "Calendar" && rb != null)
        {
            Invoke(nameof(SetKinematicTrue), 1.5f);
            return;
        }

        // Sau 3 giây thì biến mất
        Invoke(nameof(DestroySelf), 3f);
    }

    private void SetKinematicTrue()
    {
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void DestroySelf()
    {
        //Destroy(gameObject);
        transform.gameObject.SetActive(false);
    }
}
