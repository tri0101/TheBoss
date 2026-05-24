using UnityEngine;
using System.Collections;

public class RopeAndBoardController : MonoBehaviour
{
    [SerializeField] private Transform rope;    // Chỉ 1 dây thừng
    [SerializeField] private Transform board;   // Tấm ván

    [SerializeField] private float ropeMoveTime = 1f;    // Thời gian dây chạy
    [SerializeField] private float boardMoveTime = 1f;   // Thời gian ván chạy

    private bool isOpen = false; // Trạng thái ban đầu: đóng

    private float ropeMaxScaleY = 1f;
    private float ropeTopLocalY = 12.02f; // Luôn giữ cố định local Y

    private void Start()
    {
        // Đặt dây scale = 0 (đóng)
        rope.localScale = new Vector3(rope.localScale.x, 0f, rope.localScale.z);

        // Giữ nguyên vị trí đỉnh
        rope.localPosition = new Vector3(rope.localPosition.x, ropeTopLocalY, rope.localPosition.z);

        // Đảm bảo ván đóng
        board.localPosition = new Vector3(board.localPosition.x, 0f, board.localPosition.z);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F) && !isOpen)
        //{
        //    isOpen = true;
        //    Run();
        //}
       if (Input.GetKeyDown(KeyCode.G) && isOpen)
        {
            isOpen = false;
            Run();
        }
    }
    public void RunOpen()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(AnimateRope());
        StartCoroutine(AnimateBoard());
    }

    public void Run()
    {
        StartCoroutine(AnimateRope());
        StartCoroutine(AnimateBoard());
    }

    private IEnumerator AnimateRope()
    {
        float elapsed = 0f;

        float startScaleY = isOpen ? 0f : ropeMaxScaleY;
        float endScaleY = isOpen ? ropeMaxScaleY : 0f;

        while (elapsed < ropeMoveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / ropeMoveTime);

            float currentScaleY = Mathf.Lerp(startScaleY, endScaleY, t);
            rope.localScale = new Vector3(rope.localScale.x, currentScaleY, rope.localScale.z);

            // Không thay đổi vị trí vì pivot nằm ở trên
            rope.localPosition = new Vector3(rope.localPosition.x, ropeTopLocalY, rope.localPosition.z);

            yield return null;
        }

        // Đảm bảo giá trị cuối cùng chính xác
        rope.localScale = new Vector3(rope.localScale.x, endScaleY, rope.localScale.z);
        rope.localPosition = new Vector3(rope.localPosition.x, ropeTopLocalY, rope.localPosition.z);
    }

    private IEnumerator AnimateBoard()
    {
        float elapsed = 0f;

        Vector3 boardStart = board.localPosition;
        Vector3 boardEnd = new Vector3(boardStart.x, isOpen ? -4f : 0f, boardStart.z);

        while (elapsed < boardMoveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / boardMoveTime);
            board.localPosition = Vector3.Lerp(boardStart, boardEnd, t);
            yield return null;
        }

        board.localPosition = boardEnd;
    }
}
