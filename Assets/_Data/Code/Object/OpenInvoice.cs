using UnityEngine;

public class OpenInvoice : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject player;
    PlayerController playerController;
    private bool isInvoiceOpen = false;
    public bool IsInvoiceOpen => isInvoiceOpen;

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
            isInvoiceOpen = true;

    }
    public void CloseLaptop()
    {
        canvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<PlayerController>().enabled = true;
        PlayerObjectNameDisplay pond = player.GetComponent<PlayerObjectNameDisplay>();
        pond.CanvasButtonQCloseInvoice.gameObject.SetActive(false);
        isInvoiceOpen = false;
    }
    void Update()
    {

        if (isInvoiceOpen && Input.GetKeyDown(KeyCode.Q))
        {
            CloseLaptop();
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
