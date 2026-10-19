using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData : ScriptableObject
{
    public string soundName;
    public AudioClip soundClip;
    public float volume = 1;
}