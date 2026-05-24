using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 30f;

    [Header("Recoil Settings")]
    public float recoilSpeedOut = 40f;   // tốc độ giật nhanh
    public float recoilSpeedBack = 6f;   // tốc độ hồi chậm

    private Quaternion originalRotation;
    private Quaternion recoilRotationTarget;
    private bool isRecoiling = false;
    private bool hasRecoiled = false;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            StartRecoil();
        }

        HandleRecoil();
    }

    void Shoot()
    {
        if (bulletPrefab && bulletSpawnPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.SetActive(true);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
            }
        }
    }

    void StartRecoil()
    {
        // Tính rotation mục tiêu khi giật
        Vector3 recoilEuler = originalRotation.eulerAngles;
        recoilEuler.x += 10f;
        //recoilEuler.y += 30f;
        recoilEuler.z += 10f;
        recoilRotationTarget = Quaternion.Euler(recoilEuler);

        isRecoiling = true;
        hasRecoiled = false;
    }

    void HandleRecoil()
    {
        if (isRecoiling && !hasRecoiled)
        {
            // Giật nhanh
            transform.localRotation = Quaternion.Lerp(transform.localRotation, recoilRotationTarget, Time.deltaTime * recoilSpeedOut);

            if (Quaternion.Angle(transform.localRotation, recoilRotationTarget) < 0.5f)
            {
                hasRecoiled = true;
            }
        }
        else if (hasRecoiled)
        {
            // Hồi chậm
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * recoilSpeedBack);

            if (Quaternion.Angle(transform.localRotation, originalRotation) < 0.5f)
            {
                isRecoiling = false;
                hasRecoiled = false;
            }
        }
    }
}
