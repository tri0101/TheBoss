using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairClimb : MonoBehaviour
{
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;

    [Header("Debug Flags")]
    public bool lowerHit = false;
    public bool upperHit = false;
    public bool canClimb = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Cập nhật vị trí upper ray lên cao 1 tí
        Vector3 upperPos = stepRayUpper.transform.localPosition;
        stepRayUpper.transform.localPosition = new Vector3(upperPos.x, stepHeight, upperPos.z);
    }

    private void FixedUpdate()
    {
        stepClimb();
    }

    void stepClimb()
    {
        // Reset trạng thái
        lowerHit = false;
        upperHit = false;
        canClimb = false;

        // Các hướng kiểm tra (forward, chéo trái, chéo phải)
        Vector3[] directions = {
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(new Vector3(1.5f, 0, 1)),
            transform.TransformDirection(new Vector3(-1.5f, 0, 1))
        };

        foreach (var dir in directions)
        {
            RaycastHit hitLower;
            if (Physics.Raycast(stepRayLower.transform.position, dir, out hitLower, 0.1f))
            {
                lowerHit = true;

                RaycastHit hitUpper;
                if (!Physics.Raycast(stepRayUpper.transform.position, dir, out hitUpper, 0.2f))
                {
                    upperHit = false;
                    canClimb = true;

                    // Di chuyển rigidbody lên
                    rigidBody.MovePosition(rigidBody.position + new Vector3(0f, stepSmooth * Time.deltaTime, 0f));

                    break;
                }
                else
                {
                    upperHit = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (stepRayLower != null && stepRayUpper != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(stepRayLower.transform.position, transform.forward * 0.1f);
            Gizmos.DrawRay(stepRayLower.transform.position, transform.TransformDirection(new Vector3(1.5f, 0, 1).normalized) * 0.1f);
            Gizmos.DrawRay(stepRayLower.transform.position, transform.TransformDirection(new Vector3(-1.5f, 0, 1).normalized) * 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(stepRayUpper.transform.position, transform.forward * 0.2f);
            Gizmos.DrawRay(stepRayUpper.transform.position, transform.TransformDirection(new Vector3(1.5f, 0, 1).normalized) * 0.2f);
            Gizmos.DrawRay(stepRayUpper.transform.position, transform.TransformDirection(new Vector3(-1.5f, 0, 1).normalized) * 0.2f);
        }
    }
}
