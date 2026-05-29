using System.Collections;
using UnityEngine;

public class RotationPassSafe : MonoBehaviour
{
    [Header("Dial Rotation")]
    [SerializeField] private float defaultZ = 196f;
    [SerializeField] private float stepAngle = 45f;
    [SerializeField] private float rotateDuration = 0.12f;

    [Header("Unlock Rotation")]
    [SerializeField] private float unlockX = 90f;
    [SerializeField] private float unlockDuration = 0.35f;

    private bool isHovered;
    private bool isRotating;

    // -----------------------------------
    // 8 số trên két
    // -----------------------------------

    private readonly int[] numbers =
    {
        0, 5, 10, 15, 20, 25, 30, 35
    };

    // -----------------------------------
    // Combo:
    // 10 trái 3 lần
    // 15 phải 2 lần
    // 25 trái 1 lần
    // -----------------------------------

    private enum Dir
    {
        Left,
        Right
    }

    private class ComboStep
    {
        public int number;
        public Dir dir;
        public int requiredCount;

        public ComboStep(int number, Dir dir, int requiredCount)
        {
            this.number = number;
            this.dir = dir;
            this.requiredCount = requiredCount;
        }
    }

    private ComboStep[] combo =
    {
        new ComboStep(10, Dir.Left, 3),
        new ComboStep(15, Dir.Right, 2),
        new ComboStep(25, Dir.Left, 1)
    };

    private int comboIndex = 0;
    private int currentCount = 0;

    private void Start()
    {
        if (transform.parent == null)
            return;

        Vector3 euler = transform.parent.localEulerAngles;

        transform.parent.localRotation =
            Quaternion.Euler(euler.x, euler.y, defaultZ);
    }

    private void Update()
    {
        if (!isHovered || isRotating)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(RotateAndCheck(Dir.Left));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RotateAndCheck(Dir.Right));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSafe();
        }
    }

    // -----------------------------------
    // Rotate
    // -----------------------------------

    private IEnumerator RotateAndCheck(Dir dir)
    {
        yield return StartCoroutine(RotateDial(dir));

        CheckCombo(dir);
    }

    private IEnumerator RotateDial(Dir dir)
    {
        isRotating = true;

        Vector3 euler = transform.parent.localEulerAngles;

        float currentZ = Mathf.Repeat(euler.z, 360f);

        float targetZ;

        if (dir == Dir.Right)
            targetZ = currentZ + stepAngle;
        else
            targetZ = currentZ - stepAngle;

        targetZ = Mathf.Repeat(targetZ, 360f);

        Quaternion startRot = transform.parent.localRotation;

        Quaternion endRot =
            Quaternion.Euler(euler.x, euler.y, targetZ);

        float time = 0f;

        while (time < rotateDuration)
        {
            time += Time.deltaTime;

            float t = time / rotateDuration;

            transform.parent.localRotation =
                Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.parent.localRotation = endRot;

        isRotating = false;
    }

    // -----------------------------------
    // Combo Logic
    // -----------------------------------

    private void CheckCombo(Dir dir)
    {
        if (comboIndex >= combo.Length)
            return;

        ComboStep step = combo[comboIndex];

        int currentNumber = GetCurrentNumber();

        Debug.Log("Current Number: " + currentNumber);

        // Sai hướng
        if (dir != step.dir)
        {
            FailCombo();
            return;
        }

        // Không đúng số
        if (currentNumber != step.number)
        {
            return;
        }

        // Đúng số + đúng hướng
        currentCount++;

        Debug.Log("Count: " + currentCount);

        // Đủ số lần
        if (currentCount >= step.requiredCount)
        {
            comboIndex++;
            currentCount = 0;

            Debug.Log("Next Step");
        }

        // Mở két
        if (comboIndex >= combo.Length)
        {
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.dumpster, transform.position);
            StartCoroutine(OpenSafe());
        }
    }

    // -----------------------------------
    // Open
    // -----------------------------------

    private IEnumerator OpenSafe()
    {
        isRotating = true;

        Vector3 euler = transform.parent.parent.localEulerAngles;

        Quaternion startRot = transform.parent.parent.localRotation;

        Quaternion endRot =
            Quaternion.Euler(unlockX, euler.y, euler.z);

        float time = 0f;

        while (time < unlockDuration)
        {
            time += Time.deltaTime;

            float t = time / unlockDuration;

            transform.parent.parent.localRotation =
                Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.parent.parent.localRotation = endRot;

        Debug.Log("SAFE OPENED");

        isRotating = false;
    }

    // -----------------------------------
    // Reset
    // -----------------------------------

    private void ResetSafe()
    {
        comboIndex = 0;
        currentCount = 0;

        Vector3 euler = transform.parent.localEulerAngles;

        transform.parent.localRotation =
            Quaternion.Euler(euler.x, euler.y, defaultZ);

        Debug.Log("RESET");
    }

    // -----------------------------------
    // Number Mapping
    // -----------------------------------

    private int GetCurrentNumber()
    {
        float z =
            Mathf.Repeat(transform.parent.localEulerAngles.z, 360f);

        float delta =
            Mathf.DeltaAngle(defaultZ, z);

        int index =
            Mathf.RoundToInt(delta / stepAngle);

        index %= numbers.Length;

        if (index < 0)
            index += numbers.Length;

        return numbers[index];
    }

    // -----------------------------------
    // Mouse
    // -----------------------------------

    private void OnMouseOver()
    {
        isHovered = true;
    }

    private void OnMouseExit()
    {
        isHovered = false;
    }
    private void FailCombo()
    {
        Debug.Log("WRONG COMBO");

        comboIndex = 0;
        currentCount = 0;
    }
}