using UnityEngine;

public class NailRemove : MonoBehaviour
{
    [HideInInspector] public bool isRemoved = false;

    public void Run()
    {
        StartCoroutine(MoveNailOut());
    }

    private System.Collections.IEnumerator MoveNailOut()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(-0.15f, 0f, 0f);

        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Đánh dấu đã gỡ xong
        isRemoved = true;
    }
}
