using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartingPlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraHead;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform canvasOver;

    private Animator animator;
    private PlayerController playerController;
    private DisableChildColliders dCC;

    private void Start()
    {
        ReturnPlay();
    }

    public void ReturnPlay()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        dCC = GetComponent<DisableChildColliders>();
        dCC.EnableAllChildColliders();

        if (cameraHead != null)
            cameraHead.gameObject.SetActive(true);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        animator = GetComponent<Animator>();

        if (animator != null)
            StartCoroutine(PlayStandUp());

        StartCoroutine(SwitchToMainCamera());
    }

    private IEnumerator PlayStandUp()
    {
        yield return null;
        if (animator != null)
            animator.SetTrigger("isDefault");
    }

    private IEnumerator SwitchToMainCamera()
    {
        yield return new WaitForSeconds(4.5f);

        Collider col = GetComponent<Collider>();
        col.enabled = true;

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        if (cameraHead != null)
            cameraHead.gameObject.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;
        //Vector3 pos = transform.parent.localPosition;
        //pos.x -= 1.5f;
        //transform.localPosition = pos;
        dCC.DisableAllChildColliders();
    }

    // -------------------- HÀM GAME OVER --------------------
    public void GameOver()
    {
        // Hiển thị canvas Over
        if (canvasOver != null)
        {
            canvasOver.gameObject.SetActive(true);
            Transform parentCanvas = canvasOver.parent.parent;
            Transform parentCanvasTrue = canvasOver.parent.parent.parent;
            canvasOver.parent.SetParent(parentCanvasTrue);
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(parentCanvasTrue, false);
            parentCanvas.gameObject.SetActive(false);

        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //// Chờ 3 giây
        //yield return new WaitForSeconds(3f);

        //// Ẩn canvas Over
        //if (canvasOver != null)
        //    canvasOver.gameObject.SetActive(false);
        //LoadingScene.instance.LoadMenuScene();

    }

    //private IEnumerator GameOverCoroutine()
    //{

    //    // Hiển thị canvas Over
    //    if (canvasOver != null)
    //    {
    //        canvasOver.gameObject.SetActive(true);
    //        Transform parentCanvas = canvasOver.parent.parent;
    //        canvasOver.parent.SetParent(null);
    //        parentCanvas.gameObject.SetActive(false);

    //    }

    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.None;

    //    //// Chờ 3 giây
    //    //yield return new WaitForSeconds(3f);

    //    //// Ẩn canvas Over
    //    //if (canvasOver != null)
    //    //    canvasOver.gameObject.SetActive(false);
    //    //LoadingScene.instance.LoadMenuScene();


    //}
}
