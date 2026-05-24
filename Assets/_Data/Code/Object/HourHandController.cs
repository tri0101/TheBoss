using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HourHandController : MonoBehaviour
{
    [SerializeField] private float rotateDuration = 0.3f;

    private bool isHovered = false;
    private bool isRotating = false;
    [SerializeField] private bool isSet = false;
   
    public float CurrentAngle { get; private set; } = -40f;

    private List<float> hourAngles = new List<float> {
        -40f, -5f, 25f, 50f, 80f, 108f, 140f, 175f, 203f, 233f, 260f, 290f
    };
    private int currentIndex = 0;

    public void SetIsSet()
    {
        isSet = true;
    }
    
    void Update()
    {
        if (isHovered && !isRotating)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(RotateHand(1));  // quay xuôi
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(RotateHand(-1)); // quay ngược
            }
        }
    }

    private IEnumerator RotateHand(int direction)
    {
        isRotating = true;

        int nextIndex = (currentIndex + direction + hourAngles.Count) % hourAngles.Count;
        float newZ = hourAngles[nextIndex];
        CurrentAngle = newZ;
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.tikSound, transform.position);
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, newZ);

        float elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
        currentIndex = nextIndex;
        isRotating = false;
    }

    private void OnMouseEnter()
    {
     
      
        isHovered = true;
    }
    private void OnMouseExit() => isHovered = false;
}
