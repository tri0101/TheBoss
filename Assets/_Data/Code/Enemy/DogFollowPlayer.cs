
using UnityEngine;

public class DogFollowPlayer : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Transform idleTarget;
    [SerializeField] private Transform meatTarget;

    private Animator animator;
    private Rigidbody rb;

    public float moveSpeed = 3f;
    public float stopDistance = 1f;
    private float destinationTolerance = 0.1f;

    [SerializeField] private LayerMask gardenLayer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition;

        // Ưu tiên thịt nếu nó nằm trên sân
        if (meatTarget != null && IsObjectOnGarden(meatTarget))
        {
            Vector3 directionToMeat = (meatTarget.position - transform.position).normalized;
            Vector3 targetNearMeat = meatTarget.position - directionToMeat * 0.5f;

            float distance = Vector3.Distance(transform.position, targetNearMeat);

            if (distance > 0.2f)
            {
                MoveTowards(targetNearMeat);
                animator.SetBool("isWalking", true);
                animator.SetBool("isEating", false);
            }
            else
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isEating", true);

                // Quay về hướng thịt
                Quaternion toRotation = Quaternion.LookRotation(directionToMeat);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.fixedDeltaTime));
            }

            return; // Bỏ qua phần kiểm tra player nếu thịt đang trên sân
        }
        else
        {
            // Nếu player trên sân
            if (IsObjectOnGarden(player))
            {
                float distance = Vector3.Distance(transform.position, player.position);

                if (distance > stopDistance)
                {
                    targetPosition = player.position;
                    MoveTowards(targetPosition);
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isEating", false);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isEating", false);
                }
            }
            else
            {
                Vector3 idlePosition = idleTarget.position + idleTarget.forward * 1f;
                float distance = Vector3.Distance(transform.position, idlePosition);

                if (distance > 0.5f)
                {
                    MoveTowards(idlePosition);
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isEating", false);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isEating", false);

                    // Xoay về phía trục Z
                    Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward);
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.fixedDeltaTime));
                }
            }
        }
        
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.fixedDeltaTime));
        }
    }

    private bool IsObjectOnGarden(Transform obj)
    {
        Vector3 origin = obj.position + Vector3.up * 0.1f;
        float rayLength = 1f;

        RaycastHit hit;
        bool hitDown = Physics.Raycast(origin, Vector3.down, out hit, rayLength, gardenLayer, QueryTriggerInteraction.Collide);
        bool hitUp = Physics.Raycast(origin, Vector3.up, out hit, rayLength, gardenLayer, QueryTriggerInteraction.Collide);

        Debug.DrawRay(origin, Vector3.down * rayLength, hitDown ? Color.green : Color.red, 0.1f);
        Debug.DrawRay(origin, Vector3.up * rayLength, hitUp ? Color.green : Color.red, 0.1f);

        return hitDown || hitUp;
    }
}
