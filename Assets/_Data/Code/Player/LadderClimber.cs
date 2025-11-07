using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LadderClimber : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 3f;

    private Rigidbody rb;
    private bool isClimbing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isClimbing)
        {
            float moveY = 0f;

            if (Input.GetKey(KeyCode.W))
                moveY = climbSpeed;

            rb.linearVelocity = new Vector3(0, moveY, 0);

            // Ấn E lần nữa để thoát
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitClimb();
            }
        }
    }

    public void ToggleClimbMode(Transform ladderTransform)
    {
        if (!isClimbing)
        {
            // Bắt đầu leo
            isClimbing = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            transform.position = ladderTransform.position + Vector3.up; // Dịch lên 1f
        }
        else
        {
            ExitClimb(); // Nếu đang leo, ấn E sẽ thoát
        }
    }

    private void ExitClimb()
    {
        isClimbing = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
    }
}
