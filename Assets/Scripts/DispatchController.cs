using System;
using Data;
using UnityEngine;

public class DispatchController : MonoBehaviour, IPunchcardReceiver
{

    public static DispatchController Instance { get; private set; }
    
    private PunchCardData insertedKeycard;
    
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
        
        return true;
    }

    public void RemovePunchcard()
    {
        insertedKeycard = null;
    }
}
