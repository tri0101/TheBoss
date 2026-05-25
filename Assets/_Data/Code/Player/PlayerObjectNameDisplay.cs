using SojaExiles;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObjectNameDisplay : MonoBehaviour
{
    [SerializeField] private Camera fpsCam;
    [SerializeField] private float checkRange = 3f;
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform canvasButtonE;
    [SerializeField] private Transform canvasButtonLMB;
    [SerializeField] private Transform canvasButtonLMBRed;
    [SerializeField] private Transform canvasButtonQ;
    [SerializeField] private Transform canvasButtonShoot;
    [SerializeField] private Transform canvasButtonSpray;
    [SerializeField] private Transform canvasButtonERotate;
    [SerializeField] private Transform canvasButtonQRotate;
    [SerializeField] private Transform canvasButtonQCloseLaptop;
    public Transform CanvasButtonQCloseLaptop { get => canvasButtonQCloseLaptop; set => canvasButtonQCloseLaptop = value; }
    [SerializeField] private Transform canvasButtonQCloseInvoice;
     public Transform CanvasButtonQCloseInvoice { get => canvasButtonQCloseInvoice; set => canvasButtonQCloseInvoice = value; }
    [SerializeField] private Transform canvasButtonTab;
    [SerializeField] private Transform holdContainer;
    [SerializeField] private TextMeshProUGUI messageText;
    private Transform currentHit;
    [SerializeField] private bool isRayTrue;
    private Coroutine lockMessageCoroutine;
    private Coroutine messageCoroutine;
    private PlayerController pc;
    [SerializeField] private Transform doorBlueKey;
    [SerializeField]  private bool isDoorBlueKeyOpen;
  
    private bool isHoldedSet = false;
 

    [SerializeField] private Transform carHood;
    [SerializeField] private RectTransform canvasCar;
    [SerializeField] private float moveSpeed = 600f;
    private bool isMoving = false;
    private bool isVisible = false;
   
    int countListComplete = 0;

    private bool isGetKeyCar = false;

    public void setFalseLMB(){
        canvasButtonLMB.gameObject.SetActive(false);
    }
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        canvasButtonTab.gameObject.SetActive(false);
        canvasCar.anchoredPosition = new Vector2(250f, canvasCar.anchoredPosition.y);
        isVisible = false;
        isMoving = false;
        canvasCar.gameObject.SetActive(false);
    }
    void Update()
    {
        
        CheckHoldBottle();
        ShowObjectName();
        CheckInteractObject();
        CheckChildHoldContainer();
        CheckDoorKeyOpen();
        if (isDoorBlueKeyOpen)
        {
            CheckTaskList();
        }
        
        if (!isGetKeyCar)
        {
            CheckKeyCar();
        }
        

    }
    private void CheckDoorKeyOpen()
    {
        if (isDoorBlueKeyOpen) return;
        opencloseDoor ocD = doorBlueKey.GetComponent<opencloseDoor>();
        if (ocD.isCode)
        {
            
            canvasButtonTab.gameObject.SetActive(true);
            canvasCar.gameObject.SetActive(true);
            StartCoroutine(MoveCanvasCar());
            isDoorBlueKeyOpen = true;
        }
    }
    private void CheckKeyCar()
    {
        if(holdContainer.childCount > 0)
        {
            if(holdContainer.GetChild(0).name == "CarKey")
            {
                CompleteTask("carKeyReady");
                isGetKeyCar = true;
            }
        }
    }
    private void CheckTaskList()
    {
        // Ấn Tab để mở / đóng bảng nhiệm vụ
        if (Input.GetKeyDown(KeyCode.Tab) && !isMoving && canvasButtonTab.gameObject.activeSelf)
        {
            
            StartCoroutine(MoveCanvasCar());
        }
    }
    public bool GetReturnDoorKeyBlue()
    {
        return isDoorBlueKeyOpen;
    }
    private IEnumerator MoveCanvasCar()
    {
        isMoving = true;
        Vector2 startPos = canvasCar.anchoredPosition;
        Vector2 targetPos;

        if (!isVisible)
            targetPos = new Vector2(-250f, startPos.y); // Trượt vào
        else
            targetPos = new Vector2(250f, startPos.y); // Trượt ra đến x = 250

        while (Vector2.Distance(canvasCar.anchoredPosition, targetPos) > 0.1f)
        {
            canvasCar.anchoredPosition = Vector2.MoveTowards(
                canvasCar.anchoredPosition,
                targetPos,
                moveSpeed * Time.unscaledDeltaTime
            );
            yield return null;
        }

        canvasCar.anchoredPosition = targetPos;
        isVisible = !isVisible;
        isMoving = false;
    }
    public void CompleteTask(string taskName)
    {
        StartCoroutine(ShowAndCompleteTask(taskName));
    }

    private IEnumerator ShowAndCompleteTask(string taskName)
    {
        // 🔹 Nếu bảng chưa hiển thị thì mở ra trước
        if (!isVisible && !isMoving)
        {
            yield return StartCoroutine(MoveCanvasCar());
            yield return new WaitForSecondsRealtime(0.2f); // đợi tí cho người chơi thấy
        }

        // 🔹 Tìm nhiệm vụ và bật dấu hoàn thành
        Transform task = canvasCar.Find(taskName);
        if (task == null)
        {
            Debug.LogWarning($"❌ Không tìm thấy nhiệm vụ có tên: {taskName}");
            yield break;
        }

        if (task.childCount > 0)
        {
            countListComplete++;
            task.GetChild(0).gameObject.SetActive(true);
            Debug.Log($"✅ Đã hoàn thành nhiệm vụ: {taskName}");
        }
        else
        {
            Debug.LogWarning($"⚠ Nhiệm vụ {taskName} không có object con để kích hoạt!");
        }
    }
    private void  CheckHoldBottle()
    {
        if (isHoldedSet) return;
        if (holdContainer.childCount == 0) return;
        if(holdContainer.GetChild(0).name == "Whisky_Bottle")
        {
            ShowMessage(3f, "There is something in the bottle");
        }
    }
    private void CheckLaptopOpen()
    {
         
    }
    private void CheckChildHoldContainer()
    {
        if (holdContainer.childCount > 0)
        {
            canvasButtonQ.gameObject.SetActive(true);
            foreach (Transform child in holdContainer)
            {
                if (child.name == "tranquilizer_gun")
                {
                    TranquilizerGun tz = child.GetComponent<TranquilizerGun>();
                    if(tz.dartTransform != null)
                    {
                        canvasButtonShoot.gameObject.SetActive(true);
                    }
                    else
                    {
                        canvasButtonShoot.gameObject.SetActive(false);
                    }
                   
                }
                else if (child.name == "Ex")
                {
                    TranquilizerGun tz = child.GetComponent<TranquilizerGun>();
                    
                     canvasButtonSpray.gameObject.SetActive(true);
                   

                }

            }

        }
        else
        {
            canvasButtonQ.gameObject.SetActive(false);
            canvasButtonShoot.gameObject.SetActive(false);
            canvasButtonSpray.gameObject.SetActive(false);
        }
    }

    private void CheckInteractObject()
    {
        isRayTrue = false;

        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, checkRange))
        {
            if (hit.collider.CompareTag("InteractableObject") ||  hit.collider.transform.name.Contains("Meat")  )
            {
                // Lấy tất cả script InteractableObject trên object đó
                InteractableObject[] interactables = hit.collider.GetComponents<InteractableObject>();

                foreach (var interactable in interactables)
                {
                    if (interactable != null && interactable.enabled && interactable.keyRequirement != null)
                    {
                        string requiredKeyName = interactable.keyRequirement.requiredKeyName;
                        if(requiredKeyName == "null")
                        {
                            isRayTrue = true;
                            canvasButtonLMB.gameObject.SetActive(true);
                            pc.isOnObject = true;


                            return;
                        }
                        // Duyệt qua các con trong holdContainer
                        if (holdContainer.childCount == 0)
                        {
                            if (hit.collider.transform.name.Contains("Meat")) return;
                            if (hit.collider.transform.name.Contains("Tuong"))
                            {
                                SetStatus setStatus = hit.transform.GetComponent<SetStatus>();
                                if (setStatus.IsSetted()) return;
                            }
                            pc.isNotCorrect = true;
                            canvasButtonLMBRed.gameObject.SetActive(true);
                            return;
                        }

                            foreach (Transform child in holdContainer)

                        {
                            if(hit.transform.name == "FuelForm")
                            {
                                if(holdContainer.childCount > 0)
                                {
                                    if (holdContainer.GetChild(0).name == "EmptyGasCan")
                                    {
                                        if (Input.GetMouseButtonDown(0))
                                        {
                                            ShowMessage(3f, "The gas can is empty");
                                            return;
                                        }

                                    }
                                }
                             
                            }
                            
                           
                            if (child.name.Contains("Status") && requiredKeyName.Contains("Status"))
                            {
                                isRayTrue = true;
                                canvasButtonLMB.gameObject.SetActive(true);
                                pc.isOnObject = true;


                                return;
                            }
                            if (child.name == requiredKeyName)
                            {
                                isRayTrue = true;
                                canvasButtonLMB.gameObject.SetActive(true);
                                pc.isOnObject = true;
                                
                                   
                                return;
                            }
                            else if (hit.collider.transform.name == "CLock_ClockDark_0")
                            {
                                if (holdContainer.GetChild(0).name != "ClockHandOne" && holdContainer.GetChild(0).name != "ClockHandtwo")
                                {
                                    pc.isNotCorrect = true;
                                    canvasButtonLMBRed.gameObject.SetActive(true);
                                    return;
                                }
                            }
                            else if (hit.collider.transform.name == "MotorBlockForm")
                            {
                                if (holdContainer.GetChild(0).name != "MotorBattery" && holdContainer.GetChild(0).name != "MotorBlock")
                                {
                                    pc.isNotCorrect = true;
                                    canvasButtonLMBRed.gameObject.SetActive(true);
                                    return;
                                }
                            }
                            else
                            {
                                if (hit.collider.transform.name.Contains("Meat")) return;
                                if (hit.collider.transform.name.Contains("Tuong"))
                                {
                                    SetStatus setStatus = hit.transform.GetComponent<SetStatus>();
                                    if (setStatus.IsSetted()) return;
                                }
                                
                                pc.isNotCorrect = true;
                                canvasButtonLMBRed.gameObject.SetActive(true);
                                return;
                            }

                        
                        }
                       
                    }
                }
            }
            if(hit.collider.transform.name == "Sphere001")
            {
                isRayTrue = true;
                canvasButtonLMB.gameObject.SetActive(true);
                pc.isOnObject = true;
                return;
            }
            if (hit.collider.transform.name == "Ladder")
            {
                isRayTrue = true;
                canvasButtonE.gameObject.SetActive(true);
                pc.isOnObject = true;
                return;
            }
           
        }


    }

    private void ShowObjectName()
    {
        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, checkRange))
        {
            
            Transform hitTransform = hit.collider.transform;
            if (hitTransform.name == "ClockHandOne_ClockHands_0")
            {
                HourHandController hhctrl = hitTransform.GetComponent<HourHandController>();
                if (hhctrl.enabled)
                {
                    pc.isOnObject = true;
                    canvasButtonERotate.gameObject.SetActive(true);
                    canvasButtonQRotate.gameObject.SetActive(true);
                    return;
                }
            }
            if (hitTransform.name == "ClockHandtwo_ClockHands_0")
            {
                MinuteHandController mmctrl = hitTransform.GetComponent<MinuteHandController>();
                if (mmctrl.enabled)
                {
                    pc.isOnObject = true;
                    canvasButtonERotate.gameObject.SetActive(true);
                    canvasButtonQRotate.gameObject.SetActive(true);
                    return;
                }
            }
            if (hitTransform.name.StartsWith("_"))
            {
                if (hit.transform.name == "_Door.Block")
                {


                    if (transform.position.x > hit.transform.position.x)
                    {
                        pc.isNotCorrect = true;
                        canvasButtonLMBRed.gameObject.SetActive(true);
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(3f, "It’s locked from the other side");
                            return;
                        }
                    }
                }
                opencloseDoor cl = hitTransform.GetComponent<opencloseDoor>();
                if (cl != null && !cl.isCode)
                {
                    // Chỉ hiện khi ấn chuột trái
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (cl.open)
                            return;
                        InteractableObject[] interactables = hitTransform.GetComponents<InteractableObject>();
                        
                        foreach (var interactable in interactables)
                        {
                            if (interactable != null && interactable.enabled && interactable.keyRequirement != null)
                            {
                                string requiredKeyName = interactable.keyRequirement.requiredKeyName;
                                if(requiredKeyName != null)
                                {
                                    if(holdContainer.childCount > 0)
                                    {
                                        if (holdContainer.GetChild(0).name != requiredKeyName)
                                        {
                                            //if (messageText.text == " ")
                                            //{
                                            //    Debug.Log(" ok");
                                            //    return;
                                            //}
                                            ShowMessage(1f, "It's locked");
                                        }

                                        else
                                        {
                                            ShowMessage(1f, " ");
                                            //Debug.Log(" cách nha ");
                                        }
                                        
                                    }
                                    else 
                                    {
                                        ShowMessage(1f, "It's locked");
                                    }
                                    
                                }
                            }
                        }
                       
                    }
                    
                }
                else
                {
                    pc.isOnObject = true;
                    canvasButtonLMB.gameObject.SetActive(true);
                    
                }
                   
                return;
            }
            if (hit.transform.name == "CookingBook")
            {
                nameText.text = "\"Baking for Beginners\", \"The Art of Grilling\", \"Sweet Desserts\"...";
                pc.isOnObject = true;
                canvasButtonE.gameObject.SetActive(false);
                return;
            }
            else if (hit.transform.name == "DetectiveBook")
            {
                nameText.text = "\"The Last Detective\", \"Murder on Bell Street\", \"Whispers Behind the Door\" ....";
                pc.isOnObject = true;
                canvasButtonE.gameObject.SetActive(false);
                return;
            }
            else if (hit.transform.name == "ScienceBook")
            {
                nameText.text = "\"Introduction to Physics\", \"The Laws of Motion\",\"Energy and Matter\"...";
                pc.isOnObject = true;
                canvasButtonE.gameObject.SetActive(false);
                return;
            }
            
            if (hitTransform.name == "KeypadVisuals")
            {
                
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if(holdContainer.childCount == 0 || holdContainer.GetChild(0).name != "Cue Stick")
                        ShowMessage(3f, "You can't reach it");
                        else ShowMessage(1f, "");
                    }
                return;
                
            }
            if(hitTransform.name == "Plank_wood")
            {
                
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if(holdContainer.childCount == 0 || holdContainer.GetChild(0).name != "Cue Stick")
                        ShowMessage(3f, "You can't reach it");
                        else ShowMessage(1f, "");
                    }
                return;
                
            }
            if(hitTransform.name.StartsWith("Wooden_Crate"))
            {
                
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if(holdContainer.childCount == 0 || holdContainer.GetChild(0).name != "Axe")
                        ShowMessage(3f, "You need something to break it");
                        else ShowMessage(1f, "");
                    }
                return;
                
            }
            if(hitTransform.name.StartsWith("Nail"))
            {
                
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if(holdContainer.childCount == 0 || holdContainer.GetChild(0).name != "Screwdriver")
                        ShowMessage(3f, "You need something to remove this nail");
                        else ShowMessage(1f, "");
                    }
                return;
                
            }
            if (hitTransform.name.StartsWith("Tuong"))
            {

                SetStatus set = hitTransform.gameObject.GetComponent<SetStatus>();
                bool isSet = set.IsSetted();
                if (Input.GetMouseButtonDown(0))
                {
                    if (holdContainer.childCount == 0)
                    {
                        if (isSet) return;
                        ShowMessage(3f, "It's missing something");
                        return;

                    }
                    if(holdContainer.GetChild(0).name != "BuffaloStatus" &&
                    holdContainer.GetChild(0).name != "WolfStatus" && holdContainer.GetChild(0).name != "LionStatus" &&
                    holdContainer.GetChild(0).name != "RihnoStatus")
                    {
                        if (isSet) return;
                        ShowMessage(3f, "It's missing something");
                    }
                       
                    else ShowMessage(1f, "");
                }
                return;

            }
            if (hitTransform.name.StartsWith("CLock_ClockDark_0"))
            {

                SetHourHand hourSet = hitTransform.gameObject.GetComponent<SetHourHand>();
                SetMinuteHand minuteSet = hitTransform.GetComponent<SetMinuteHand>();
                if (Input.GetMouseButtonDown(0))
                {
                    if (holdContainer.childCount == 0)
                    {
                        if (hourSet.isSetting && minuteSet.isSetting) return;
                        
                            ShowMessage(3f, "It's missing something");
                        return;
                        
                    }
                    if ( holdContainer.GetChild(0).name != "ClockHandOne" && holdContainer.GetChild(0).name != "ClockHandtwo")
                    {
                        if (hourSet.isSetting && minuteSet.isSetting) return;
                        ShowMessage(3f, "It's missing something");
                    }
                        
                    else ShowMessage(1f, "");
                }
                return;

            }
            if (hitTransform.name.StartsWith("MotorBlockForm"))
            {

                MotorBatterySetUp batterySet = hitTransform.gameObject.GetComponent<MotorBatterySetUp>();
                MotorBlockSetUp blockSet = hitTransform.GetComponent<MotorBlockSetUp>();
                if (Input.GetMouseButtonDown(0))
                {
                    if (holdContainer.childCount == 0)
                    {
                        if (batterySet.isSetted && blockSet.isSetted) return;
                        
                            ShowMessage(3f, "It's missing something");
                        return;
                        
                    }
                    if ( holdContainer.GetChild(0).name != "MotorBattery" && holdContainer.GetChild(0).name != "MotorBlock")
                    {
                        if (batterySet.isSetted && blockSet.isSetted) return;
                        ShowMessage(3f, "It's missing something");
                    }
                        
                    else ShowMessage(1f, "");
                }
                return;

            }
            if (hitTransform.name == "ButtonGarage")
            {
                
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                    if (holdContainer.childCount == 0 || holdContainer.GetChild(0).name != "Cue Stick")
                    {
                        //if (messageText.text == " ") return;
                        ShowMessage(3f, "You can't reach it");
                    }
                        
                    else
                    {
                        ShowMessage(1f, " ");
                    }
                    }
                return;
                
            }
            if(hit.transform.name == "InvoicePaper")
            {
                pc.isOnObject = true;
                nameText.text = "Invoice Paper";
                canvasButtonE.gameObject.SetActive(true);
                return;
            }
            if (hit.transform.name == "Laptop" ||
                hit.transform.name == "LapTopWithBattery")
            {
                OpenLaptop ol = hit.transform.GetComponent<OpenLaptop>();
                if(ol.IsSetBattery)
                {
                    pc.isOnObject = true;
                    
                    canvasButtonE.gameObject.SetActive(true);
                    return;
                }
                else
                {
                    if (holdContainer.childCount > 0)
                    {
                        if (holdContainer.GetChild(0).name != "LapBaterry")
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                ShowMessage(3f, "The laptop is missing a battery");
                                return;
                            }

                        }
                        else
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                ShowMessage(1f, "");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(3f, "The laptop is missing a battery");
                            return;
                        }
                    }
                }
                
            }
            
            if (hit.transform.name == "glassCabinet")
            {

                if (holdContainer.childCount > 0)
                {
                    if (holdContainer.GetChild(0).name != "Hammer")
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(3f, "You need to break the glass cabinet");
                            return;
                        }

                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(1f, "");
                            return;
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ShowMessage(3f, "You need to break the glass cabinet");
                        return;
                    }
                }
            }
            if (hit.transform.name == "VFX_Fire_Floor_01")
            {

                if (holdContainer.childCount > 0)
                {
                    if (holdContainer.GetChild(0).name != "Ex")
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(3f, "The fire is too hot!");
                            return;
                        }

                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(1f, "");
                            return;
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ShowMessage(3f, "The fire is too hot!");
                        return;
                    }
                }
            }
            if (hit.transform.name == "BoxElectric")
            {

                if (holdContainer.childCount > 0)
                {
                    if (holdContainer.GetChild(0).name != "Handle")
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(3f, "It's missing a handle");
                            return;
                        }

                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(1f, "");
                            return;
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ShowMessage(3f, "It's missing a handle");
                        return;
                    }
                }
            }
            
            if (hit.transform.name == "Object_314")
            {
                if (countListComplete < 5)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ShowMessage(3f, "You haven’t completed all objectives yet");
                        return;
                    }
                }
                else {
                    if (holdContainer.childCount > 0)
                    {
                        if (holdContainer.GetChild(0).name != "CarKey")
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                ShowMessage(3f, "You must bring the car key to open the door");
                                return;
                            }

                        }
                        else
                        {
                            if (carHood.transform.localRotation.x != 0)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    ShowMessage(3f, "You need to close the car hood");
                                    return;
                                }
                            }
                            else
                            { if (Input.GetMouseButtonDown(0))
                                {
                                    CarRun carRun = carHood.transform.parent.parent.parent.GetComponent<CarRun>();
                                    
                                    if (carRun.IsBossInRange())
                                    {
                                        ShowMessage(3f, "The Boss is too close!");
                                    }
                                }
                                
                            }
                           
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ShowMessage(3f, "You must bring the car key to open the door");
                            return;
                        }
                    }
                }
                    

            }
            if (hitTransform.CompareTag("PickUp"))
            {
                
                    Transform parent = hitTransform.parent;
                    Transform grandParent = parent != null ? parent.parent : null;

                    // 👉 TH1: chính object có PickUpConfig
                    PickUpConfig selfConfig = hitTransform.GetComponent<PickUpConfig>();
                    if (selfConfig != null)
                    {
                        if (parent == null || parent.name != "holdContainer")
                        {
                            nameText.text = selfConfig.NameObject;
                          
                            pc.isOnObject = true;
                        if (hitTransform.name == "Tranquilizer dart" || hitTransform.name == "Used tranquilizer dart")
                        {

                            if(holdContainer.childCount > 0)
                            {
                                if (holdContainer.GetChild(0).name == "tranquilizer_gun")
                                {
                                    canvasButtonE.gameObject.SetActive(true);
                                }
                                return;
                            }
                           

                        }
                        if (holdContainer.childCount > 0) return;
                            canvasButtonE.gameObject.SetActive(true);

                            return;
                        }
                    }

                    // 👉 TH2: cha có PickUpConfig
                    if (parent != null)
                    {
                        PickUpConfig parentConfig = parent.GetComponent<PickUpConfig>();
                        if (parentConfig != null)
                        {
                            if (grandParent == null || grandParent.name != "holdContainer")
                            {
                                nameText.text = parentConfig.NameObject;
                                pc.isOnObject = true;
                                if (holdContainer.childCount > 0) return;
                                canvasButtonE.gameObject.SetActive(true);
                                return;
                            }
                        }
                    }
                
                
            }
        }
       
            // Không tìm thấy thì clear
            nameText.text = "";
        
        canvasButtonE.gameObject.SetActive(false);
        if (isRayTrue)
        {

            return;
        }
        pc.isOnObject = false;
        pc.isNotCorrect = false;
        canvasButtonLMB.gameObject.SetActive(false);
        
        
        canvasButtonERotate.gameObject.SetActive(false);
        canvasButtonQRotate.gameObject.SetActive(false);
        
        canvasButtonLMBRed.gameObject.SetActive(false);
    }
 
    public void SetClearText()
    {

        nameText.text = "";
        
    }
    public void ShowMessage(float time, string text)
    {
        // Nếu đang chạy coroutine cũ thì dừng
        if (messageCoroutine != null) StopCoroutine(messageCoroutine);

        // Bắt đầu coroutine mới
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(time, text));
    }

    private IEnumerator ShowMessageCoroutine(float time, string text)
    {
        messageText.text = text;
        yield return new WaitForSeconds(time);
        messageText.text = "";
        messageCoroutine = null;
    }
    public void SetFalseTab()
    {
        canvasButtonTab.gameObject.SetActive(false);
    }
    public void SetTrueTab()
    {
        canvasButtonTab.gameObject.SetActive(true);
    }
}
