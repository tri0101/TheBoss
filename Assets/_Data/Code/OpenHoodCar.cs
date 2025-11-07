using UnityEngine;
using System.Collections;

public class OpenHoodCar : MonoBehaviour
{
    [SerializeField] private Transform car;

    private Animator animator;
    private bool isOpen = false;       // trạng thái hiện tại (mở hay đóng)
    private bool isPlaying = false;    // animation đang chạy?
    [SerializeField] private Camera mainCam;
    private void Awake()
    {

        mainCam = Camera.main;
    }

    private void Start()
    {
        if (car != null)
        {
            animator = car.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("❌ Chưa gán Car vào script OpenHoodCar!");
        }
    }

    private void OnMouseOver()
    {
        // Raycast từ camera về hướng tâm màn hình (hoặc hướng chuột)
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (Input.GetMouseButtonDown(0) && !isPlaying)
            {
                if (!isOpen)
                {
                    StartCoroutine(PlayAnimationState("OpenHood", true));
                }
                else
                {
                    StartCoroutine(PlayAnimationState("CloseHood", false));
                }
            }

        }
            
    }

    private IEnumerator PlayAnimationState(string stateName, bool open)
    {
        isPlaying = true;

        // phát state
        animator.Play(stateName);

        // chờ hết state
        yield return null; // đợi 1 frame để Animator cập nhật
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // đợi đúng thời gian của animation
        yield return new WaitForSeconds(stateInfo.length);

        isOpen = open;
        isPlaying = false;
    }
}
