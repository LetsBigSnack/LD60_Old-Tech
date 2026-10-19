using System;
using UnityEngine;
using UnityEngine.UI;

public class UISoundManager : MonoBehaviour
{  
    
    public static UISoundManager Instance;
    
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    private void Start()
    {
        Debug.Log("I GOT CALLED");
        MusicVolumeChange();
        SFXVolumeChange();
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void MusicVolumeChange()
    {
        if (musicSlider == null)
        {
            Debug.Log("No music slider");
            return;
        }

        float volume = musicSlider.value;
        SoundManager.Instance.SetMusicVolume(volume);
    }

    public void SFXVolumeChange()
    {
        if (sfxSlider == null)
        {
            Debug.Log("No music slider");
            return;
        }
        
        float volume = sfxSlider.value;
        SoundManager.Instance.SetSfxVolume(volume);
    }
}