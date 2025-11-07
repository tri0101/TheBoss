using UnityEngine;

public class LaptopUIManager : MonoBehaviour
{
    [SerializeField] private Transform noteUI;
    [SerializeField] private Transform terminalUI;

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
    public void OpenTer()
    {
        if (terminalUI != null)
        {
            bool isActive = terminalUI.gameObject.activeSelf;
            terminalUI.gameObject.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("terUI chưa được gán trong Inspector.");
        }
    }
}
