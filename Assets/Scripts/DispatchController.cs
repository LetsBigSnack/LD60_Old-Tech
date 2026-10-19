using System;
using Data;
using JetBrains.Annotations;
using UnityEngine;

public class DispatchController : MonoBehaviour, IPunchcardReceiver
{

    public static DispatchController Instance { get; private set; }
    
    [SerializeReference] private PunchCardData insertedKeycard = null;
    
    public PunchCardData InsertedKeycard => insertedKeycard;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public bool InsertPunchcard(PunchCardData card)
    {
        if (card == null || insertedKeycard != null)
            return false;

        insertedKeycard = card;

        if (!SoundManager.Instance.IsLoopSFXPlaying("Tape"))
        {
            SoundManager.Instance.StartLoopSFX("Tape");
        }
        
        return true;
    }

    public void RemovePunchcard()
    {
        SoundManager.Instance.StopLoopSFX("Tape");
        insertedKeycard = null;
    }
}
