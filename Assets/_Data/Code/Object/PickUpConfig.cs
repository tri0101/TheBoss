using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickUpConfig : MonoBehaviour
{
    public PickUpPropertiesSO properties;
    [SerializeField] private bool isFalling;
    public bool IsFalling { get => isFalling; set => isFalling = value; }
    public string NameObject;
   
    private AudioSource audioSource;
    [SerializeField] private string[] groundLayers = { "WoodFloor", "GardenBake", "Grass", "WoodFloorOutDoor" };// đặt layer này trong Unity

    [SerializeField] private Transform enemy;
    [SerializeField] private FieldOfView fieldEnemy;
    
    private void Awake()
    {
        if (properties != null) NameObject = properties.nameObject;
        else NameObject = gameObject.name;

            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    private void Start()
    {
        GameObject enemyFind = GameObject.FindGameObjectWithTag("Enemy");
        enemy = enemyFind.transform;
        fieldEnemy = enemyFind.GetComponent<FieldOfView>();
    }
    public void ChangeNameObject(string name)
    {
        NameObject = name;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isFalling) return;

        int collisionLayer = collision.gameObject.layer;

        foreach (string layerName in groundLayers)
        {
            if (collisionLayer == LayerMask.NameToLayer(layerName))
            {
                if (gameObject.name == "Whisky_Bottle")
                {
                    Destructible des = GetComponent<Destructible>();
                    if (des != null)
                        des.CallDestruct();
                }

                if (properties != null && properties.audioClips != null && properties.audioClips.Count > 0)
                {
                    AudioManager.instance.PlaySFXAtPosition(properties.audioClips[0], transform.position);
                    fieldEnemy.CanHearSound(transform);
                    Debug.Log("Dã gọi");
                }

                isFalling = false;
                break;
            }
        }
    }
}
