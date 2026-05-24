using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    //[SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject cameraPadlock;
    [SerializeField] private GameObject player;

    public void Run()
    {
        
        if (player != null) player.SetActive(false);
        if (cameraPadlock != null) cameraPadlock.SetActive(true);
    }
}