using System;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;
public class PickUpSystem : MonoBehaviour
{
    [Header("References")]
    public Transform fpsCam;
    public Transform holdContainer;
    public Transform PickUpItem;
    

    [Header("Settings")]
    public float pickUpRange = 3f;
    public float dropForwardForce = 2f;
    public float dropUpwardForce = 1f;
    [SerializeField] private int countGun = 0;
    public Vector3 heldTargetLocalOffset = new Vector3(0.4f, -0.3f, 0.8f);

    [SerializeField] private GameObject _heldObject;
    public GameObject HeldObject => _heldObject;

    [SerializeField] private Rigidbody _heldRb;
    public Rigidbody HeldRb => _heldRb;

    [SerializeField] private PickUpConfig _cachedConfig;
    public PickUpConfig CachedConfig => _cachedConfig;

    private Quaternion heldInitialRotation;
    public void MinusCountGun()
    {
        countGun--;
    }
    void Update()
    {
        Debug.DrawRay(fpsCam.position, fpsCam.forward * pickUpRange, Color.red);

        // Raycast để xác định object có thể nhặt
        Ray ray = new Ray(fpsCam.position, fpsCam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpRange))
        {
            if (hit.collider.CompareTag("PickUp"))
            {
                Transform t = hit.collider.transform;
                Rigidbody rb = t.GetComponent<Rigidbody>();
                PickUpItem = rb != null ? t : t.parent;
            }
            else
            {
                PickUpItem = null;
            }
        }
        else
        {
            PickUpItem = null;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Đã nhấn phím E");
            if (PickUpItem != null)
                Debug.Log("Đang nhìn vào: " + PickUpItem.name);
            else
                Debug.Log("Không có vật thể nào trước mặt");
        }

