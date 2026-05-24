using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RotateElectricHandle : MonoBehaviour
{
    [SerializeField] private bool isSetUp;   // bật setup từ ngoài
    [SerializeField] private bool isRotated = false; // đã xoay chưa
    [SerializeField] private bool isMouseOver = false;
    [SerializeField] private Transform electric;
    [SerializeField] private Transform gameObjectBox;

    private Quaternion startRot;
    private Quaternion targetRot;
    private float rotateTime = 1f; // thời gian xoay (1 giây)
    private float t = 0f;
    private bool isRotating = false;
   
    void OnMouseEnter() => isMouseOver = true;
    void OnMouseExit() => isMouseOver = false;
    
    public void SetIsSetUp()
    {
        isSetUp = true;
    }

    void Update()
    {
        if (isRotated && !isRotating) return;
        
        // Khi click
        if (isSetUp && isMouseOver && Input.GetMouseButtonDown(0) && !isRotating)
        {
            isRotated = true;
            startRot = transform.parent.localRotation;
            targetRot = Quaternion.Euler(38f, 0f, 0f);
            t = 0f;
            isRotating = true;
            
            AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.knobRotate, transform.position);
            gameObjectBox.gameObject.SetActive(false);
            transform.name = "SettedSphere";
        }
       
        // Quay dần trong 1 giây
        if (isRotating)
        {
            t += Time.deltaTime / rotateTime;
            transform.parent.localRotation = Quaternion.Slerp(startRot, targetRot, t);

            if (t >= 1f)
            {
                isRotating = false;
              
            }
            electric.gameObject.SetActive(false);
        }
        
    }
}
