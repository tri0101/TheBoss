using System;
using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Yêu cầu")]
    public KeyRequirementSO keyRequirement;

    [Header("Tham chiếu")]
    public PickUpSystem pickUpSystem;
    public Transform playerCamera;
    [SerializeField] private PlayerController pc;
    public Transform holdContainer;
    public Transform keyPosition;
    public Transform doorLock;
    public MonoBehaviour scriptHolder;
    [SerializeField] private Camera mainCam;
    private void Awake()
    {
        
        mainCam = Camera.main;
    }

    private void Start()
    {
        if (keyRequirement == null || string.IsNullOrEmpty(keyRequirement.scriptParentName))
        {
            Debug.LogWarning($"[{name}] Thiếu ScriptableObject hoặc tên script.");
            return;
        }

        Type scriptType = Type.GetType(keyRequirement.scriptParentName);
        if (scriptType == null)
        {
            Debug.LogError($"[{name}] Không tìm thấy kiểu script: {keyRequirement.scriptParentName}");
            return;
        }

        scriptHolder = GetComponent(scriptType) as MonoBehaviour;
        if (scriptHolder == null)
        {
            Debug.LogError($"[{name}] Không tìm thấy script {scriptType.Name} trên object hiện tại.");
            return;
        }

        var isCodeField = scriptType.GetField("isCode");
        if (isCodeField != null && isCodeField.FieldType == typeof(bool))
        {
            isCodeField.SetValue(scriptHolder, false);
            Debug.Log($"[{name}] Đã set isCode = false cho script: {scriptType.Name}");
        }
       
    }

    private void OnMouseOver()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (keyRequirement == null || holdContainer == null) return;
            //if (!Input.GetMouseButtonDown(0)) return;
            if (keyRequirement.requiredKeyName == "null")
            {
                //if (scriptHolder == null)
                //{
                //    Debug.LogWarning("Không có script để gọi ");
                //    return;
                //}
                if (!Input.GetMouseButtonDown(0)) return;
                var method = scriptHolder.GetType().GetMethod("Run");
                if (method != null)
                {
                    Debug.Log("Đang gọi (trường hợp null)");
                    method.Invoke(scriptHolder, null);
                }
                else
                {
                    Debug.LogError("Script không có hàm Run");
                }
                return;
            }
            foreach (Transform child in holdContainer)
            {
                if (keyRequirement.requiredKeyName == "BuffaloStatus" || keyRequirement.requiredKeyName == "LionStatus" || keyRequirement.requiredKeyName
                    == "WolfStatus" || keyRequirement.requiredKeyName == "RihnoStatus")
                {
                    if (scriptHolder == null)
                    {
                        Debug.LogWarning("Không có script để gọi ");
                        return;
                    }

                    if (!Input.GetMouseButtonDown(0)) return;
                    var method = scriptHolder.GetType().GetMethod("Run");
                    if (method != null)
                    {
                        Debug.Log("Đang gọi");
                        method.Invoke(scriptHolder, null);

                    }
                    return;
                }
                if (keyRequirement.requiredKeyName == "Soups and Stews" || 
                    keyRequirement.requiredKeyName == "The Fallen Kingdom" || 
                    keyRequirement.requiredKeyName == "Shadows of the Alley" || 
                    keyRequirement.requiredKeyName == "Advanced Physics Concepts")
                {
                    if (scriptHolder == null)
                    {
                        Debug.LogWarning("Không có script để gọi ");
                        return;
                    }

                    if (!Input.GetMouseButtonDown(0)) return;
                    var method = scriptHolder.GetType().GetMethod("Run");
                    if (method != null)
                    {
                        Debug.Log("Đang gọi");
                        method.Invoke(scriptHolder, null);

                    }
                    return;
                }
                if (child.name == keyRequirement.requiredKeyName)
                {

                    if (!Input.GetMouseButtonDown(0)) return;
                    if (child.name.StartsWith("Key"))
                    {
                        Debug.Log($"[{name}] Có chìa khóa hợp lệ: {child.name}");
                        StartCoroutine(UseKeyAndOpen(child));

                        return;
                    }
                    else
                    {
                        if (scriptHolder == null)
                        {
                            Debug.LogWarning("Không có script để gọi ");
                            return;
                        }

                        var method = scriptHolder.GetType().GetMethod("Run");
                        if (method != null)
                        {
                            Debug.Log("Đang gọi");

                            method.Invoke(scriptHolder, null);
                            if(transform.name == "FuelForm")
                            {
                                FuelManager fuel = transform.GetComponent<FuelManager>();
                                if (fuel.isFuel)
                                {
                                    this.enabled = false;
                                }
                                return;
                            }
                            if(transform.name == "KeypadVisuals")
                            {
                                return;
                            }
                            if(transform.name == "Object_314")
                            {
                                OpenDoorCar openDoor = transform.GetComponent<OpenDoorCar>();
                                if (openDoor.isOpen)
                                {
                                    this.enabled = false;
                                }
                                return;
                            }
                            this.enabled = false;
                        }
                        else
                        {
                            Debug.LogError("Script không có hàm ");
                        }
                    }
                }

            }
        }
           

        
    }

    private IEnumerator UseKeyAndOpen(Transform keyObject)
    {
        if (keyObject.name.StartsWith("Key"))
        {
            var config = pickUpSystem.CachedConfig;
            

            if (pickUpSystem != null && pickUpSystem.HeldObject == keyObject.gameObject)
            {
                pickUpSystem.ReleaseHeldObject();
            }

            Rigidbody rb = keyObject.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);
            GameObject fakeKey = Instantiate(keyObject.gameObject);
            fakeKey.name = keyObject.name; // trùng tên
            fakeKey.transform.SetParent(holdContainer.transform);
            fakeKey.transform.localPosition = Vector3.zero;
            fakeKey.transform.localRotation = Quaternion.identity;
            fakeKey.gameObject.SetActive(false);

  
            keyObject.SetParent(null);
            SetLayerRecursively(keyObject, "Default");
            keyObject.localRotation = Quaternion.Euler(config.properties.localRotationEuler);

            Vector3 forwardDir = new Vector3(0, 0, 0);
            Vector3 upDir = new Vector3(0, 0, 0);
            Vector3 afterPushPos = new Vector3(0, 0, 0);
            Vector3 targetPos = keyPosition.position;
            if (transform.parent != null && transform.parent.parent != null)
            {
                float rotationY = transform.parent.parent.localEulerAngles.y;
                if (rotationY == 90 || rotationY == 0)
                {
                    forwardDir = -keyPosition.right;
                    upDir = keyPosition.up;
                    afterPushPos = targetPos - keyPosition.right * 0.1f;

                }
                else
                {
                    forwardDir = keyPosition.right;
                    upDir = -keyPosition.up;
                   afterPushPos = targetPos + keyPosition.right * 0.1f;
                }
                Debug.Log("Local Rotation Y của cha của cha: " + rotationY);
            }
            if (keyPosition != null)
            {
                Vector3 startToKey = keyObject.position;

                Quaternion targetRot = Quaternion.LookRotation(forwardDir, upDir) * Quaternion.Euler(0, 0, 90);

             
                keyObject.rotation = targetRot;

                float moveToKeyDuration = 0.25f;
                float elapsedMove = 0f;

                while (elapsedMove < moveToKeyDuration)
                {
                    elapsedMove += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedMove / moveToKeyDuration);

                    
                    keyObject.position = Vector3.Lerp(startToKey, targetPos, t);

                    yield return null;
                }

                keyObject.position = targetPos;
                keyObject.rotation = targetRot;

                yield return new WaitForSeconds(0.25f);
   
                float pushDuration = 0.25f;
                float elapsedPush = 0f;

                while (elapsedPush < pushDuration)
                {
                    elapsedPush += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedPush / pushDuration);
                    keyObject.position = Vector3.Lerp(targetPos, afterPushPos, t);
                    yield return null;
                }

                keyObject.position = afterPushPos;

            }
            else
            {
                Debug.LogWarning("Chưa gán keyPosition.");
            }
            

            yield return new WaitForSeconds(0.25f);

            Quaternion startKeyRot = keyObject.rotation;
            Quaternion targetKeyRot = Quaternion.LookRotation(forwardDir, upDir) * Quaternion.Euler(0, 0, 0);

            Quaternion startDoorRot = doorLock.localRotation;
            Quaternion targetDoorRot =  Quaternion.Euler(-90, 180, 90);

            float rotateDuration = 0.25f;
            float elapsedRotate = 0f;

            while (elapsedRotate < rotateDuration)
            {
                elapsedRotate += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedRotate / rotateDuration);
                keyObject.rotation = Quaternion.Slerp(startKeyRot, targetKeyRot, t);
                doorLock.localRotation = Quaternion.Slerp(startDoorRot, targetDoorRot, t);

                yield return null;
            }

    
            keyObject.rotation = targetKeyRot;
            doorLock.localRotation = targetDoorRot;
            Destroy(keyObject.gameObject);
            Destroy(fakeKey);
 
            OpenDoor();

            this.enabled = false;
        }
        
    }



    private void OpenDoor()
    {
        if (scriptHolder == null)
        {
            Debug.LogWarning("Không có script để gọi Open()");
            return;
        }

        var method = scriptHolder.GetType().GetMethod("Open");
        if (method != null)
        {
            Debug.Log("Đang gọi Open()");
            method.Invoke(scriptHolder, null);
        }
        else
        {
            Debug.LogError("Script không có hàm Open()");
        }
    }
    void SetLayerRecursively(Transform target, string newLayer)
    {

        target.gameObject.layer = LayerMask.NameToLayer(newLayer);

        foreach (Transform child in target)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
}