        // Xử lý nhặt
        if (Input.GetKeyDown(KeyCode.E) && PickUpItem != null)
        {
            // Nếu nhặt Dart và đang cầm súng
            if ((PickUpItem.name == "Tranquilizer dart" || PickUpItem.name == "Used tranquilizer dart") &&
                _heldObject != null && _heldObject.name == "tranquilizer_gun")
            {
                if (countGun == 1) return;
                AudioManager.instance.PlaySFXAtPosition(AudioManager.instance.setUpMotor, transform.position);
                countGun++;
                Debug.Log("Đã gắn Dart vào tranquilizer_gun khi đang cầm súng.");
                PickUpItem.SetParent(_heldObject.transform, false);
                PickUpItem.localRotation = Quaternion.Euler(90, 0, 0);
                PickUpItem.localPosition = Vector3.zero;
                PickUpItem.localScale = Vector3.one;

                Rigidbody dartRb = PickUpItem.GetComponent<Rigidbody>();
                Destroy(dartRb);
                //if (dartRb != null) dartRb.isKinematic = true;

                //foreach (Collider col in PickUpItem.GetComponentsInChildren<Collider>())
                //{
                //    col.isTrigger = true;
                //}
                    

                //MeshCollider meshCol = PickUpItem.GetComponentInChildren<MeshCollider>();
                //if (meshCol != null) meshCol.enabled = false;

                return;
            }
            else if (_heldObject == null)
            {
                PickUp(PickUpItem.gameObject);
            }
            Collider playerCol = transform.GetComponent<Collider>();
            Collider[] objCols = _heldObject.GetComponentsInChildren<Collider>();
            foreach (var objCol in objCols)
            {
                if (playerCol != null && objCol != null)
                {
                    Physics.IgnoreCollision(playerCol, objCol, true);


                }
            }
            Destroy(_heldRb);
        }
        else if (_heldObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            StopClipping();
            Drop();
        }
    }
    void SetLayerRecursively(GameObject obj, int newLayer, string state)
    {
        if (obj == null) return;

        // Nếu đối tượng tên là "NonHoldLayer" → đặt layer "Ex"
        if (obj.name == "NonHoldLayer")
        {
            if(state == "Drop")
            {
                obj.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                obj.layer = LayerMask.NameToLayer("Ex");
            }
            
        }
        else
        {
            obj.layer = newLayer;
        }

        // Đệ quy cho tất cả con
        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer, state);
            }
        }
    }

    void PickUp(GameObject pickUpObject)
    {
        SetLayerRecursively(pickUpObject, LayerMask.NameToLayer("holdLayer"), "Pick up");
        _heldObject = pickUpObject;
        _heldRb = _heldObject.GetComponent<Rigidbody>();
        //Lưu ý nhá
        foreach (Collider col in pickUpObject.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = true;
        }
        if (_heldRb == null)
        {
            Debug.LogWarning("Không tìm thấy Rigidbody để nhặt.");
            return;
        }

        PickUpConfig config = _heldObject.GetComponentInParent<PickUpConfig>();
        if (config != null && config.properties != null)
        {
            _cachedConfig = config;
            _heldObject.transform.localScale = config.properties.localScale;
            _heldObject.transform.localPosition = config.properties.localPosition;
            _heldObject.transform.rotation = Quaternion.Euler(config.properties.localRotationEuler);
        }

        heldInitialRotation = _heldObject.transform.rotation;

        _heldRb.isKinematic = true;

        _heldObject.transform.SetParent(holdContainer, false);

        if (_heldObject.name == "ShotGun")
        {
            GunShoot gunScript = _heldObject.GetComponent<GunShoot>();
            if (gunScript != null)
            {
                gunScript.enabled = true;
                Debug.Log("GunShoot script đã được kích hoạt cho ShotGun.");
            }
        }

        if (_heldObject.name == "Ex")
        {
            Transform armature = _heldObject.transform.Find("Armature");
            if (armature != null)
            {
                armature.localPosition = new Vector3(340.5346f, 3378.28f, -386.4091f);
                armature.localRotation = Quaternion.Euler(-55.392f, 64.462f, -45.441f);
                armature.localScale = new Vector3(186.2408f, 186.2408f, 186.2408f);
            }
        }

    }
    public void DropWhenDie()
    {

        _heldRb = _heldObject.AddComponent<Rigidbody>();
        _heldRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        if (_heldObject == null || _heldRb == null) return;
        SetLayerRecursively(_heldObject, LayerMask.NameToLayer("Default"), "Drop");
        _heldObject.transform.SetParent(null);
        holdContainer.localRotation = Quaternion.identity;

        _heldRb.isKinematic = false;
        //_heldRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //if(_heldObject.name == "Stick")
        //{
        //    HeldRb.angularDamping = 0.07f;
        //}

        foreach (Collider col in _heldObject.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = false;
        }
        // Chống xuyên tường khi thả
        // -------------------------
        bool adjusted = false;
        Collider[] allCols = _heldObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in allCols)
        {
            if (col == null) continue;

            Collider[] overlaps = Physics.OverlapBox(
                col.bounds.center,
                col.bounds.extents,
                col.transform.rotation,
                LayerMask.GetMask("Default", "Wall", "ObjectCanMove")
            );

            if (overlaps.Length > 0)
            {
                // Raycast từ camera ra trước để tìm chỗ trống
                Ray ray = new Ray(fpsCam.position, fpsCam.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 2f))
                {
                    // Phân tích hướng bề mặt bị trúng
                    Vector3 hitNormal = hit.normal;  // pháp tuyến của bề mặt
                    float offset;

                    if (_heldObject.name.Contains("Key"))
                        offset = 0.1f;
                    else if (_heldObject.name == "Cue Stick" || _heldObject.name == "Ex")
                        offset = 0.7f;
                    else if (_heldObject.name == "Handle")
                        offset = 1f;
                    else
                        offset = 0.3f;

                    // Nếu mặt bị trúng gần như là tường (normal hướng ngang)
                    if (Mathf.Abs(hitNormal.y) < 0.5f)
                    {
                        // Tường đứng → dịch ngang ra trước tường
                        Vector3 horizontalDir = fpsCam.forward;
                        horizontalDir.y = 0f;
                        horizontalDir.Normalize();

                        Vector3 newPos = hit.point - horizontalDir * offset;
                        newPos.y = hit.point.y; // giữ độ cao ổn định
                        _heldObject.transform.position = newPos;
                    }
                    else
                    {
                        // Trúng sàn hoặc trần → đặt ngay trên mặt chạm, không offset nhiều
                        Vector3 newPos = hit.point + hitNormal * 0.05f; // nhấc nhẹ lên 5cm
                        _heldObject.transform.position = newPos;
                    }
                }


                //else
                //{
                //    // Nếu không đụng gì, đặt object cách player 1m
                //    _heldObject.transform.position = fpsCam.position + fpsCam.forward * 1f;
                //}
                adjusted = true;
                break;
            }
        }
        if (_heldObject.name == "Ex")
        {
            Transform armature = _heldObject.transform.Find("Armature");
            if (armature != null)
            {
                armature.localPosition = new Vector3(340.5346f, 3378.28f, -386.4091f);
                armature.localRotation = Quaternion.Euler(11.616f, 90f, -90f);
            }
        }

        if (_heldObject.name.Contains("ShotGun"))
        {
            GunShoot gunScript = _heldObject.GetComponent<GunShoot>();
            if (gunScript != null) gunScript.enabled = false;
        }



        PickUpConfig config = _heldObject.GetComponent<PickUpConfig>();
        if (config != null)
        {
            config.IsFalling = true;
        }

        ReleaseHeldObject();

    }
    public void Drop()
    {
        _heldRb = _heldObject.AddComponent<Rigidbody>();
        _heldRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        if (_heldObject == null || _heldRb == null) return;
        SetLayerRecursively(_heldObject, LayerMask.NameToLayer("Default"), "Drop");
        _heldObject.transform.SetParent(null);
        holdContainer.localRotation = Quaternion.identity;

        _heldRb.isKinematic = false;
        //_heldRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //if(_heldObject.name == "Stick")
        //{
        //    HeldRb.angularDamping = 0.07f;
        //}

        foreach (Collider col in _heldObject.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = false;
        }
        // Chống xuyên tường khi thả
        // -------------------------
        bool adjusted = false;
        Collider[] allCols = _heldObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in allCols)
        {
            if (col == null) continue;

            Collider[] overlaps = Physics.OverlapBox(
                col.bounds.center,
                col.bounds.extents,
                col.transform.rotation,
                LayerMask.GetMask("Default", "Wall", "ObjectCanMove")
            );

            if (overlaps.Length > 0)
            {
                // Raycast từ camera ra trước để tìm chỗ trống
                Ray ray = new Ray(fpsCam.position, fpsCam.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 2f))
                {
                    // Phân tích hướng bề mặt bị trúng
                    Vector3 hitNormal = hit.normal;  // pháp tuyến của bề mặt
                    float offset;

                    if (_heldObject.name.Contains("Key"))
                        offset = 0.1f;
                    else if (_heldObject.name == "Cue Stick" || _heldObject.name == "Ex")
                        offset = 0.7f;
                    else if (_heldObject.name == "Handle")
                        offset = 1f;
                    else
                        offset = 0.3f;

                    // Nếu mặt bị trúng gần như là tường (normal hướng ngang)
                    if (Mathf.Abs(hitNormal.y) < 0.5f)
                    {
                        // Tường đứng → dịch ngang ra trước tường
                        Vector3 horizontalDir = fpsCam.forward;
                        horizontalDir.y = 0f;
                        horizontalDir.Normalize();

                        Vector3 newPos = hit.point - horizontalDir * offset;
                        newPos.y = hit.point.y; // giữ độ cao ổn định
                        _heldObject.transform.position = newPos;
                    }
                    else
                    {
                        // Trúng sàn hoặc trần → đặt ngay trên mặt chạm, không offset nhiều
                        Vector3 newPos = hit.point + hitNormal * 0.05f; // nhấc nhẹ lên 5cm
                        _heldObject.transform.position = newPos;
                    }
                }


                //else
                //{
                //    // Nếu không đụng gì, đặt object cách player 1m
                //    _heldObject.transform.position = fpsCam.position + fpsCam.forward * 1f;
                //}
                adjusted = true;
                break;
            }
        }
        if (_heldObject.name == "Ex")
        {
            Transform armature = _heldObject.transform.Find("Armature");
            if (armature != null)
            {
                armature.localPosition = new Vector3(340.5346f, 3378.28f, -386.4091f);
                armature.localRotation = Quaternion.Euler(11.616f, 90f, -90f);
            }
        }
      
        // Thêm lực đẩy khi thả
        _heldRb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        _heldRb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        if (_heldObject.name.Contains("ShotGun"))
        {
            GunShoot gunScript = _heldObject.GetComponent<GunShoot>();
            if (gunScript != null) gunScript.enabled = false;
        }
        
      
       
        PickUpConfig config = _heldObject.GetComponent<PickUpConfig>();
        if (config != null)
        {
            config.IsFalling = true;
        }
        _heldObject.transform.SetParent(transform.parent, true);

        ReleaseHeldObject();
     
    }
    //void Drop()
    //{
    //    if (_heldObject == null || _heldRb == null) return;

    //    // Trả layer về mặc định
    //    SetLayerRecursively(_heldObject, LayerMask.NameToLayer("Default"));
    //    _heldObject.transform.SetParent(null);
    //    holdContainer.localRotation = Quaternion.identity;

    //    // Bật physics
    //    _heldRb.isKinematic = false;
    //    //_heldRb.useGravity = true;
    //    _heldRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    //    foreach (Collider col in _heldObject.GetComponentsInChildren<Collider>())
    //    {
    //        col.isTrigger = false;
    //    }


    //    // -------------------------
    //    // Chống xuyên tường khi thả
    //    // -------------------------
    //    bool adjusted = false;
    //    Collider[] allCols = _heldObject.GetComponentsInChildren<Collider>();
    //    foreach (Collider col in allCols)
    //    {
    //        if (col == null) continue;

    //        Collider[] overlaps = Physics.OverlapBox(
    //            col.bounds.center,
    //            col.bounds.extents,
    //            col.transform.rotation,
    //            LayerMask.GetMask("Default", "Wall")
    //        );

    //        if (overlaps.Length > 0)
    //        {
    //            // Raycast từ camera ra trước để tìm chỗ trống
    //            Ray ray = new Ray(fpsCam.position, fpsCam.forward);
    //            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
    //            {
    //                // Đặt object ngay trước mặt tường, lùi một chút
    //                //_heldObject.transform.position = hit.point - ray.direction * 0.5f;
    //                Vector3 newPos = hit.point - ray.direction * 0.5f;

    //                // Giữ nguyên độ cao (Y) của object đang cầm
    //                newPos.y = _heldObject.transform.position.y;

    //                _heldObject.transform.position = newPos;
    //                //_heldRb.linearVelocity = Vector3.zero;
    //                //_heldRb.angularVelocity = Vector3.zero;


    //            }
    //            //else
    //            //{
    //            //    // Nếu không đụng gì, đặt object cách player 1m
    //            //    _heldObject.transform.position = fpsCam.position + fpsCam.forward * 1f;
    //            //}
    //            adjusted = true;
    //            break;
    //        }
    //    }

    //    // -------------------------
    //    // Tính lực ném tự nhiên
    //    // -------------------------
    //    float forwardForce = dropForwardForce;
    //    float upwardForce = dropUpwardForce;

    //    // Nếu quá gần tường → giảm lực ném
    //    Ray checkRay = new Ray(fpsCam.position, fpsCam.forward);
    //    if (Physics.Raycast(checkRay, out RaycastHit wallHit, 1f, LayerMask.GetMask("Wall")))
    //    {
    //        forwardForce *= 0f; // giảm xuống 30%
    //        upwardForce *= 0.5f;
    //    }

    //    _heldRb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
    //    _heldRb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

    //    // -------------------------
    //    // Xử lý đặc biệt
    //    // -------------------------
    //    if (_heldObject.name.Contains("ShotGun"))
    //    {
    //        GunShoot gunScript = _heldObject.GetComponent<GunShoot>();
    //        if (gunScript != null) gunScript.enabled = false;
    //    }

    //    if (_heldObject.name == "Ex")
    //    {
    //        Transform armature = _heldObject.transform.Find("Armature");
    //        if (armature != null)
    //        {
    //            armature.localPosition = new Vector3(340.5346f, 3378.28f, -386.4091f);
    //            armature.localRotation = Quaternion.Euler(11.616f, 90f, -90f);
    //        }
    //    }

    //    PickUpConfig config = _heldObject.GetComponent<PickUpConfig>();
    //    if (config != null) config.IsFalling = true;

    //    // Reset biến
    //    _heldObject = null;
    //    _heldRb = null;
    //    _cachedConfig = null;
    //}
    //void Drop()
    //{
    //    if (_heldObject == null || _heldRb == null) return;

    //    // Trả layer về mặc định
    //    SetLayerRecursively(_heldObject, LayerMask.NameToLayer("Default"));
    //    _heldObject.transform.SetParent(null);
    //    holdContainer.localRotation = Quaternion.identity;

    //    // Bật physics cơ bản
    //    _heldRb.isKinematic = false;
    //    _heldRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    //    foreach (Collider col in _heldObject.GetComponentsInChildren<Collider>())
    //    {
    //        col.isTrigger = false;
    //    }

    //    // -------------------------
    //    // Kiểm tra nếu quá gần tường
    //    // -------------------------
    //    Ray checkRay = new Ray(fpsCam.position, fpsCam.forward);
    //    if (Physics.Raycast(checkRay, out RaycastHit wallHit, 1f, LayerMask.GetMask("Wall", "Default")))
    //    {
    //        // Tắt Rigidbody tạm để tránh AddForce
    //        _heldRb.isKinematic = true;

    //        // Dịch object ra phía trước player (ra ngoài tường)
    //        Vector3 dirToPlayer = (fpsCam.position - wallHit.point).normalized;
    //        Vector3 newPos = wallHit.point + dirToPlayer * 0.3f;  // dịch ra khỏi tường 0.3f

    //        // Giữ nguyên chiều cao Y
    //        newPos.y = _heldObject.transform.position.y;

    //        // Raycast xuống để đặt ngay trên mặt đất
    //        Ray downRay = new Ray(newPos + Vector3.up * 0.5f, Vector3.down);
    //        if (Physics.Raycast(downRay, out RaycastHit groundHit, 5f, LayerMask.GetMask("Default", "Ground")))
    //        {
    //            newPos.y = groundHit.point.y + 0.05f;
    //        }

    //        _heldObject.transform.position = newPos;

    //        // Bật lại Rigidbody để rớt tự nhiên
    //        _heldRb.isKinematic = false;

    //        // Reset biến
    //        _heldObject = null;
    //        _heldRb = null;
    //        _cachedConfig = null;
    //        Debug.Log("Moi");
    //        return;
    //    }

    //    // -------------------------
    //    // Trường hợp bình thường (không sát tường) → ném như cũ
    //    // -------------------------

    //    Debug.Log("Nhu cu");
    //    float forwardForce = dropForwardForce;
    //    float upwardForce = dropUpwardForce;

    //    _heldRb.AddForce(fpsCam.forward * forwardForce, ForceMode.Impulse);
    //    _heldRb.AddForce(fpsCam.up * upwardForce, ForceMode.Impulse);

    //    // -------------------------
    //    // Xử lý đặc biệt
    //    // -------------------------
    //    if (_heldObject.name.Contains("ShotGun"))
    //    {
    //        GunShoot gunScript = _heldObject.GetComponent<GunShoot>();
    //        if (gunScript != null) gunScript.enabled = false;
    //    }

    //    if (_heldObject.name == "Ex")
    //    {
    //        Transform armature = _heldObject.transform.Find("Armature");
    //        if (armature != null)
    //        {
    //            armature.localPosition = new Vector3(340.5346f, 3378.28f, -386.4091f);
    //            armature.localRotation = Quaternion.Euler(11.616f, 90f, -90f);
    //        }
    //    }

    //    PickUpConfig config = _heldObject.GetComponent<PickUpConfig>();
    //    if (config != null) config.IsFalling = true;

    //    // Reset biến
    //    _heldObject = null;
    //    _heldRb = null;
    //    _cachedConfig = null;
    //}

    void StopClipping()
    {
        if (_heldObject == null) return;

        float clipRange = Vector3.Distance(_heldObject.transform.position, fpsCam.position);
        RaycastHit[] hits = Physics.RaycastAll(fpsCam.position, fpsCam.forward, clipRange);

        if (hits.Length > 1)
        {
            // dịch object về trước mặt player một chút
            _heldObject.transform.position = fpsCam.position + fpsCam.forward * 1f;
        }
    }

    public void ReleaseHeldObject()
    {
        _heldObject = null;
        _heldRb = null;
        _cachedConfig = null;
    }
}

