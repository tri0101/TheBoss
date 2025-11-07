using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [Header("UI - Panels & Sliders")]
    [SerializeField] private GameObject canvasPanel;

    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxText;
    [SerializeField] private GameObject audioManager; // GameObject chứa AudioSource

    private AudioSource musicSource;
    private AudioSource sfxSource;
    //private void Awake()
    //{
    //    AudioManager.instance.sfxVolume = 1f;
    //    AudioManager.instance.musicVolume = 1f;
    //}
    void Start()
    {
        // 🔊 Lấy các AudioSource từ AudioManager
        if (audioManager != null)
        {
            AudioSource[] sources = audioManager.GetComponentsInChildren<AudioSource>();
            foreach (var src in sources)
            {
                if (src.name.ToLower().Contains("music"))
                    musicSource = src;
                else if (src.name.ToLower().Contains("sfx"))
                    sfxSource = src;
            }
        }

        // 🎵 Thiết lập thanh Music
        musicSlider.minValue = 0f;
        musicSlider.maxValue = 100f;
        musicSlider.wholeNumbers = true;

        // 🔸 Lấy giá trị thật từ AudioManager (thay vì mặc định 100%)
        float currentMusic = 100f;
        if (AudioManager.instance != null)
            currentMusic = AudioManager.instance.musicVolume * 100f;
        else if (musicSource != null)
            currentMusic = musicSource.volume * 100f;

        musicSlider.value = currentMusic;
        UpdateMusicText(currentMusic);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        // 🔊 Thiết lập thanh SFX
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 100f;
        sfxSlider.wholeNumbers = true;

        // 🔸 Lấy giá trị thật từ AudioManager (thay vì mặc định 100%)
        float currentSfx = 100f;
        if (AudioManager.instance != null)
            currentSfx = AudioManager.instance.sfxVolume * 100f;
        else if (sfxSource != null)
            currentSfx = sfxSource.volume * 100f;

        sfxSlider.value = currentSfx;
        UpdateSFXText(currentSfx);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    // === MUSIC ===
    private void OnMusicVolumeChanged(float value)
    {
        UpdateMusicText(value);
        float normalized = value / 100f;

        if (musicSource != null)
            musicSource.volume = normalized;

        if (AudioManager.instance != null)
            AudioManager.instance.musicVolume = normalized;
    }

    private void UpdateMusicText(float value)
    {
        musicText.text = value.ToString("0");
    }

    // === SFX ===
    private void OnSFXVolumeChanged(float value)
    {
        UpdateSFXText(value);
        float normalized = value / 100f;

        if (sfxSource != null)
            sfxSource.volume = normalized;

        if (AudioManager.instance != null)
            AudioManager.instance.sfxVolume = normalized;
    }

    private void UpdateSFXText(float value)
    {
        sfxText.text = value.ToString("0");
    }
}
