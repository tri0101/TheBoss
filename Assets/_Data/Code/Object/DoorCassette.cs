using UnityEngine;
using System.Collections;
using Unity.Entities;

public class DoorCassette : MonoBehaviour
{
    [SerializeField] SetCassette setCassette;
    [SerializeField] private Transform starpillow;
    [SerializeField] private Transform babyCrib;
    [SerializeField] bool isOk = false;
    public bool IsOk => isOk;
    bool hasStartedMoving = false;
    bool isOpenMusic = false;
    public bool HasStartedMoving => hasStartedMoving;
    [Header("Audio")]
    [SerializeField] private float babyCryInterval = 6f;
    private float babyCryTimer = 0f;
    private void Update()
    {
        //if (isOpenMusic) return;
        //if (AudioManager.instance == null) return;
        //if (babyCrib == null) return;

        //babyCryTimer += Time.deltaTime;
        //if (babyCryTimer >= babyCryInterval)
        //{
        //    babyCryTimer = 0f;
        //    AudioManager.instance.PlaySFXAtPosition(
        //        AudioManager.instance.babyCry,
        //        babyCrib.transform.position,
                
        //        0.7f
        //    );
        //}

    }
    private void OnMouseOver()
    {
        if (setCassette == null) return;
        if (setCassette.IsOk && Input.GetMouseButtonDown(0) && !hasStartedMoving)
        {
            
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.buttonPress, transform.position);
            StartCoroutine(MoveStarCassete());
            
        }
    }

    public void SetIsOk(bool value)
    {
        isOk = value;
    }

    IEnumerator MoveStarCassete()
    {
        if (starpillow == null) yield break;

        Transform parentToMove = transform.parent;
        if (parentToMove == null) yield break;

        if(!hasStartedMoving)
        {
            hasStartedMoving = true;
        }
        else
        {
            yield break;
        }

        const float startStarZ = 12.5f;
        const float endStarZ = 12f;

        const float startParentX = -0.155f;
        const float endParentX = -0.149f;

        const float duration = 0.5f;

        Vector3 starStart = starpillow.localPosition;
        starStart.z = startStarZ;

        Vector3 starEnd = starStart;
        starEnd.z = endStarZ;

        Vector3 parentStart = parentToMove.localPosition;
        parentStart.x = startParentX;

        Vector3 parentEnd = parentStart;
        parentEnd.x = endParentX;

        // snap to known start values
        starpillow.localPosition = starStart;
        parentToMove.localPosition = parentStart;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            starpillow.localPosition = Vector3.Lerp(starStart, starEnd, t);
            parentToMove.localPosition = Vector3.Lerp(parentStart, parentEnd, t);

            yield return null;
        }

        starpillow.localPosition = starEnd;
        parentToMove.localPosition = parentEnd;

        this.enabled = false;

        isOpenMusic = true;
        AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.babyLullaby, babyCrib.position);
    }
}