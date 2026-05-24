using UnityEngine;

[CreateAssetMenu(fileName = "NewDoorSound", menuName = "Interaction/Door Sound")]
public class DoorSoundSO : ScriptableObject
{
    
    public AudioClip openSound;

   
    public AudioClip closeSound;
}
