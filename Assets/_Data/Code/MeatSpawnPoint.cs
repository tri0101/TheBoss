using UnityEngine;

public class MeatSpawnPoint : MonoBehaviour
{
    [SerializeField] private Transform meatCopy; // prefab để spawn
    [SerializeField] private float spawnInterval = 10f;

    private GameObject currentMeat; // tham chiếu tới meat đang tồn tại
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnMeat();
        }
    }

    private void TrySpawnMeat()
    {
        // Chỉ spawn nếu chưa có meat hoặc object đó đã biến mất
        if (currentMeat == null)
        {
            
            currentMeat = Instantiate(meatCopy.gameObject, transform.position, Quaternion.identity);
            currentMeat.transform.localRotation = Quaternion.Euler(90, 0, 0);
            currentMeat.transform.SetParent(transform, true);
            currentMeat.gameObject.SetActive(true);
        }
    }
}
