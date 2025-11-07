using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [Header("----Audio Source----")]
    [SerializeField] private AudioSource musicSorce;
    [SerializeField] private AudioSource SFXSource;
    [Header("----Global Volume Settings----")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Header("----Audio Clip------")]
    public AudioClip walkOnWood;
    public AudioClip walkOnGrass;
    public AudioClip woodDoorOpen;
    public AudioClip drawerOpen;
    public AudioClip drawerClose;
    public AudioClip cabinetDoorOpen;
    public AudioClip cabinetDoorClose;
    public AudioClip slidingDoorOpen;
    public AudioClip slidingDoorClose;
    public AudioClip glassBreak;
    public AudioClip woodDoorClose;
    public AudioClip tranquilizerGunShot;
    public AudioClip prisonCellDoor;
    public AudioClip knobRotate;
    public AudioClip crateBreak;
    public AudioClip garageDoor;
    public AudioClip tireSound;
    public AudioClip setUpMotor;
    public AudioClip carRunning;
    public AudioClip diePlayerSound;
    public AudioClip punchSound;
    public AudioClip dogSound;
    public AudioClip stickSound;

    public AudioClip tikSound;
    public AudioClip tokSound;
    public AudioClip outdoorSound;
    public AudioClip elec;
    public AudioClip fire;
    public AudioClip keypadClick;
    public AudioClip keypadDenied;
    public AudioClip keypadAccess;
    public AudioClip buttonClick;
    public AudioClip buttonClickPlay;

   
    public AudioClip angryManSound;
    public AudioClip ringSound;
    private Dictionary<string, AudioClip> clipDict;

    [SerializeField] private Transform player;
    private bool isOutDoor = false;
    private bool isMusicPlaying = false; // để tránh gọi lại liên tục
    private float currentMusicVolume;

    public AudioClip whatthenoise;
    public AudioClip whocoming;
    [Header("Old Man Voice Lines")]
    public List<AudioClip> oldManVoice = new List<AudioClip>();
    [Header("----Music Clip------")]
     public AudioClip tensionMusic;
    /// <summary>
    /// Phát SFX luôn nghe được (2D).
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        SFXSource.PlayOneShot(clip, volume * sfxVolume);
    }
    public void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // 🟩 Tạo GameObject tạm để phát âm thanh
        GameObject temp = new GameObject("TempAudio2D");
        temp.transform.SetParent(listTempAudio, true);

        // 🟦 Thêm AudioSource và cấu hình 2D
        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * sfxVolume;
        source.spatialBlend = 0f; // 0 = âm thanh 2D
        source.Play();

        // 🟥 Xoá object sau khi âm thanh phát xong
        Destroy(temp, clip.length);
    }

    public static AudioManager instance; // instance toàn cục

    [SerializeField] private Transform listTempAudio;
    private void Awake()
    {
        // Nếu chưa có instance thì gán
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // giữ lại khi đổi scene
        }
        else
        {
            Destroy(gameObject); // tránh trùng instance
        }
        InitializeClipDictionary();
    }
    private void Update()
    {
        if (player != null)
        {
            if (isOutDoor)
            {
                PlayMusic(outdoorSound, 0.7f);
                
            }
            else
            {
                StopMusic();
            }
        }
    }
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // Nếu chưa phát hoặc đang phát nhạc khác
        if (!isMusicPlaying || musicSorce.clip != clip)
        {
            currentMusicVolume = volume;
            musicSorce.clip = clip;
            musicSorce.volume = musicVolume * volume;
            musicSorce.loop = true;
            musicSorce.Play();
            isMusicPlaying = true;
        }
    }

    public void StopMusic()
    {
        if (isMusicPlaying)
        {
            musicSorce.Stop();
            isMusicPlaying = false;
            currentMusicVolume = 1f;
        }
    }
    public void isOutdoor()
    {
        isOutDoor = true;
    }
    public void isNotOutdoor()
    {
        isOutDoor = false;
    }
    private void InitializeClipDictionary()
    {
        clipDict = new Dictionary<string, AudioClip>()
        {
            { "walkOnWood", walkOnWood },
            { "walkOnGrass", walkOnGrass },
            { "woodDoorOpen", woodDoorOpen },
            { "drawerOpen", drawerOpen },
            { "drawerClose", drawerClose },
            { "cabinetDoorOpen", cabinetDoorOpen },
            { "cabinetDoorClose", cabinetDoorClose },
            { "slidingDoorOpen", slidingDoorOpen },
            { "slidingDoorClose", slidingDoorClose },
            { "glassBreak", glassBreak },
            { "woodDoorClose", woodDoorClose },
            { "tranquilizerGunShot", tranquilizerGunShot },
            { "prisonCellDoor", prisonCellDoor },
            { "knobRotate", knobRotate },
            { "crateBreak", crateBreak },
            { "garageDoor", garageDoor },
            { "tireSound", tireSound },
            { "setUpMotor", setUpMotor }
        };
    }
    /// <summary>
    /// Phát SFX tại vị trí trong không gian (3D).
    /// </summary>
    public void PlaySFXAtPosition(AudioClip clip, Vector3 pos, float minDist = 2f, float maxDist = 15f, float volume = 1f)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = pos;
        temp.transform.SetParent(listTempAudio, true);
        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * sfxVolume;
        source.spatialBlend = 1f; // 3D
        source.rolloffMode = AudioRolloffMode.Custom;

        // Tạo custom rolloff để nghe rõ ở gần và tắt nhanh khi xa
        AnimationCurve rolloffCurve = new AnimationCurve();
        rolloffCurve.AddKey(0f, 1f);   // 0m = volume 1
        rolloffCurve.AddKey(0.25f, 0.8f);
        rolloffCurve.AddKey(0.5f, 0.3f);
        rolloffCurve.AddKey(1f, 0f);   // maxDist = volume 0

        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, rolloffCurve);
        source.minDistance = minDist;
        source.maxDistance = maxDist;

        source.Play();
        Destroy(temp, clip.length);
    }
    public AudioSource PlayLoopSFXAtParent(AudioClip clip, Transform parent, float minDist = 2f, float maxDist = 15f, float volume = 1f)
    {
        if (clip == null || parent == null) return null;

        // Tạo gameobject tạm để phát âm thanh
        GameObject temp = new GameObject("TempLoopAudio");
        temp.transform.SetParent(parent, false); // làm con của transform truyền vào
        temp.transform.localPosition = Vector3.zero;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * sfxVolume;
        source.spatialBlend = 1f; // 3D
        source.loop = true;       // lặp lại âm thanh
        source.rolloffMode = AudioRolloffMode.Custom;

        // Custom rolloff để giảm âm dần theo khoảng cách
        AnimationCurve rolloffCurve = new AnimationCurve();
        rolloffCurve.AddKey(0f, 1f);
        rolloffCurve.AddKey(0.25f, 0.8f);
        rolloffCurve.AddKey(0.5f, 0.3f);
        rolloffCurve.AddKey(1f, 0f);

        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, rolloffCurve);
        source.minDistance = minDist;
        source.maxDistance = maxDist;

        source.Play();
        return source; // trả về AudioSource để sau này dừng thủ công
    }

    public GameObject PlaySFXAtPositionObject(AudioClip clip, Vector3 pos, float minDist = 2f, float maxDist = 15f, float volume = 1f)
    {
        if (clip == null) return null;

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = pos;
     

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f; // 3D
        source.rolloffMode = AudioRolloffMode.Custom;

        // Tạo custom rolloff để nghe rõ ở gần và tắt nhanh khi xa
        AnimationCurve rolloffCurve = new AnimationCurve();
        rolloffCurve.AddKey(0f, 1f);   // 0m = volume 1
        rolloffCurve.AddKey(0.25f, 0.8f);
        rolloffCurve.AddKey(0.5f, 0.3f);
        rolloffCurve.AddKey(1f, 0f);   // maxDist = volume 0

        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, rolloffCurve);
        source.minDistance = minDist;
        source.maxDistance = maxDist;

        source.Play();
        Destroy(temp, clip.length);
        return temp;
    }
    public void PlaySFXAtPositionByString(string nameClip, Vector3 pos, float minDist = 2f, float maxDist = 15f, float volume = 1f)
    {
        AudioClip ad = GetClipByName(nameClip);

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = pos;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = ad;
        source.volume = volume;
        source.spatialBlend = 1f; // 3D
        source.rolloffMode = AudioRolloffMode.Custom;

        // Tạo custom rolloff để nghe rõ ở gần và tắt nhanh khi xa
        AnimationCurve rolloffCurve = new AnimationCurve();
        rolloffCurve.AddKey(0f, 1f);   // 0m = volume 1
        rolloffCurve.AddKey(0.25f, 0.8f);
        rolloffCurve.AddKey(0.5f, 0.3f);
        rolloffCurve.AddKey(1f, 0f);   // maxDist = volume 0

        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, rolloffCurve);
        source.minDistance = minDist;
        source.maxDistance = maxDist;

        source.Play();
        Destroy(temp, ad.length);
    }
    public AudioClip GetClipByName(string clipName)
    {
        if (clipDict.TryGetValue(clipName, out AudioClip clip))
            return clip;

        Debug.LogWarning($"Không tìm thấy AudioClip: {clipName}");
        return null;
    }
    public void getPlayer(Transform player)
    {
        this.player = player;
    }
    public void ChangeVolumeMusic()
    {
        
        musicSorce.volume = musicVolume * currentMusicVolume;
    }
    public void PlayOldManVoice(Vector3 pos, float minDist = 2f, float maxDist = 15f, float volume = 1f)
    {
        if (oldManVoice == null || oldManVoice.Count == 0) return;

        // Chọn ngẫu nhiên 1 đoạn voice
        int randomIndex = Random.Range(0, oldManVoice.Count);
        AudioClip randomClip = oldManVoice[randomIndex];

        // Dùng lại logic phát âm thanh 3D
        PlaySFXAtPosition(randomClip, pos, minDist, maxDist, volume);
    }
    public AudioClip GetAudioOldMan()
    {
        if (oldManVoice == null || oldManVoice.Count == 0) return null;

        // Chọn ngẫu nhiên 1 đoạn voice
        int randomIndex = Random.Range(0, oldManVoice.Count);
        AudioClip randomClip = oldManVoice[randomIndex];
        return randomClip;
    }

    // ==================== TENSION MUSIC ====================

    private Coroutine fadeOutCoroutine;

    /// <summary>
    /// Phát nhạc tension bằng AudioClip, có thể chỉnh âm lượng và lặp lại.
    /// </summary>
    public void CallTensionMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("Không có AudioClip để phát tension music!");
            return;
        }

        // Hủy fade-out cũ nếu đang chạy
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }

        // Nếu đang phát nhạc khác hoặc chưa phát
        if (!isMusicPlaying || musicSorce.clip != clip)
        {
            musicSorce.clip = clip;
            musicSorce.volume = musicVolume * volume;
            musicSorce.loop = true;
            musicSorce.Play();

            isMusicPlaying = true;
            currentMusicVolume = volume;
        }
    }

    /// <summary>
    /// Dừng tension music bằng hiệu ứng giảm dần âm lượng.
    /// </summary>
    public void StopTensionMusicSmooth(float fadeDuration = 2f)
    {
        if (!isMusicPlaying || musicSorce.clip == null) return;

        if (fadeOutCoroutine != null)
            StopCoroutine(fadeOutCoroutine);

        fadeOutCoroutine = StartCoroutine(FadeOutMusic(fadeDuration));
    }

    /// <summary>
    /// Coroutine giảm âm lượng dần rồi mới tắt nhạc.
    /// </summary>
    //private IEnumerator FadeOutMusic(float duration)
    //{
    //    float startVolume = musicSorce.volume;
    //    float time = 0f;

    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        musicSorce.volume = Mathf.Lerp(startVolume, 0f, time / duration);
    //        yield return null;
    //    }

    //    musicSorce.Stop();
    //    musicSorce.volume = startVolume; // reset lại cho lần sau
    //    isMusicPlaying = false;
    //    fadeOutCoroutine = null;
    //}
    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSorce.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSorce.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        musicSorce.Stop();
        musicSorce.clip = null;  // đảm bảo phát lại được lần sau
        isMusicPlaying = false;  // reset flag
        musicSorce.volume = startVolume;
        fadeOutCoroutine = null;
    }

}
