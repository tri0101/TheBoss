using UnityEngine;

public class LadderInteraction : MonoBehaviour
{
    private bool isNearLadder = false;
    private bool isClimbing = false;
    private GameObject player;
    private Rigidbody rb;
    private PlayerController playerController;

    [SerializeField] private SetUpClimb climbData;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            rb = player.GetComponent<Rigidbody>();
            playerController = player.GetComponent<PlayerController>();
        }
    }

    void OnMouseOver() => isNearLadder = true;
    void OnMouseExit() => isNearLadder = false;

    void Update()
    {
        if (isNearLadder && !isClimbing && Input.GetKeyDown(KeyCode.E))
        {
            StartClimbing();
        }

        if (isClimbing)
        {
            float vertical = Input.GetAxis("Vertical");
            Vector3 pos = player.transform.position;

            if (vertical > 0)
            {
                pos.y += climbData.climbSpeed * Time.deltaTime;
                if (pos.y >= climbData.maxY)
                {
                    pos.y = climbData.topPosition.y;
                    pos.x = climbData.topPosition.x;
                    pos.z = climbData.topPosition.z;
                    StopClimbing();
                }
            }
            else if (vertical < 0)
            {
                pos.y -= climbData.climbSpeed * Time.deltaTime;
                if (pos.y <= climbData.minY)
                {
                    pos.y = climbData.minY;
                    pos.x = climbData.bottomPosition.x;
                    StopClimbing();
                }
            }

            player.transform.position = pos;
        }
    }

    void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        if (playerController != null)
            playerController.enabled = false;

        float newY = player.transform.position.y > climbData.startYThreshold ? climbData.startYTop : climbData.startYBottom;
        player.transform.position = new Vector3(climbData.bottomPosition.x, newY, climbData.bottomPosition.z);
        player.transform.rotation = Quaternion.Euler(climbData.climbRotationEuler);
    }

    void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;

        if (playerController != null)
            playerController.enabled = true;
    }
}
