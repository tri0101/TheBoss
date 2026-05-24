using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PauseMenu : MonoBehaviour
{
    [Header("UI - Panels & Sliders")]
    [SerializeField] private GameObject canvasPanel;

    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;
    [SerializeField] private PlayerController playerController;

    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxText;
    [SerializeField] private GameObject audioManager; // GameObject chứa AudioSource

    [Header("General")]
    public bool isDisabled = false;
    private bool isPaused = false;
    [Header("Other script")]
    [SerializeField] private OpenLaptop openLaptop;
    // Cấu hình giới hạn
    private const float stepSensitivity = 0.1f;
    private const float maxSensitivity = 1.5f;
    private const float baseSensitivity = 4f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Start()
    {
        // Ẩn menu pause lúc đầu
        if (canvasPanel != null)
            canvasPanel.SetActive(false);

        // Lấy AudioSource trong AudioManager
        if (audioManager != null)
        {
            AudioSource[] sources = audioManager.GetComponentsInChildren<AudioSource>(true);
            foreach (var src in sources)
            {
                if (src.name.ToLower().Contains("music"))
                    musicSource = src;
                else if (src.name.ToLower().Contains("sfx"))
                    sfxSource = src;
            }
        }

        // --- 🎧 Đồng bộ từ AudioManager ---
        float currentMusic = 100f;
        float currentSfx = 100f;

        if (AudioManager.instance != null)
        {
            currentMusic = AudioManager.instance.musicVolume * 100f;
            currentSfx = AudioManager.instance.sfxVolume * 100f;

            if (musicSource != null)
                musicSource.volume = AudioManager.instance.musicVolume;
            if (sfxSource != null)
                sfxSource.volume = AudioManager.instance.sfxVolume;
        }
        else
        {
            if (musicSource != null) currentMusic = musicSource.volume * 100f;
            if (sfxSource != null) currentSfx = sfxSource.volume * 100f;
        }

        // --- 🎮 Cấu hình thanh Sensitivity ---
        sensitivitySlider.minValue = 0.1f;
        sensitivitySlider.maxValue = maxSensitivity;
        sensitivitySlider.wholeNumbers = false;

        float currentSens = playerController != null ? playerController.GetCurrentSensitivity() / baseSensitivity : 1f;
        sensitivitySlider.value = Mathf.Clamp(currentSens, 0.1f, maxSensitivity);
        UpdateSensitivityText(sensitivitySlider.value);
        sensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

        // --- 🔊 Music ---
        musicSlider.minValue = 0f;
        musicSlider.maxValue = 100f;
        musicSlider.wholeNumbers = true;
        musicSlider.value = currentMusic;
        UpdateMusicText(currentMusic);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        // --- 🎵 SFX ---
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 100f;
        sfxSlider.wholeNumbers = true;
        sfxSlider.value = currentSfx;
        UpdateSFXText(currentSfx);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Ẩn chuột khi bắt đầu game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isDisabled) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Continue();
            else Pause();
        }
    }

    // 🕹️ PAUSE / CONTINUE
    public void Pause()
    {
        if (canvasPanel != null)
            canvasPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continue()
    {
        AudioManager.instance.PlaySFX2D(AudioManager.instance.buttonClick, 1f);
        if (canvasPanel != null)
            canvasPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        if (openLaptop.IsLaptopOpen) return;
        if (playerController != null)
            playerController.enabled = true;
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void MainMenu()
    {
        //AudioManager.instance.PlaySFX2D(AudioManager.instance.buttonClick, 1f);
        AudioManager.instance.StopTensionMusicSmooth(0.5f);
        LoadingScene.instance.LoadMenuScene();
    }
    private IEnumerator PlayThenDo(System.Action action)
    {
        // 🔊 Phát âm thanh click
        AudioManager.instance.PlaySFX2D(AudioManager.instance.buttonClickPlay, 0.5f);

        // ⏱ Chờ 0.5 giây
        yield return new WaitForSeconds(0.5f);

        // ▶ Thực hiện hành động sau khi âm thanh phát xong
        action?.Invoke();
    }

    // 🖱️ Mouse Sensitivity
    private void OnMouseSensitivityChanged(float value)
    {
        float snappedValue = Mathf.Round(value / stepSensitivity) * stepSensitivity;
        sensitivitySlider.SetValueWithoutNotify(snappedValue);

        if (playerController != null)
            playerController.SetMouseSensitivity(snappedValue);

        UpdateSensitivityText(snappedValue);
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
            sensitivityText.text = value.ToString("0.0");
    }

    // 🎧 Music Volume
    private void OnMusicVolumeChanged(float value)
    {
        UpdateMusicText(value);
        float normalized = value / 100f;

        if (musicSource != null)
            musicSource.volume = normalized;

        if (AudioManager.instance != null)
            AudioManager.instance.musicVolume = normalized;
        AudioManager.instance.ChangeVolumeMusic();
    }

    private void UpdateMusicText(float value)
    {
        if (musicText != null)
            musicText.text = value.ToString("0");
    }

    // 🔊 SFX Volume
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
        if (sfxText != null)
            sfxText.text = value.ToString("0");
    }
}
