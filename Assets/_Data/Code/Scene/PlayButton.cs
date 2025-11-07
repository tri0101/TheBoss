using UnityEngine;
using System.Collections;
public class PlayButton : MonoBehaviour
{
    [SerializeField] private Transform canvasSetting;
    [SerializeField] private Transform canvasCredit;
    private bool settingCanvasOpen = false;
    private bool creditCanvasOpen = false;

    private void Awake()
    {
        canvasCredit.gameObject.SetActive(false);
        canvasSetting.gameObject.SetActive(false);
    }

    public void OnPlayClick()
    {
        StartCoroutine(PlayThenDo(() =>
        {
            LoadingScene.instance.LoadGame();
        }));
    }

    public void QuitGame()
    {
        StartCoroutine(PlayThenDo(() =>
        {
            Application.Quit();
        }));
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
    public void SettingCanvas()
    {
        AudioManager.instance.PlaySFX2D(AudioManager.instance.buttonClick, 1f);
        // Nếu đang mở thì tắt
        if (settingCanvasOpen)
        {
            canvasSetting.gameObject.SetActive(false);
            settingCanvasOpen = false;
        }
        else // Nếu đang tắt thì bật
        {
            canvasSetting.gameObject.SetActive(true);
            canvasCredit.gameObject.SetActive(false);
            settingCanvasOpen = true;
            creditCanvasOpen = false;
        }
    }

    public void CreditCanvas()
    {
        // Nếu đang mở thì tắt
        if (creditCanvasOpen)
        {
            canvasCredit.gameObject.SetActive(false);
            creditCanvasOpen = false;
        }
        else // Nếu đang tắt thì bật
        {
            canvasCredit.gameObject.SetActive(true);
            canvasSetting.gameObject.SetActive(false);
            creditCanvasOpen = true;
            settingCanvasOpen = false;
        }
    }
}
