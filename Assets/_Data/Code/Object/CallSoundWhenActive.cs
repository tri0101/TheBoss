using UnityEngine;

public class CallSoundWhenActive : MonoBehaviour
{

    private void Start()
    {
        AudioManager.instance.PlayLoopSFXAtParent(AudioManager.instance.elec, transform, 0.5f, 1.5f, 1f);
    }
}
