using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PickUpProperties", menuName = "ScriptableObjects/PickUpProperties")]
public class PickUpPropertiesSO : ScriptableObject
{
    public string nameObject;           // 👉 tên hiển thị
    public Vector3 localRotationEuler;
    public Vector3 localScale;
    public Vector3 localPosition;
    public Vector3 worldRotationEuler;  // 👉 Rotation thế giới mong muốn khi gỡ ra

    [Header("Audio Clips")]
    public List<AudioClip> audioClips;  // 👉 danh sách audio clip có thể gán trong Inspector
}
