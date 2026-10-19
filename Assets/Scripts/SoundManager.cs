using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundData> allSfxSounds;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [SerializeField] private float masterVolume = 0.3f;
    [SerializeField] private float musicVolume = 0.001f;
    [SerializeField] private float sfxVolume = 0.7f;
    
    private readonly Dictionary<string, AudioSource> loopingSfx = new();
    
    private Scenes _currScene = Scenes.TitleScreen;

    public Scenes CurrScene
    {
        get { return _currScene; }
        set { _currScene = value; }
    }


    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Setup();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Setup()
    {
        Debug.Log("Setup for sounds done");
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
    }

    public void PlaySFX(string name)
    {
        SoundData sound = allSfxSounds.Find(s => s.soundName == name);

        if (sound == null)
        {
            Debug.Log("SFX Sound not found: " + name);
            return;
        }

        sfxSource.volume = sound.volume;
        sfxSource.PlayOneShot(sound.soundClip);
    }
    
    
    public void StartLoopSFX(string name)
    {
        if (loopingSfx.ContainsKey(name))
            return;

        SoundData sound = allSfxSounds.Find(s => s.soundName == name);

        if (sound == null || sound.soundClip == null)
        {
            Debug.LogWarning("SFX Sound not found: " + name);
            return;
        }

        AudioSource source = gameObject.AddComponent<AudioSource>();

        source.clip = sound.soundClip;
        source.volume = sound.volume;
        source.loop = true;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;

        loopingSfx.Add(name, source);
        source.Play();
    }

    public void StopLoopSFX(string name)
    {
        if (!loopingSfx.TryGetValue(name, out AudioSource source))
            return;

        source.Stop();
        Destroy(source);
        loopingSfx.Remove(name);
    }

    public void StopAllLoopSFX()
    {
        foreach (AudioSource source in loopingSfx.Values)
        {
            source.Stop();
            Destroy(source);
        }

        loopingSfx.Clear();
    }

    public bool IsLoopSFXPlaying(string name)
    {
        return loopingSfx.TryGetValue(name, out AudioSource source)
               && source != null
               && source.isPlaying;
    }

    
    public void SetMasterVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("Master", 0);
        }
        else
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("Master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("Music", -9999);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        }
        Debug.Log("Music Volume: " + volume);
        Debug.Log( Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Music", volume);
    }

    public void SetSfxVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("SFX", -9999);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
        Debug.Log("SFX Volume: " + volume);
        Debug.Log( Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFX", volume);
    }
    
}
