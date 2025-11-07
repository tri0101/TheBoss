using UnityEngine;
using System.Collections;

public class DoorClockAuto : MonoBehaviour
{
    public MinuteHandController minuteHand;
    public HourHandController hourHand;
    [SerializeField] private DoorSoundSO doorSound;
    private bool rotatedNegative = false;
    [SerializeField] private float rotateDuration = 0.5f;

    private bool doorOpened = false;

    void Update()
    {
        if (!doorOpened && minuteHand != null && hourHand != null)
        {
            if (Mathf.Approximately(minuteHand.CurrentAngle, 150f) && Mathf.Approximately(hourHand.CurrentAngle, 260f))
            {
                doorOpened = true;
                StartCoroutine(AutoOpenAndDisable());
            }
        }
    }

    private IEnumerator AutoOpenAndDisable()
    {
        Quaternion startRotation = transform.localRotation;
        float angleDelta = rotatedNegative ? 90f : -90f;
        AudioManager.instance.PlaySFXAtPosition(doorSound.openSound, transform.position);
        Quaternion endRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + angleDelta,
            transform.localEulerAngles.z
        );

        float elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRotation;

        // Tắt cả 3 script sau khi mở
        minuteHand.enabled = false;
        hourHand.enabled = false;
        this.enabled = false;
    }
}
