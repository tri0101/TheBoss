using UnityEngine;

public class WolfCollider : MonoBehaviour
{
    WolfController wc;
    private void Awake()
    {
        wc = transform.parent.GetComponent<WolfController>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (wc.getDead()) return;
        if (collision != null && collision.transform.name == "Tranquilizer dart")

        {
            DartControl dc = collision.transform.GetComponent<DartControl>();
            if (!dc.GetShot()) return;
            
            
            wc.Die();
            wc.CallAfterDie(collision.transform);
        }
    }
}
