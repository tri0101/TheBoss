using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaptopUIManager : MonoBehaviour
{
    [SerializeField] private Transform noteUI;
    [SerializeField] private Transform waitPanel;
    [SerializeField] private Transform panel;
    [SerializeField] private Transform informText;

    [SerializeField] private TMP_InputField input;
    [SerializeField] private Button buttonLogin;

    private const string PASSWORD = "Winston10";

    private void Start()
    {
       

       
    }

    public void OpenNote()
    {
        if (noteUI != null)
        {
            bool isActive = noteUI.gameObject.activeSelf;
            noteUI.gameObject.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("NoteUI chưa được gán trong Inspector.");
        }
    }

    public void CheckPassword()
    {
        string enteredPassword = input.text;

        if (enteredPassword == PASSWORD)
        {
            waitPanel.gameObject.SetActive(false);
            panel.gameObject.SetActive(true);

            informText.gameObject.SetActive(false);
        }
        else
        {
            informText.gameObject.SetActive(true);
        }
    }
}