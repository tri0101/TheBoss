using UnityEngine;

public class CallSoundWhenActiveFire : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        AudioManager.instance.PlayLoopSFXAtParent(AudioManager.instance.fire, transform, 0.5f, 2.3f, 1f);
    }
}
