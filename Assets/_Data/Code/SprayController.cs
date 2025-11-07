using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SprayController : MonoBehaviour
{
    public ParticleSystem smokeParticle;
    [SerializeField] private Transform holdContainer;

    [Header("Collider cần bật/tắt")]
    public Transform targetTransform; // Transform chứa BoxCollider
    private BoxCollider targetCollider;

    [Header("Âm thanh spray")]
    [SerializeField] private AudioClip spraySound;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        // Chuẩn bị audio
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;       // để loop khi giữ
        audioSource.playOnAwake = false;

        // Chuẩn bị collider
        if (targetTransform != null)
        {
            targetCollider = targetTransform.GetComponent<BoxCollider>();
            if (targetCollider == null)
                Debug.LogWarning("Không tìm thấy BoxCollider trên targetTransform!");
            else
                targetCollider.enabled = false; // tắt ban đầu
        }
    }

    void Update()
    {
        audioSource.volume = AudioManager.instance.sfxVolume;
        // Kiểm tra holdContainer có con đầu tiên không và tên có phải "Ex" không
        bool holdingEx = holdContainer.childCount > 0 && holdContainer.GetChild(0).name == "Ex";

        if (holdingEx)
        {
            if (Input.GetMouseButton(1)) // giữ chuột trái
            {
                if (!smokeParticle.isPlaying)
                    smokeParticle.Play(); // bật particle

                if (targetCollider != null)
                    targetCollider.enabled = true; // bật collider

                if (spraySound != null && !audioSource.isPlaying)
                {
                    audioSource.clip = spraySound;
                    audioSource.Play();
                }
            }
            else // nhả chuột
            {
                if (smokeParticle.isPlaying)
                    smokeParticle.Stop();

                if (targetCollider != null)
                    targetCollider.enabled = false;

                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
        else
        {
            // Nếu không cầm đúng object thì tắt hết
            if (smokeParticle.isPlaying)
                smokeParticle.Stop();

            if (targetCollider != null)
                targetCollider.enabled = false;

            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
