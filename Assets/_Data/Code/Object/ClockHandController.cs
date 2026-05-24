using UnityEngine;

public class ClockHandController : MonoBehaviour
{
    [SerializeField] private SetHourHand setHourHand;
    [SerializeField] private SetMinuteHand setMinuteHand;
    private BoxCollider boxCollider;

    void Start()
    {
        if (setHourHand == null)
            setHourHand = GetComponent<SetHourHand>();
        if (setMinuteHand == null)
            setMinuteHand = GetComponent<SetMinuteHand>();

        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (setHourHand != null && setMinuteHand != null &&
            setHourHand.isSetting && setMinuteHand.isSetting)
        {
            if (boxCollider != null)
                boxCollider.enabled = false;

            setHourHand.enabled = false;
            setMinuteHand.enabled = false;

            enabled = false; // Vô hiệu hóa script này luôn
        }
    }
}
