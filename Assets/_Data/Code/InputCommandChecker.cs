using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class TerminalManager : MonoBehaviour
{
    [Header("Prefab")]
    public TMP_InputField inputPrefab;

    [Header("Container")]
    public RectTransform terminalContent;

    private float currentY = 125f;
    private TerminalState state = TerminalState.Idle;

    private string sshTarget = "admin@secretdoor.local";
    private string expectedPassword = "hunter2";
    private bool isLoggedIn = false;
    [Header("Scroll")]
    public ScrollRect terminalScroll;
    enum TerminalState
    {
        Idle,
        Busy,
        AwaitingPassword,
        LoggedIn,
        Locked
    }
    [SerializeField] private Transform openDoor;

    //void Start()
    //{
    //    SpawnInput(); // khởi tạo dòng đầu tiên
    //}
    void OnEnable()
    {
        // Xóa tất cả các dòng input/output cũ (Clone)
        foreach (Transform child in terminalContent)
        {
            if (child.name.Contains("(Clone)"))
            {
                Destroy(child.gameObject);
            }
        }

        currentY = 125f; // reset lại vị trí Y nếu cần
        state = TerminalState.Idle;
        isLoggedIn = false;

        SpawnInput(); // tạo dòng input mới
    }

    void SpawnInput()
    {
        var input = Instantiate(inputPrefab, terminalContent);
        input.gameObject.SetActive(true); // đảm bảo active
        var rt = input.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, currentY);
        input.interactable = true;
        input.readOnly = false;
        input.onEndEdit.AddListener(delegate { HandleInput(input); });
        input.ActivateInputField();
        ScrollToBottom();
    }

    void AddOutput(string message)
    {
        currentY -= 50f;
        var output = Instantiate(inputPrefab, terminalContent);
        output.gameObject.SetActive(true); // đảm bảo active
        var rt = output.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, currentY);
        output.text = message;
        output.interactable = false;
        output.readOnly = true;
        ScrollToBottom();
    }

    void HandleInput(TMP_InputField inputField)
    {
        string input = inputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        inputField.interactable = false;
        inputField.readOnly = true;

        AddOutput($"> {input}");

        switch (state)
        {
            case TerminalState.Idle:
                if (input == $"ssh {sshTarget}")
                {
                    AddOutput($"{sshTarget}'s password:");
                    state = TerminalState.AwaitingPassword;
                }
                else
                {
                    AddOutput("Unknown command.");
                }
                break;

            case TerminalState.AwaitingPassword:
                if (input == expectedPassword)
                {
                    isLoggedIn = true;
                    state = TerminalState.LoggedIn;
                    AddOutput("Access granted.");
                    AddOutput("Welcome to SecretDoor System v1.3");
                    AddOutput("Type 'help' to see available commands");
                }
                else
                {
                    AddOutput("Wrong password. Try again.");
                    state = TerminalState.Idle;
                }
                break;

            case TerminalState.LoggedIn:
                if (input == "help")
                {
                    AddOutput("Available commands:");
                    AddOutput("- open_door   : Open the hidden door");
                    AddOutput("- status      : Show current door status");
                    AddOutput("- exit        : Disconnect from system");
                }
                else if (input == "open_door")
                {
                   
                    StartCoroutine(OpenDoorSequence());
                    return; // không spawn input nữa
                }
                else if (input == "status")
                {
                    AddOutput("Door status: CLOSED");
                }
                else if (input == "exit")
                {
                    AddOutput($"Disconnected from {sshTarget}");
                    state = TerminalState.Idle;
                }
                else
                {
                    AddOutput("Unknown command. Type 'help' for available commands.");
                }
                break;

            case TerminalState.Locked:
                AddOutput("[SYSTEM LOCKED] No further input allowed.");
                return;
        }

        SpawnInput(); // dòng tiếp theo cho người chơi
    }
    IEnumerator OpenDoorSequence()
    {
        state = TerminalState.Busy;// trạng thái riêng để tránh nhập

        AddOutput("[INFO] Verifying user privileges...");
        yield return new WaitForSeconds(1f);

        AddOutput("[OK] User verified");
        yield return new WaitForSeconds(1f);

        AddOutput("[INFO] Sending unlock signal to actuator...");
        yield return new WaitForSeconds(1f);

        AddOutput("[OK] Door successfully unlocked for 5 seconds.");

        state = TerminalState.Locked;
        if (openDoor != null)
        {
            RopeAndBoardController controller = openDoor.GetComponent<RopeAndBoardController>();
            if (controller != null)
            {
                controller.RunOpen();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy RopeAndBoardController trong openDoor.");
            }
        }
    }
    void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // đảm bảo UI cập nhật đúng trước khi scroll
        terminalScroll.verticalNormalizedPosition = 0f;
    }
}
