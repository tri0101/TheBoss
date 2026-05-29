using UnityEngine;

public class OpenNoteSafe : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject player;
    PlayerController playerController;
    private bool isNoteOpen = false;
    public bool IsNoteOpen => isNoteOpen;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();

    }
    public void Run()
    {

        player.GetComponent<PlayerController>().enabled = false;
        canvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();
        pond.CanvasButtonQCloseInvoice.gameObject.SetActive(true);
        isNoteOpen = true;

    }
    public void CloseNoteSafe()
    {
        canvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<PlayerController>().enabled = true;
        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();
        pond.CanvasButtonQCloseInvoice.gameObject.SetActive(false);
        isNoteOpen = false;
    }
    void Update()
    {

        if (isNoteOpen && Input.GetKeyDown(KeyCode.Q))
        {
            CloseNoteSafe();
        }
    }

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Run();
        }
    }
}
